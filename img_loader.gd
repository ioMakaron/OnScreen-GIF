extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	var image = Image.new()
	image.load(G.path)
	var t = ImageTexture.new()
	t.create_from_image(image)
	#$Sprite.texture = t
	t.set_image(image)
	$"../Node2D/Sprite2D".texture = GifManager.animated_texture_from_file(G.path)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
