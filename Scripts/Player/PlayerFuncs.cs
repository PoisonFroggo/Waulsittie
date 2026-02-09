using Godot;
using System;

namespace PlayerFuncs
{
	//Helper functions for the player
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
		//This function should get only normals that are the correct amount up. If there are no normals detected that are facing up enough, the player slides down the slope.
		//By comparing a maximum angle converted into a cosine (minDot) with the normals which are themselves added to the current pitch and roll of the player. This allows
		//for the programmer to rotate the player so that a once flat surface is now a steep slope, causing the player to slide down the slope.
		//Replace GetClosestHit with this
		/*public static bool GetValidHits(
			ShapeCast3D shapeCast3D,
			CollisionShape3D playerCol,
			out Vector3 hitPoint,
			out Vector3 hitNorm,
			out RigidBody3D hitBody
		)
		{
			hitPoint = Vector3.Zero;
			hitNorm = Vector3.Zero;
			hitBody = null;
			float maxAngleDeg = 85;
			float minDot = Mathf.Cos(Mathf.DegToRad(maxAngleDeg)); //Gets cosine of the maxAngle, compare this with the detected normals. If detected normals are too high, then they are disqualified from collision calculations.
			//Vector3 playerRot = new Vector3(playerCol.Rotation.X, 0f, playerCol.Rotation.Z); //The current pitch/roll rotation of the player
			if (shapeCast3D == null || !shapeCast3D.IsColliding())
				return false;
			
			int count = shapeCast3D.GetCollisionCount();
			for(int i =0; i < count; i++) //for loop validates each collision point, checking if it is valid and adding it to a list. These will then be compared to each other and the one closest to the point of collision will be chosen as the 'main' collision point.
			{
				
			}
			return true;
		}*/
		public static bool GetClosestHit(
			ShapeCast3D shapeCast3D,
			out Vector3 hitPoint,
			out Vector3 hitNorm,
			out RigidBody3D hitBody
		)
		{
			CollisionShape3D playerCol = shapeCast3D.GetParent<CollisionShape3D>();
			hitPoint = Vector3.Zero;
			hitNorm = Vector3.Zero;
			hitBody = null;

			if (shapeCast3D == null || !shapeCast3D.IsColliding())
				return false;
			
			float closestDistSq = float.MaxValue;
			Vector3 origin = playerCol.GlobalTransform.Origin;

			int count = shapeCast3D.GetCollisionCount();
			for (int i = 0; i < count; i++)
			{
				Vector3 point = shapeCast3D.GetCollisionPoint(i);
				//var cx = shapeCast3D.GetCollider(i);
				//GD.Print("Hit " + ((Node)cx).Name);
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
