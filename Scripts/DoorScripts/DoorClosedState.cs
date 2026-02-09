using Godot;
using System;

public class DoorClosedState : DoorState
{

    public DoorClosedState(DoorAnimatorUnlocked door) : base(door) {}
    public Node3D rootNode;

    public override void Enter()
    {
        rootNode = door.GetNode<Node3D>("RootEmpty");
        GD.Print("Door entered Closed state");
        PrintChildrenRecursive(door);
        GD.Print("Root Node. " + rootNode);
    }
    public override void Exit() {}
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
        //door.ChangeState(DoorAnimatorUnlocked.DoorStates.OpenIn);
    }

    public static void PrintChildrenRecursive(Node node, string indent = "")
{
    foreach (Node child in node.GetChildren())
    {
        GD.Print($"{indent}{child.Name}");
        PrintChildrenRecursive(child, indent + "  ");
    }
}
}