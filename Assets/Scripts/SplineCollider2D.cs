using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D), typeof(BezierSpline))]
public class SplineCollider2D : MonoBehaviour
{
	public float ColliderWidth = 1;
	public int ColliderQuality = 50;

	// The collider start position is a normalised value going from spline start to spline end
	[Range(0, 1)]
	public float ColliderStartPos = 0.1f;
	[Range(0, 1)]
	public float ColliderEndPos = 0.9f;

	public bool AutomaticallyUpdateCollisionMesh = true;

	private void GenerateCollisionMesh()
	{
		BezierSpline spline = GetComponent<BezierSpline>();
		PolygonCollider2D collider = GetComponent<PolygonCollider2D>();

		float precision = 1f / (float)ColliderQuality;

		List<Vector2> topPoints = new List<Vector2>(1000);
		List<Vector2> bottomPoints = new List<Vector2>(1000);

		for (float i = ColliderStartPos; i <= ColliderEndPos; i += precision)
		{
			Vector3 actualPoint = transform.InverseTransformPoint(spline.GetPoint(i));
			Vector3 pointDirection = spline.GetDirection(i);

			// To get the tangent to the point direction (x, y) => (-y, x)
			topPoints.Add(actualPoint + new Vector3(-pointDirection.y, pointDirection.x) * ColliderWidth);	
			bottomPoints.Add(actualPoint - new Vector3(-pointDirection.y, pointDirection.x) * ColliderWidth);	
		}

		List<Vector2> finalPoints = new List<Vector2>(topPoints.Count + bottomPoints.Count);
		finalPoints.AddRange(topPoints);
		bottomPoints.Reverse();
		finalPoints.AddRange(bottomPoints);

		collider.points = finalPoints.ToArray();
	}

	private void OnDrawGizmosSelected()
	{
		if (AutomaticallyUpdateCollisionMesh)
		{
			GenerateCollisionMesh();
		}
	}
}
