using Godot;
using System;

public class DoorClosedState : DoorState
{
    public bool closed = true;
    public DoorClosedState(DoorAnimatorUnlocked door) : base(door) 
    {
        this.door = door;
    }
    public Node3D rootNode;
    public Node3D bottomHingeNode;
    public Node3D wrongOpenHingeNode;
    public Node3D rightOpenHingeNode;
    public Node3D topOpenHingNode;

    Vector3 doorToPlayer;
    Vector3 doorForward;
    float dot;

    public override void Enter()
    {
        GD.Print("Closed bool = " + closed);
        rootNode = door.GetNode<Node3D>("RootEmpty");
        bottomHingeNode = rootNode.GetNode<Node3D>("BottomOpenHinge");
        wrongOpenHingeNode = bottomHingeNode.GetNode<Node3D>("WrongOpenHinge");
        rightOpenHingeNode = wrongOpenHingeNode.GetNode<Node3D>("RightOpenHinge");
        topOpenHingNode = rightOpenHingeNode.GetNode<Node3D>("TopOpenHinge");
        
		doorForward = rootNode.Transform.Basis.Z.Normalized();

        var rot = rootNode.Rotation;
        bottomHingeNode.Rotation = rot;
        wrongOpenHingeNode.Rotation = rot;
        rightOpenHingeNode.Rotation = rot;
        topOpenHingNode.Rotation = rot;

        GD.Print("Door entered Closed state");
        PrintChildrenRecursive(door);
        GD.Print("Root Node. " + rootNode);
        closed = !closed;
        GD.Print("closed " + closed);
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
        doorToPlayer = (kicker.GlobalTransform.Origin - rootNode.GlobalTransform.Origin).Normalized();
        GD.Print("HEY" + doorToPlayer);
        dot = doorForward.Dot(doorToPlayer);
        GD.Print("Door Closed State dot = " + dot);
        if (closed = true)
        {
            door.OpenInOut(dot);
        }
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