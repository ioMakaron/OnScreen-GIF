extends Window

var is_dragging = false
var mouse_offset = Vector2.ZERO
#
#func _ready() -> void:
	#size = $Node2D/Sprite2D.get_rect().size

func _process(delta: float) -> void:
	if G.edit_mode:
		self.borderless = false
	else:
		self.borderless = true
func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.is_pressed():
			if self.has_focus():
			# Check if the click is within the visible rect of the window
				#if get_visible_rect().has_point(event.position):
					is_dragging = true
					mouse_offset = event.position # Store the click position relative to the window
		else:
			is_dragging = false

	if event is InputEventMouseMotion and is_dragging:
		if self.has_focus():
		# Move the window by the difference between the current and initial mouse positions
			position += Vector2i(event.position.x,event.position.y)  - Vector2i(mouse_offset.x,mouse_offset.y) 

	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_RIGHT:
		if event.is_pressed():
			#if self.has_focus():
				if get_visible_rect().has_point(event.position):
					self.queue_free()
