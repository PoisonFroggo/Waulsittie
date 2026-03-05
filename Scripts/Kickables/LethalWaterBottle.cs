using Godot;
using System;

public partial class LethalWaterBottle : Node3D, IKickable
{
	public override void _Ready()
	{
	}

	
	public void OnKicked(
		IKickable hitNode,
		Vector3 hitPoint,
		Vector3 hitNormal,
		Vector3 interactHitPoint,
		Node3D kicker
	)
		{
			GD.Print("Bottle was kicked");
		}
}
