using Godot;
using System;

namespace PlayerFuncs
{
    public static class PlayerFuncs
    {
                public static T FindParentWith<T>(Node start) where T : class
        {
            Node current = start;

            while (current != null)
            {
                if (current is T match)
                    return match;

                current = current.GetParent();
            }

            return null;
        }
        public static bool GetClosestHit(
            ShapeCast3D shapeCast3D,
            out Vector3 hitPoint,
            out Vector3 hitNorm,
            out RigidBody3D hitBody
        )
        {
            hitPoint = Vector3.Zero;
            hitNorm = Vector3.Zero;
            hitBody = null;

            if (shapeCast3D == null || !shapeCast3D.IsColliding())
                return false;
            
            float closestDistSq = float.MaxValue;
            Vector3 origin = shapeCast3D.GlobalTransform.Origin;

            int count = shapeCast3D.GetCollisionCount();
            for (int i = 0; i < count; i++)
            {
                Vector3 point = shapeCast3D.GetCollisionPoint(i);
                float distSq = origin.DistanceSquaredTo(point);

                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    hitPoint = point;
                    hitBody = shapeCast3D.GetCollider(i) as RigidBody3D;
                    hitNorm = shapeCast3D.GetCollisionNormal(i);
                }
            }
            return true;
        }

        
        public static void ProcessKickHits(
            ShapeCast3D shapeCast,
            Node3D kicker,
            RayCast3D interactRay
        )
        {
            GD.Print("Kick occured");
            if (shapeCast == null || interactRay == null)
                return;

            shapeCast.ForceShapecastUpdate();
            Vector3 intHitPoint = interactRay.GetCollisionPoint();

            int count = shapeCast.GetCollisionCount();

            for (int i = 0; i < count; i++)
            {
                Node collider = shapeCast.GetCollider(i) as Node;
                var obj = FindParentWith<IKickable>(collider);
                if (obj != null)
                {
                    obj.OnKicked(
                        obj,
                        shapeCast.GetCollisionPoint(i),
                        shapeCast.GetCollisionNormal(i),
                        intHitPoint,
                        kicker
                    );
                }
            }
        }
    }
}
