extends Node


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	DisplayServer.window_set_size(DisplayServer.screen_get_size())
