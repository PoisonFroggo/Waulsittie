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
/*
using Godot;
using System;
using System.Diagnostics.CodeAnalysis;


public partial class mbns_player_controller : Node3D
{

	
	[Export] public const float Speed = 100.0f;
	[Export] public float JumpVelocity { get; set; }

	[Export] public string IdleAnimationName { get; set; } //idle animation

	[Export] public Node3D CameraNode { get; set; }
	//Only needed if the player has a model
	//[Export] public Node3D PlayerModel { get; set; }
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


	private Vector3 camTargetRotation;
	//private Vector3 bodyTargetRotation;
	private Vector3 rootTargetRotation;
	private Vector3 Velocity;
	private Vector3 collide_location;

	public bool IsOnFloor = true;


	private float _rotationX = 0f;
	private float _rotationY = 0f;
	private float lookSensitivity = -.01f;

	public enum State {
		Grounded,
		Midair
	}

	public enum GroundedStates {
		Walk,
		Run,
		Crouch,
		Slide
	}

	public enum MidairStates {
		Glide,
		Dash
	}

	public enum DualStates {
		Damaged
	}

	
	public override void _Ready()
	{
		// What the hell is this
		//lock the mouse cursor
		Input.MouseMode = Input.MouseModeEnum.Captured;

		PlayerRoot.LinearDamp = LinearDamping;
	}

	public override void _PhysicsProcess(double delta)
	{

		Vector3 velocity = Velocity;				//local variable velocity equals the value of the universal variable Velocity
		Vector3 downDir = new Vector3(0, -1, 0);    //local variable downDir equals a new 3D vector pointing down

		float dist = 0;								//local variable dist is declared, a value of 0 is given
		float rideHeight = 1;						//local variable rideHeight is declared, a value of 1 is given. rideHeight is the desired height of the player in a Grounded state

		if (GroundHeightRay.IsColliding()) {
			IsOnFloor = true;
			collide_location = GroundHeightRay.GetCollisionPoint();
			dist = collide_location.DistanceTo(PlayerRoot.GlobalPosition);

			GD.Print(dist);
			GD.Print(collide_location);
		}
		else {
			IsOnFloor = false;
		}

		float rayDirVel = downDir.Dot(velocity);
		float x = dist - rideHeight;
		float springForce = x * SpringRideForce;    // - (relVel * rideSpringDamper);

		
		GD.Print("Velocity = " + Velocity);
		GD.Print("velocity = " + velocity);
		//GD.Print(PlayerRoot.LinearDamp);
		GD.Print("is grounded = " + IsOnFloor);
		 

		// Add the gravity.
		if (!IsOnFloor) {
			velocity.Y -= 25;
			float z = velocity.Y;
			x = collide_location.DistanceTo(GroundHeightRay.GlobalPosition);
			Vector3 y = new Vector3(0, z, 0);
			PlayerRoot.ApplyCentralForce(y);
		}
		else {
	//_RB.AddForce(rayDir * springForce);
			Vector3 dY = new Vector3(0, velocity.Y * springForce, 0);
			GD.Print("dY = " + dY);
			//Make function to handle jump
			PlayerRoot.ApplyCentralForce(dY);
		}

		static void Jump() 
		{
			
		}

		
		// Handle Jump.
		if (Input.IsActionJustPressed("jump") & IsOnFloor)
			velocity.Y += JumpVelocity;


		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		//use applycentralforce for rigidbody
		//PlayerRoot.ApplyCentralForce(velocity);

		//handle camera up/down (x) rotation
		CameraNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraNode.Rotation.X, Mathf.DegToRad(camTargetRotation.X), CameraActualRotationSpeed * (float)delta),
			0,
			0
			);
		//handle player side to side (y) rotation
		PlayerRoot.Rotation = new Vector3(
				0,
				Mathf.LerpAngle(PlayerRoot.Rotation.Y, Mathf.DegToRad(rootTargetRotation.Y), RootActualRotationSpeed * (float)delta),
				0
			);
		Velocity = velocity;
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

	private void ImplementMovement()
	{

	}
}
*/