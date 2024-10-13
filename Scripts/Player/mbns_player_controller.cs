/*
VVV hover template code

if (_rayDidHit)
{
	Vector3 vel = RB.velocity;
	Rigidbody hitBody = _rayHit.rigidbody;
	if (hitBody != null)
	{
		otherVel = hitBody.velocity;
	}

	float rayDirVel = Vector3.Dot(rayDir, vel;
	float otherDirVel = Vector3.Dot(rayDir, otherVel);

	float relVel = rayDirVel - otherDirVel;

	float x = _rayHit.distance - RideHeight;

	float springForce = (x * RideSpringStrength) - (relVel * RideSpringDamper);

	_RB.AddForce(rayDir * springForce);
}

*/

using Godot;
using System;


public partial class mbns_player_controller : Node3D
{
	[Export] public float Speed = 100.0f;
	[Export] public float JumpVelocity { get; set; }
	[Export] public string IdleAnimationName { get; set; } //idle animation

	[Export] public Node3D CameraNode { get; set; }
	[Export] public RigidBody3D PlayerRoot { get; set; }
	[Export] public float RotationSpeed { get; set; }
	[Export] public float CameraActualRotationSpeed { get; set; }
	[Export] public float BodyActualRotationSpeed { get; set; }
	[Export] public float RootActualRotationSpeed { get; set; }
	[Export] public float VerticalRotationLimit { get; set; } = 90; //we want the player to be able to spin when in midair, so this may prove unnecessary in the future

	[Export] public RayCast3D GroundHeightRay { get; set;}
	[Export] public float SpringRideForce { get; set; } = 30;
	[Export] public float LinearDamping { get; set; } = 7;
	[Export] public float gravity { get; set; } = 80;


	private GodotObject otherObj;


	private Vector3 camTargetRotation;
	//private Vector3 bodyTargetRotation;
	private Vector3 rootTargetRotation;
	private Vector3 Velocity;
	private Vector3 collide_location;
	private Vector3 otherVel = new Vector3(0,0,0);

	private Vector3 oldLoc = new Vector3(0,0,0);
	private Vector3 currentLoc = new Vector3(0,0,0);

	public bool IsOnFloor = true;

	private float _rotationX = 0f;
	private float _rotationY = 0f;
	private float lookSensitivity = -.01f;

	private float rideSpringDamper = 4;

	[ExportGroup("Controls")]
	[Export] string JUMP = "ui_accept";
	[Export] string LEFT = "ui_left";
	[Export] string RIGHT = "ui_right";
	[Export] string FORWARD = "ui_up";
	[Export] string BACKWARD = "ui_down";
	[Export] string PAUSE = "ui_cancel";
	[Export] string CROUCH = "crouch";
	[Export] string SPRINT = "sprint";
	
	[ExportGroup("Controller support")] //Controller support
	[Export] string LOOK_LEFT = "look_left";
	[Export] string LOOK_RIGHT = "look_right";
	[Export] string LOOK_UP = "look_up";
	[Export] string LOOK_DOWN = "look_down";

	[ExportGroup("Feature Settings")]
	[Export] bool jumping_enabled = true;
	[Export] bool in_air_momentum = true;
	[Export] bool motion_smoothing = true;
	[Export] bool sprint_enabled = true;
	[Export] bool crouch_enabled = true;
	[Export] bool dynamic_fov = true;
	[Export] bool continuous_jumping = true;
	[Export] bool view_bobbing = true;
	[Export] bool jump_animation = true;
	[Export] bool pausing_enabled = true;
	[Export] bool gravity_enabled = true;

	private State currentState = State.Grounded;

	public enum crouch_mode {
		HoldCrouch,
		ToggleCrouch
	}

	public enum sprint_mode {
		HoldSprint,
		ToggleSprint
	}

	public enum State { //Types of state, these are mutually exclusive
		Grounded,
		Midair
	}

	public enum GroundedStates { //states that only happen on the ground
		Walk,
		Run,
		Crouch,
		Slide
	}

	public enum MidairStates { //states that only happen in the air
		Glide,
		Dash,
		Slam
	}

	public enum DualStates { //can happen while both grounded and midair
		LeaningLeft,
		LeaningRight
	}


	
	//Default states of the character on load
	private string CurrentState = "Grounded";
	private string CurrentSubState = "Walk";

	
	public override void _Ready()
	{
		//lock the mouse cursor for first person
		Input.MouseMode = Input.MouseModeEnum.Captured;

		//set linear damping
		PlayerRoot.LinearDamp = LinearDamping;

		//Initialize cam rotations
		camTargetRotation = Vector3.Zero;
		rootTargetRotation = Vector3.Zero;

		//Set initial state
		CurrentState = "Grounded";
	}

		float dist = 0;
		float rideHeight = 4;
	public override void _PhysicsProcess(double delta)
	{
		UpdateState();
		HandleMovement();
		HandleJump();

		ApplyMovement();
		UpdateCameraRotation(delta);


		Vector3 downDir = new Vector3(0, -1, 0);    //local variable downDir equals a new 3D vector pointing down

		float rayDirVel = downDir.Dot(Velocity);
		//GD.Print("Velocity = " + velocity);
		
	// Grounding logic with spring force
	if (CurrentState == "Grounded" && GroundHeightRay.IsColliding())
	{
		Vector3 collideLocation = GroundHeightRay.GetCollisionPoint();
		float distanceToGround = collideLocation.DistanceTo(PlayerRoot.GlobalPosition);
		
		// Spring force calculation
		float x = distanceToGround - rideHeight;  // Calculate compression/expansion
		float springForce = x * SpringRideForce - rayDirVel * rideSpringDamper; // Basic spring force equation

		// Apply spring force
		PlayerRoot.ApplyCentralForce(downDir * springForce);
	}

	// Apply gravity if in midair
	if (CurrentState == "Midair")
	{
		Velocity.Y -= gravity * (float)delta; // Apply gravity over time
	}
	}
		
	public override void _Input(InputEvent @event)
	{
		//detects and responds to user mouse movement
		if (@event is InputEventMouseMotion mouseMotion)
		{
			//rotate the characterbody node on the Y axis when mouse is moved side to side
			//rotate characterbody node on the X axis when mouse is moved up and down (later change this to the hip bone on the player skeleton)
			//Debug.WriteLine("mouse input detected");
			//calculate camera x rotation
			camTargetRotation = new Vector3(
				Mathf.Clamp((-1 * mouseMotion.Relative.Y * RotationSpeed) + camTargetRotation.X, -VerticalRotationLimit, VerticalRotationLimit),
				0, 
				0);
			//calculate y rotation of the entire player
			rootTargetRotation = new Vector3(
				0,
				Mathf.Wrap((-1 * mouseMotion.Relative.X * RotationSpeed) + rootTargetRotation.Y, 0, 360), 
				0
			);
		
		}

		if (@event.IsActionPressed("escape")){
			ToggleMouseMode();
		}

		if (@event.IsActionPressed("primary_fire")) {

		}

		if (@event.IsActionPressed("secondary_fire")) {
			
		}
	}

	private void ToggleMouseMode()
	{
		if(Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			Input.MouseMode =Input.MouseModeEnum.Captured;
		}
		else 
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	private void UpdateState() {
		if (GroundHeightRay.IsColliding()) {
			CurrentState = "Grounded";
		}
		else{
			CurrentState = "Midair";
		}
	}

	private void HandleMovement() {
		Vector2 inputDir = Input.GetVector(LEFT, RIGHT, FORWARD, BACKWARD);
		Vector3 direction = PlayerRoot.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

		if (direction.LengthSquared() > 0)
		{
			Velocity.X = direction.X * Speed;
			Velocity.Y = direction.Y * Speed;
		} else {
			Velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			Velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
	}

	private void HandleJump() {
		if (currentState == State.Grounded && Input.IsActionJustPressed("jump")) 
		{
			Velocity.Y += JumpVelocity;
		}
	}

	private void ApplyMovement() {
		PlayerRoot.ApplyCentralForce(Velocity);
	}

	private void UpdateCameraRotation(double delta) {
		CameraNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraNode.Rotation.X, Mathf.DegToRad(camTargetRotation.X), CameraActualRotationSpeed * (float)delta),
			0,
			0
		);

		PlayerRoot.Rotation = new Vector3(
			0,
			Mathf.LerpAngle(PlayerRoot.Rotation.Y, Mathf.DegToRad(rootTargetRotation.Y), RootActualRotationSpeed * (float)delta),
			0
		);
	}


	private void ImplementMovement()
	{
		if (sprint_enabled && Input.IsActionPressed(SPRINT)) {
			Velocity *= 1.5f;
		}

		//crouch stuff here
	}

	private void enterState(State newState) //base state enter function
	{
		switch (newState) {
			case State.Grounded:
				//grounded stuff
				break;
			case State.Midair:
				//Midair stuff
				break;
		}
	}
}
