extends Control
@onready var file_dialog: FileDialog = $"../FileDialog"
const WINDOW = preload("uid://dgn3ejxfa6x25")



# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_file_dialog_file_selected(path: String) -> void:
	G.path = path
	
	var inst = WINDOW.instantiate()
	#inst.position = DisplayServer.window_get_size() / 2
	get_tree().root.add_child(inst)


func _on_select_img_button_pressed() -> void:
	file_dialog.show()


func _on_clone_btn_pressed() -> void:
	var inst = WINDOW.instantiate()
	get_tree().root.add_child(inst)
	


		


func _on_edit_btn_toggled(toggled_on: bool) -> void:
	G.edit_mode = toggled_on


func _on_spin_box_value_changed(value: float) -> void:
	G.img_scale = value


func _on_fps_spin_box_value_changed(value: float) -> void:
	Engine.max_fps = value
