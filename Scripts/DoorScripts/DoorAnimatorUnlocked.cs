using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

//This script allows for the player to open the door, currently only implemented as kicking. Eventually make helper functions that allow for breaking up a collider into chunks
//and making different things happen based on what chunk has been kicked
public partial class DoorAnimatorUnlocked : Node3D, IKickable
{
	[ExportGroup("Hinges")]
	[Export] public Node3D RightHinge;
	[Export] public Node3D WrongHinge;
	[Export] public Node3D TopHinge;
	[Export] public Node3D BottomHinge;
	[ExportGroup("Misc")]
	[Export] public Node3D rootPoint;
	private AnimationPlayer anim;
	public enum DoorStates
	{
		OpenIn,
		OpenOut,
		Closed,
		Wyoming,
		WrongSideIn,
		WrongSideOut,
		OpenInUp,
		OpenOutUp,
		OpenInDown,
		OpenOutDown
	}
	private DoorState currentState;
	private Dictionary<DoorStates, DoorState> states = new Dictionary<DoorStates, DoorState>();
	private bool isLocked = false;
	private bool isOpen = false;
	private Vector3 doorForward;
	private int x = 0;

	private int baseRotY = 0;
	private float currentRotY = 0;

	private float rightRotY;

	//Step 1: get bounding box of the mesh (find and assign each side of the door)
	//If interact ray is detected, use the hit location to decide what animation to run. otherwise always open using RightHinge

	public override void _Ready()
	{
		//anim = GetNode<AnimationPlayer>("AnimationPlayer");
		GD.Print(isLocked);
		doorForward = rootPoint.Transform.Basis.Z.Normalized();

		states = new Dictionary<DoorStates, DoorState>()
		{
			{DoorStates.Closed, new DoorClosedState(this)},
			{DoorStates.OpenIn, new DoorOpenInState(this)},
			{DoorStates.OpenOut, new DoorOpenOutState(this)}
		};
		ChangeState(DoorStates.Closed);
	}

	
	public void OnKicked(
		IKickable hitNode,
		Vector3 hitPoint,
		Vector3 hitNormal,
		Vector3 interactHitPoint,
		Node3D kicker
	)
	{
		
		currentState?.OnKicked(
			hitNode,
			hitPoint,
			hitNormal,
			interactHitPoint,
			kicker
		);
		
		if(kicker == null)
		{
			GD.PrintErr("Kicker is unrecognizable!");
		}
		Vector3 doorToPlayer = (kicker.GlobalTransform.Origin - rootPoint.GlobalTransform.Origin).Normalized();
		float dot = doorForward.Dot(doorToPlayer);
		GD.Print(dot);
		OpenInOut(dot);
	}

	public void ChangeState(DoorStates newState)
	{
		currentState?.Exit();
		currentState = states[newState];
		currentState.Enter();
	}

	public void OpenInOut(float dot)
	{
		if(dot>0)
		{
			ChangeState(DoorStates.OpenIn);
		}
		else if (dot < 0)
		{
			ChangeState(DoorStates.OpenOut);
		}
		else
		{
			GD.Print("???");
		}
	}

	public void OpenInFunc()
	{
		GD.Print(RightHinge.Rotation.Y);
		RightHinge.RotationDegrees += new Vector3(0, -90f, 0);
		GD.Print(RightHinge.Rotation.Y);
	}
	public void OpenOutFunc()
	{
		GD.Print(RightHinge.Rotation.Y);
		RightHinge.RotationDegrees -= new Vector3(0, 90f, 0);
		GD.Print(RightHinge.Rotation.Y);
	}
}
