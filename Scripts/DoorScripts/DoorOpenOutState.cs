using Godot;
using System;

public class DoorOpenOutState : DoorState
{
    DoorAnimatorUnlocked door;

    public DoorOpenOutState(DoorAnimatorUnlocked door) : base(door) {}

    public override void Enter()
    {
        GD.Print("Door entered OpenOut state");
    }
    public virtual void Exit() {}
    public override void Update(double delta) 
    {
        //transition to open state
    }
    public virtual void OnKicked(
		IKickable hitNode,
		Vector3 hitPoint,
		Vector3 hitNormal,
        Vector3 interactHitPoint,
		Node3D kicker)
    {
        door.OpenOutFunc();
    }
}
