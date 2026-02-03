using Godot;
using System;

public interface IKickable
{
    void OnKicked(
        IKickable hitNode,
        Vector3 hitPoint,
        Vector3 hitNormal,
        Vector3 rayHitPoint,
        Node3D kicker
    );
}