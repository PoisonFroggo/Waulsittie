using Godot;
using System;

public abstract class DoorState
{
    protected DoorAnimatorUnlocked door;

    public DoorState(DoorAnimatorUnlocked door)
    {
        this.door = door;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }
    public virtual void OnKicked(
		IKickable hitNode,
		Vector3 hitPoint,
		Vector3 hitNormal,
        Vector3 interactHitPoint,
		Node3D kicker) { }
}
