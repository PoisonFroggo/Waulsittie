using Godot;
using System;

public class DoorOpenInState : DoorState
{

	public DoorOpenInState(DoorAnimatorUnlocked door) : base(door) 
	{
		this.door = door;
	}

	public override void Enter()
	{
		GD.Print("Door entered OpenIn state");
		door.OpenInFunc();
	}
	public override void Exit()//Do not do implicit state changes through exit, exit is for cleaning up leftover data
	{
	}
	public override void Update(double delta) 
	{
		//transition to open state
	}
	public override void OnKicked(
		IKickable hitNode,
		Vector3 hitPoint,
		Vector3 hitNormal,
		Vector3 interactHitPoint,
		Node3D kicker)
	{
		//door.OpenInFunc();
		GD.Print("OpenIn kick detected");
		door.ChangeState(DoorAnimatorUnlocked.DoorStates.Closed);
	}
}
