extends Control

@onready var window = preload("res://window.tscn")

# Called when the node enters the scene tree for the first time.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta: float) -> void:
	#$Window/Node2D/TextureRect.texture =$Window/Node2D/Sprite2D.texture
	

func _on_img_button_pressed() -> void:
	$FileDialog.visible = !$FileDialog.visible


func _on_file_dialog_files_selected(paths: PackedStringArray) -> void:
	print(paths)


func _on_file_dialog_file_selected(path: String) -> void:

	G.path = path
	
	var inst = window.instantiate()
	add_child(inst)
	

	


func _on_h_slider_value_changed(value: float) -> void:
	G.img_scale = value
	$MarginContainer/VBoxContainer/Label.text = "Image scale: "+ str(G.img_scale)
	$MarginContainer/VBoxContainer/HBoxContainer/SpinBox.value = value


func _on_spin_box_value_changed(value: float) -> void:
	G.img_scale = value
	$MarginContainer/VBoxContainer/Label.text = "Image scale: "+ str(G.img_scale)
	$MarginContainer/VBoxContainer/HBoxContainer/HSlider.value = value


func _on_fps_spin_box_value_changed(value: float) -> void:
	Engine.max_fps = value


func _on_clone_img_button_pressed() -> void:
	if G.path != "":
		var inst = window.instantiate()
		add_child(inst)
	else:
		OS.alert("Nothing to clone","Alert!")
