using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class SplineGizmos : MonoBehaviour 
{
	[HideInInspector][SerializeField]
	GameObject addEndCurveDetector;
	[HideInInspector][SerializeField]
	GameObject addStartCurveDetector;
	[HideInInspector][SerializeField]
	GameObject removeStartCurveDetector;
	[HideInInspector][SerializeField]
	GameObject removeEndCurveDetector;

	[HideInInspector][SerializeField]
	private Spline _spline;
	Spline spline
	{
		get
		{
			if (_spline == null)
				_spline = GetComponent<Spline>();

			return _spline;
		}

		set
		{
			_spline = value;
		}
	}

	Vector3 addStartGizmoPosition;
	Vector3 addEndGizmoPosition;
	Vector3 removeStartGizmoPosition;
	Vector3 removeEndGizmoPosition;
	float gizmoPlusDim = 0.2f;

	void Awake()
	{
		if (addEndCurveDetector == null)
			addEndCurveDetector = CreateGizmoDetector("Add End Curve Detector", new Vector3(6, 2, 2));

		if (addStartCurveDetector == null)
			addStartCurveDetector = CreateGizmoDetector("Add Start Curve Detector", new Vector3(6, 2, 2));

		if (removeStartCurveDetector == null)
			removeStartCurveDetector = CreateGizmoDetector("Remove Start Curve Detector", new Vector3(6, 2, 2));

		if (removeEndCurveDetector == null)
			removeEndCurveDetector = CreateGizmoDetector("Remove Start Curve Detector", new Vector3(6, 2, 2));
	}
	
	GameObject CreateGizmoDetector(string name, Vector3 size)
	{
		GameObject detector = new GameObject();
		detector.hideFlags = HideFlags.HideInHierarchy;
		detector.name = name;
		detector.transform.SetParent(transform);
		BoxCollider box = detector.AddComponent<BoxCollider>();
		box.size = size * gizmoPlusDim;
		box.isTrigger = true;

		return detector;
	}

	void OnDrawGizmos()
	{
		Vector3 offset = new Vector3(0, 0.8f, 0);

		addEndGizmoPosition = spline.EndPosition() + spline.EndDirection() * 2 + offset;
		addStartGizmoPosition = spline.StartPosition() + spline.StartDirection() * 2 + offset;
		removeEndGizmoPosition = spline.EndPosition() + spline.EndDirection() * 2 - offset;
		removeStartGizmoPosition = spline.StartPosition() + spline.StartDirection() * 2 - offset;

		// Update collision box positions
		addEndCurveDetector.transform.position = addEndGizmoPosition;
		addStartCurveDetector.transform.position = addStartGizmoPosition;
		removeEndCurveDetector.transform.position = removeEndGizmoPosition;
		removeStartCurveDetector.transform.position = removeStartGizmoPosition;

		Gizmos.color = UTIL.GIZMO_LINE;
		
		// Gizmo : Add curve to start
		DrawGizmoPlus(addStartGizmoPosition);
		
		// Gizmo : Add curve to end
		DrawGizmoPlus(addEndGizmoPosition);

		// Gizmo : Remove curve from start
		DrawGizmoMinus(removeStartGizmoPosition);

		// Gizmo : Remove curve from end
		DrawGizmoMinus(removeEndGizmoPosition);
	}

	void DrawGizmoPlus(Vector3 center)
	{
		float startX = -3 * gizmoPlusDim;
		float mid1X = -gizmoPlusDim;
		float mid2X = gizmoPlusDim;
		float endX = 3 * gizmoPlusDim;

		float topY = 3 * gizmoPlusDim;
		float upperY = gizmoPlusDim;
		float lowerY = -gizmoPlusDim;
		float bottomY = -3 * gizmoPlusDim;

		float z = 0;

		Vector3 upperStart = new Vector3(startX, upperY, z) + center;
		Vector3 lowerStart = new Vector3(startX, lowerY, z) + center;
		Vector3 upperMid1 = new Vector3(mid1X, upperY, z) + center;
		Vector3 upperMid2 = new Vector3(mid2X, upperY, z) + center;
		Vector3 upperEnd = new Vector3(endX, upperY, z) + center;
		Vector3 lowerMid1 = new Vector3(mid1X, lowerY, z) + center;
		Vector3 lowerMid2 = new Vector3(mid2X, lowerY, z) + center;
		Vector3 lowerEnd = new Vector3(endX, lowerY, z) + center;
		Vector3 topMid1 = new Vector3(mid1X, topY, z) + center;
		Vector3 topMid2 = new Vector3(mid2X, topY, z) + center;
		Vector3 bottomMid1 = new Vector3(mid1X, bottomY, z) + center;
		Vector3 bottomMid2 = new Vector3(mid2X, bottomY, z) + center;

		Gizmos.DrawLine(lowerStart, upperStart);
		Gizmos.DrawLine(upperStart, upperMid1);
		Gizmos.DrawLine(lowerStart, lowerMid1);
		Gizmos.DrawLine(upperMid1, topMid1);
		Gizmos.DrawLine(lowerMid1, bottomMid1);
		Gizmos.DrawLine(topMid1, topMid2);
		Gizmos.DrawLine(bottomMid1, bottomMid2);
		Gizmos.DrawLine(upperMid2, topMid2);
		Gizmos.DrawLine(lowerMid2, bottomMid2);
		Gizmos.DrawLine(upperMid2, upperEnd);
		Gizmos.DrawLine(lowerMid2, lowerEnd);
		Gizmos.DrawLine(lowerEnd, upperEnd);
	}

	void DrawGizmoMinus(Vector3 center)
	{
		float startX = -3 * gizmoPlusDim;
		float endX = 3 * gizmoPlusDim;
		float upperY = gizmoPlusDim;
		float lowerY = -gizmoPlusDim;

		float z = 0;

		Vector3 upperStart = new Vector3(startX, upperY, z) + center;
		Vector3 lowerStart = new Vector3(startX, lowerY, z) + center;
		Vector3 upperEnd = new Vector3(endX, upperY, z) + center;
		Vector3 lowerEnd = new Vector3(endX, lowerY, z) + center;
		
		Gizmos.DrawLine(lowerStart, upperStart);
		Gizmos.DrawLine(lowerStart, lowerEnd);
		Gizmos.DrawLine(upperStart, upperEnd);
		Gizmos.DrawLine(lowerEnd, upperEnd);
	}

	public void RaycastGizmos()
	{
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitData = new RaycastHit();
		
		if (addEndCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			spline.AddPoint(false);
		}
		else if (addStartCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			spline.AddPoint(true);
		}
		else if (removeEndCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			spline.RemovePoint(false);
		}
		else if (removeStartCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			spline.RemovePoint(true);
		}
	}
}
