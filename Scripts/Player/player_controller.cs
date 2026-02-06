/*
Base hover template code

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
using PlayerFuncs;
using System.Runtime.CompilerServices;
public partial class player_controller : Node3D
{
	[ExportGroup("Height Spring")]
	[Export] public float RideSpringStrength = 50f;

	[ExportGroup("Misc")]
	[Export] public float Speed {get; set;} = 900.0f;
	[Export] public float JumpVelocity { get; set; }
	[Export] public AnimationPlayer AnimationPlayer { get; set; }
	[Export] public string KickAnimName { get; set; } //idle animation
	[Export] public Skeleton3D Skel { get; set; } //player skeleton
	[Export] public Node3D LeftLegLocation { get; set; } 
	//The location to set the left leg when playing the kick animation so that the player can see the animation 
	//(I could spawn a new leg at this location, but that looks less janky and silly)

	[Export] public Node3D CameraNode { get; set; }
	[Export] public RayCast3D InteractRay { get; set; }
	//Only needed if the player has a model
	//[Export] public Node3D PlayerModel { get; set; }
	[Export] public RigidBody3D PlayerRoot { get; set; }
	[Export] public float RotationSpeed { get; set; }
	[Export] public float CameraActualRotationSpeed { get; set; }
	[Export] public float BodyActualRotationSpeed { get; set; }
	[Export] public float RootActualRotationSpeed { get; set; }
	[Export] public float VerticalRotationLimit { get; set; } = 90; //want the player to be able to spin when in midair, so this may prove unnecessary in the future

	[Export] public ShapeCast3D GroundHeightCast {get; set;}
	[Export] public float LinearDamping { get; set; } = 7;
	[Export] public float GravityMultiplier { get; set; } = 4;

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

	[ExportGroup("Packed scenes")]
	[Export] PackedScene pissOrb { get; set; }
	[Export] public ShapeCast3D footCol {get; set;}

	[ExportGroup("Hover controls")]
	[Export] float rideHeight { get; set; } = 2f;
	[Export] public float RideSpringDamper = 5f;
		float dist = 0;

	[ExportGroup("Controls")]
	[Export] float lookSensitivity = -.01f;
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

	[ExportGroup("Move settings")]
	[Export] public float stopFactor = 5f;

	[ExportGroup("URINATION PROCLAMATION")]
	[Export] public PackedScene PissOrbScene { get; set; }
	[Export] public float PissOrbLaunchForce { get; set; } = 20f;
	[Export] public float PissOrbUpwardForce { get; set; } = 7f;

	float dt;

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
	private State CurrentState = State.Grounded;

	
    // Add this signal for object collision
    [Signal] public delegate void ObjectHitEventHandler(Vector3 hitPosition, Node3D hitObject);
	
    [Signal] public delegate void GroundedEventHandler(Vector3 hitPosition, Node3D hitObject);

	private Vector3 _gravity;
	public override void _Ready()
	{
		GroundHeightCast.AddException(PlayerRoot);
		footCol.AddException(PlayerRoot);
		//lock the mouse cursor
		Input.MouseMode = Input.MouseModeEnum.Captured;
		PlayerRoot.LinearDamp = LinearDamping;
		_gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * Vector3.Down;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		dt = (float)delta;

		bool grounded = GroundHeightCast.IsColliding();

		//GD.Print(grounded);

		switch (CurrentState)
		{
			case State.Grounded:
				HandleGroundedState(grounded);
				break;
			case State.Midair:
				HandleMidairState(grounded);
				break;
		}

		ImplementMovement();
		ApplyGravity();
		MoveCamera();
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

		if (@event.IsActionPressed("jump"))
		{
			GD.Print("JUMPING");
			HandleJump();
		}

		if (@event.IsActionPressed("escape")){
			ToggleMouseMode();
		}

		if (@event.IsActionPressed("primary_fire")) {

		}

		if (@event.IsActionPressed("secondary_fire")) {
			
		}

		if (@event.IsActionPressed("kick"))
		{
			if (AnimationPlayer != null)
			{
				AnimationPlayer.Play(KickAnimName);
			}
			else
			{
				GD.PrintErr("AnimationPlayer is null. Cannot play animation.");
			}
			PlayerFuncs.PlayerFuncs.ProcessKickHits(
				footCol,
				PlayerRoot,
				InteractRay
			);
		//Regarding skeletons, local is relative to the Parent bone, and global is relative to the Skeleton itself.
		}
		if (@event.IsActionPressed("piss"))
		{
			SummonPissOrb();
		}
	}

	private void HandleGroundedState(bool grounded)
	{
		//GD.Print("Starting grounded state");
		if (!grounded)
		{
			CurrentState = State.Midair;
			return;
		}

		if (!PlayerFuncs.PlayerFuncs.GetClosestHit(
			GroundHeightCast,
			out Vector3 hitPoint,
			out Vector3 hitNorm,
			out RigidBody3D hitBody
		))
		{
			return;
		}

		Vector3 downDir = Vector3.Down;
		Vector3 toHit = hitPoint - PlayerRoot.GlobalPosition;
		float dist = downDir.Dot(toHit);
		Vector3 velocity = PlayerRoot.LinearVelocity;
		Vector3 platformVelocity = Vector3.Zero;

		//float dist = hitPoint.DistanceTo(PlayerRoot.GlobalPosition);
		//GD.Print(hitPoint);
		//GD.Print(PlayerRoot.GlobalPosition);

		float rayDirVel = downDir.Dot(velocity);
		float x = dist - rideHeight;
		//GD.Print("dist: " + dist);
		//GD.Print("rideHeight: " + rideHeight);
		//GD.Print("x: " + x);


		float springForce =
			(x * RideSpringStrength) -
			(rayDirVel * RideSpringDamper);

		Vector3 force = Vector3.Down * springForce;
		GD.Print("Force: " + force);
		float normY = hitNorm.Y;
		float normX = hitNorm.X;
		float normZ = hitNorm.Z;
		GD.Print(normY);

		if(normY >= .1)
		{
			PlayerRoot.ApplyCentralForce(force);
		}
	}

	private void HandleMidairState(bool grounded)
	{
		GD.Print("Starting midair state");
		if (grounded)
		{
			CurrentState = State.Grounded;
			GD.Print("transition to grounded state");
			return;
		}
	}

	private bool CheckGrounded(bool rayHitGround, RayCast3D rayCast)
	{
		bool grounded;
		Vector3 collideLoc = rayCast.GetCollisionPoint();
		float collideDist = collideLoc.DistanceTo(PlayerRoot.GlobalPosition);
		if (rayHitGround == true)
		{
			grounded = collideDist <= rideHeight * 1.3f;
		}
		else
		{
			grounded = false;
		}
		return grounded;
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
		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 direction = (PlayerRoot.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		Vector3 desiredVelocity = direction * Speed;
		Vector3 currentVelocity = PlayerRoot.LinearVelocity;
		Vector3 neededAccel = desiredVelocity - new Vector3(currentVelocity.X, 0, currentVelocity.Z);
		Vector3 force = neededAccel * PlayerRoot.Mass;
		if (inputDir == Vector2.Zero && GroundHeightCast.IsColliding() == true)
		{
			Vector3 horizontalVelocity = new Vector3(currentVelocity.X, 0, currentVelocity.Z);
			Vector3 deceleration = -horizontalVelocity * stopFactor;
			force = force + deceleration;
		}

		PlayerRoot.ApplyCentralForce(force); //moves player
	}

	private void HandleJump()
	{
		if(GroundHeightCast.IsColliding() == true)
		{
			
		PlayerRoot.ApplyCentralForce(new Vector3(0, JumpVelocity, 0));
		}
	}

	private void MoveCamera()
	{
		//handle camera up/down (x) rotation
		CameraNode.Rotation = new Vector3(
			Mathf.LerpAngle(CameraNode.Rotation.X, Mathf.DegToRad(camTargetRotation.X), CameraActualRotationSpeed * (float)dt),
			0,
			0
			);
		//handle player side to side (y) rotation
		PlayerRoot.Rotation = new Vector3(
				0,
				Mathf.LerpAngle(PlayerRoot.Rotation.Y, Mathf.DegToRad(rootTargetRotation.Y), RootActualRotationSpeed * (float)dt),
				0
			);
	}

	private void ApplyGravity()
	{
		PlayerRoot.ApplyCentralForce(_gravity * GravityMultiplier * PlayerRoot.Mass);
	}

private void MoveLeg() 
	{		
	// Get the parent bone's global location (Vector3)
	Vector3 hipOriginGlobalTransformReal = Skel.ToGlobal(Skel.GetBoneGlobalPose(44).Origin);
	Vector3 leftLegLocationGlobal = LeftLegLocation.GlobalTransform.Origin;						
	// Calculate the difference (offset) between the current bone position and the target position (LeftLegLocation)
	Vector3 offset = leftLegLocationGlobal - hipOriginGlobalTransformReal;

	// Now apply the offset to the bone's local position
	Vector3 boneLocalTransform = Skel.ToLocal(Skel.GetBoneGlobalPose(44).Origin);
	boneLocalTransform += offset; // Apply the offset to the bone's local transform

	// Set the new local transform for the bone
	Skel.SetBonePosePosition(44, boneLocalTransform);

	//GD.Print("LEFT LEG LOC " + LeftLegLocation.Transform.Origin);
	}

private void SummonPissOrb()
	{
		if (PissOrbScene == null)
			return;

		// Instantiate the projectile
		RigidBody3D orb = PissOrbScene.Instantiate<RigidBody3D>();

		// Spawn position (slightly in front of camera)
		Vector3 spawnPosition =
			CameraNode.GlobalTransform.Origin +
			(-CameraNode.GlobalTransform.Basis.Z * 1.0f) +
			(Vector3.Up * 0.3f);

		orb.GlobalTransform = new Transform3D(
			Basis.Identity,
			spawnPosition
		);

		GetTree().CurrentScene.AddChild(orb);

		// Forward direction (camera-facing)
		Vector3 forwardDirection = -CameraNode.GlobalTransform.Basis.Z.Normalized();

		// Launch impulse
		Vector3 launchImpulse =
			(forwardDirection * PissOrbLaunchForce) +
			(Vector3.Up * PissOrbUpwardForce);

		orb.ApplyCentralImpulse(launchImpulse);
	}

}
