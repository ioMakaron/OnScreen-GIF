using Hardcodet.Wpf.TaskbarNotification; // Используем NotifyIcon
using Microsoft.Win32; // Для OpenFileDialog
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OnScreenPlayer
{
    public partial class App : Application
    {
        private TaskbarIcon? _notifyIcon;
        private readonly List<BorderlessWindow> _borderlessWindows = new();
        private readonly List<StandardWindow> _standardWindows = new();

        // Отслеживаем активное окно
        private Window? _currentActiveWindow;

        // Отслеживаем глобальный режим для всех окон
        private bool _isBorderlessModeForAll = true;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _notifyIcon = (TaskbarIcon)FindResource("MyNotifyIcon");

            // Создаем только одно BorderlessWindow при старте
            var initialBorderlessWindow = new BorderlessWindow();
            SubscribeToWindowEvents(initialBorderlessWindow); 
            initialBorderlessWindow.Show();
            _borderlessWindows.Add(initialBorderlessWindow);
            _currentActiveWindow = initialBorderlessWindow;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose(); // Обязательно очищаем иконку трея
            base.OnExit(e);
        }

        private void SubscribeToWindowEvents(Window window)
        {
            window.Closed += Window_Closed;
            window.Activated += Window_Activated;
        }

        private void Window_Activated(object? sender, EventArgs e)
        {
            if (sender is Window activeWindow)
            {
                _currentActiveWindow = activeWindow;
            }
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            // Отписываемся от событий закрытого окна
            if (sender is BorderlessWindow bw)
            {
                bw.Closed -= Window_Closed;
                bw.Activated -= Window_Activated;
                _borderlessWindows.Remove(bw);

                // Удаляем связанное StandardWindow, если оно существует и не было закрыто ранее
                var associatedStandard = _standardWindows.FirstOrDefault(sw => sw.Tag == bw);
                if (associatedStandard != null)
                {
                    associatedStandard.Closed -= Window_Closed; 
                    associatedStandard.Activated -= Window_Activated;
                    _standardWindows.Remove(associatedStandard);
                    associatedStandard.Close(); 
                }
            }
            else if (sender is StandardWindow sw)
            {
                sw.Closed -= Window_Closed;
                sw.Activated -= Window_Activated;
                _standardWindows.Remove(sw);

                
                var associatedBorderless = _borderlessWindows.FirstOrDefault(bww => bww == sw.Tag);
                if (associatedBorderless != null)
                {
                    associatedBorderless.Closed -= Window_Closed; // Отписываемся перед закрытием
                    associatedBorderless.Activated -= Window_Activated;
                    _borderlessWindows.Remove(associatedBorderless);
                    associatedBorderless.Close(); // Закрываем
                }
            }

           
            if (_currentActiveWindow == sender)
            {
                _currentActiveWindow = null;
                
                if (_borderlessWindows.Any())
                {
                    _currentActiveWindow = _borderlessWindows.First();
                }
                else if (_standardWindows.Any())
                {
                    _currentActiveWindow = _standardWindows.First();
                }

                _currentActiveWindow?.Activate();
            }

            
            if (!_borderlessWindows.Any() && !_standardWindows.Any())
            {
                Shutdown();
            }
        }


        // --- Обработчики контекстного меню ---

        private void ToggleModeForAll_Click(object sender, RoutedEventArgs e)
        {
            _isBorderlessModeForAll = !_isBorderlessModeForAll; 
            if (_isBorderlessModeForAll)
            {
                // Переключаем все StandardWindow на Borderless
                // Копируем список, чтобы избежать ошибок при модификации во время итерации
                foreach (var standardWindow in _standardWindows.ToList())
                {
                    ShowBorderlessWindowForStandard(standardWindow);
                }
            }
            else
            {

                foreach (var borderlessWindow in _borderlessWindows.ToList())
                {
                    ShowStandardWindowForBorderless(borderlessWindow);
                }
            }
            // Активируем последнее активное окно (если есть), чтобы оно было на переднем плане после переключения
            _currentActiveWindow?.Activate();
        }

        private void ChangeCurrentGif_Click(object sender, RoutedEventArgs e)
        {
            if (_currentActiveWindow == null)
            {
                MessageBox.Show("Нет активного окна для изменения GIF.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GIF files (*.gif)|*.gif|All image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите GIF-файл";

            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                if (_currentActiveWindow is BorderlessWindow bw)
                {
                    bw.LoadGif(filename);
                }
                else if (_currentActiveWindow is StandardWindow sw)
                {
                    sw.LoadGif(filename);
                }
            }
        }

        private void CloneActiveWindow_Click(object sender, RoutedEventArgs e)
        {
            if (_currentActiveWindow == null) return;

            if (_currentActiveWindow is BorderlessWindow sourceBorderless)
            {
                CloneBorderlessWindow(sourceBorderless);
            }
            else if (_currentActiveWindow is StandardWindow sourceStandard)
            {
                CloneStandardWindow(sourceStandard);
            }
        }

        private void AddNewGifWindow_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GIF files (*.gif)|*.gif|All image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите GIF-файл для нового окна";

            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                var newBorderlessWindow = new BorderlessWindow();
                SubscribeToWindowEvents(newBorderlessWindow);
                newBorderlessWindow.LoadGif(filename);

                if (_currentActiveWindow != null)
                {
                    newBorderlessWindow.Left = _currentActiveWindow.Left + 30;
                    newBorderlessWindow.Top = _currentActiveWindow.Top + 30;
                    newBorderlessWindow.Width = _currentActiveWindow.Width;
                    newBorderlessWindow.Height = _currentActiveWindow.Height;
                }
                newBorderlessWindow.Show();
                _borderlessWindows.Add(newBorderlessWindow);
                _currentActiveWindow = newBorderlessWindow; 
            }
           
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        // --- Логика управления окнами ---

        // Переключение с Borderless на Standard для конкретного BorderlessWindow
        private void ShowStandardWindowForBorderless(BorderlessWindow sourceBorderless)
        {
            // Проверяем, существует ли уже StandardWindow, связанный с BorderlessWindow
            StandardWindow? targetStandard = _standardWindows.FirstOrDefault(sw => sw.Tag == sourceBorderless);
            if (targetStandard == null)
            {
                // Если StandardWindow не существует, создаем новый
                targetStandard = new StandardWindow();
                targetStandard.Tag = sourceBorderless; 
                SubscribeToWindowEvents(targetStandard); 
                _standardWindows.Add(targetStandard);
            }


            targetStandard.Left = sourceBorderless.Left;
            targetStandard.Top = sourceBorderless.Top;
            targetStandard.Width = sourceBorderless.Width;
            targetStandard.Height = sourceBorderless.Height;
            targetStandard.ImageSource = sourceBorderless.ImageSource;

            sourceBorderless.Hide();
            targetStandard.Show();
            targetStandard.Activate();
        }

       
        private void ShowBorderlessWindowForStandard(StandardWindow sourceStandard)
        {
         
            BorderlessWindow? targetBorderless = sourceStandard.Tag as BorderlessWindow;
            if (targetBorderless == null)
            {
                // Если BorderlessWindow не существует
                // Создаем новое и связываем его
                MessageBox.Show("Связь между окнами утеряна. Создано новое безрамочное окно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                targetBorderless = new BorderlessWindow();
                SubscribeToWindowEvents(targetBorderless); // Подписываемся на события
                _borderlessWindows.Add(targetBorderless);
                sourceStandard.Tag = targetBorderless; // Устанавливаем связь
            }

     
            targetBorderless.Left = sourceStandard.Left;
            targetBorderless.Top = sourceStandard.Top;
            targetBorderless.Width = sourceStandard.Width;
            targetBorderless.Height = sourceStandard.Height;
            targetBorderless.ImageSource = sourceStandard.ImageSource;

            sourceStandard.Hide();
            targetBorderless.Show();
            targetBorderless.Activate();
        }

        // Клонирование безрамочного окна
        private void CloneBorderlessWindow(BorderlessWindow source)
        {
            var newBorderlessWindow = new BorderlessWindow();
            SubscribeToWindowEvents(newBorderlessWindow);

            newBorderlessWindow.Left = source.Left + 30;
            newBorderlessWindow.Top = source.Top + 30;
            newBorderlessWindow.Width = source.Width;
            newBorderlessWindow.Height = source.Height;
            newBorderlessWindow.ImageSource = source.ImageSource;

            newBorderlessWindow.Show();
            _borderlessWindows.Add(newBorderlessWindow);
            _currentActiveWindow = newBorderlessWindow;
        }

        // Клонирование стандартного окна
        private void CloneStandardWindow(StandardWindow source)
        {
            var newStandardWindow = new StandardWindow();
            SubscribeToWindowEvents(newStandardWindow);

            newStandardWindow.Left = source.Left + 30;
            newStandardWindow.Top = source.Top + 30;
            newStandardWindow.Width = source.Width;
            newStandardWindow.Height = source.Height;
            newStandardWindow.ImageSource = source.ImageSource;

            newStandardWindow.Show();
            _standardWindows.Add(newStandardWindow);
            _currentActiveWindow = newStandardWindow;
        }
    }
}
