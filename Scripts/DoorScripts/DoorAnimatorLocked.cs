using Godot;
using System;

public partial class DoorAnimatorLocked : Node3D, IKickable
{
	private AnimationPlayer anim;
	private bool isLocked = false;

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		GD.Print(isLocked);
	}

	
	public void OnKicked(
		IKickable hitNode,
		Vector3 hitPoint,
		Vector3 hitNormal,
		Vector3 interactHitPoint,
		Node3D kicker
	)
	{
		GD.Print("Locked door detected");
		if (!anim.IsPlaying() && isLocked == false)
		{
			anim.Play("kick_open");
			isLocked = true;
			GD.Print(isLocked);
		}

	}

}
