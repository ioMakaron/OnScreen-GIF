using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OnScreenPlayer
{
    public partial class BorderlessWindow : Window, INotifyPropertyChanged
    {
        private BitmapImage? _imageSource;

        public event PropertyChangedEventHandler? PropertyChanged;

        public BorderlessWindow()
        {
            InitializeComponent();
            DataContext = this;

            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.ResizeMode = ResizeMode.NoResize;
            this.Topmost = true;
            this.ShowInTaskbar = false;

            TryLoadDefaultGif();

            this.MouseDown += Window_MouseDown;
            
        }

        public BitmapImage? ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void TryLoadDefaultGif()
        {
            string defaultGifPath = "pack://application:,,,/Assets/sample.gif";
            try
            {
                LoadGif(defaultGifPath);
            }
            catch (Exception)
            {
                ImageSource = null;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Здесь может быть любая другая специфичная для окна логика горячих клавиш
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove(); 
            }
            else if (e.ChangedButton == MouseButton.Right) 
            {
                this.Close();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }

        public void LoadGif(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (path.StartsWith("pack://application:,,,/", StringComparison.OrdinalIgnoreCase))
                {
                    bitmap.UriSource = new Uri(path);
                }
                else
                {
                    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                }
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                ImageSource = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке GIF '{path}':\n{ex.Message}", "Ошибка загрузки GIF", MessageBoxButton.OK, MessageBoxImage.Error);
                ImageSource = null;
                throw;
            }
        }
    }
}
