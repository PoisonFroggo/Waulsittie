using Godot;
using System;

public class DoorOpenOutState : DoorState
{
    public DoorOpenOutState(DoorAnimatorUnlocked door) : base(door) {}

    public override void Enter()
    {
        GD.Print("Door entered OpenOut state");
        door.OpenOutFunc();
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
        door.ChangeState(DoorAnimatorUnlocked.DoorStates.Closed);
    }
}
