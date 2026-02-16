extends Node2D

const SPEED = 200
var passed = false

func _process(delta):
	position.x -= SPEED * delta
	
	if position.x < -100:
		queue_free()


func _on_score_zone_body_entered(body):
	if body.name == "Bird" and not passed:
		passed = true
		get_tree().current_scene.add_score()


func _on_pipe_body_entered(body: Node2D) -> void:
	if body.name == "Bird":
		get_tree().current_scene.game_over()
