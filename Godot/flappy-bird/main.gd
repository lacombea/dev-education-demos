extends Node2D

@export var pipe_scene : PackedScene
var score = 0

func _on_timer_timeout():
	var pipe = pipe_scene.instantiate()
	pipe.position.x = 1080
	pipe.position.y = randi_range(-250, 250)
	add_child(pipe)


func add_score():
	score += 1
	$ScoreLabel.text = "Score : " + str(score)


func game_over():
	$GameOverLabel.show()
	get_tree().paused = true
