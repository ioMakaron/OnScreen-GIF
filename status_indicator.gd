extends StatusIndicator

var cs_node

func menu() :
	
	$"..".visible =!$"..".visible
	$PopupMenu.set_item_checked(0,$"..".visible)

func _on_popup_menu_index_pressed(index: int) -> void:
	if index ==0:
		menu()
	if index ==1:
		get_tree().quit()


func _on_hide_button_pressed() -> void:
	menu()
