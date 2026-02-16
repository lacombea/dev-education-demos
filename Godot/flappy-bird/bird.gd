extends CharacterBody2D

const GRAVITY = 900
const JUMP_FORCE = -300

func _physics_process(delta):
	velocity.y += GRAVITY * delta
	
	if Input.is_action_just_pressed("ui_accept"):
		velocity.y = JUMP_FORCE
	
	move_and_slide()
