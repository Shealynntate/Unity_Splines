using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class Spline : MonoBehaviour 
{
#region Public Properties

	public GameObject BezierCurvePrefab;

#endregion

	[SerializeField]
	GameObject addEndCurveDetector;
	[SerializeField]
	GameObject addStartCurveDetector;
	[SerializeField]
	GameObject removeStartCurveDetector;
	[SerializeField]
	GameObject removeEndCurveDetector;

	Vector3 addStartGizmoPosition;
	Vector3 addEndGizmoPosition;
	Vector3 removeStartGizmoPosition;
	Vector3 removeEndGizmoPosition;
	float gizmoPlusDim = 0.2f;
	
	[SerializeField]
	List<BezierCurve> curves;

	[Space(10)]
	[Range(1, 100)]public int NumSubDivisions = 10;

	int _numSubDivisions;

	void Awake()
	{
		if (curves == null)
		{
			curves = new List<BezierCurve>();
		}

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

#region Public API

	// Called when number of subdivisions is adjusted in editor window
	public void UpdateNumberSubDivisions() 
	{
		if (_numSubDivisions != NumSubDivisions)
		{
			foreach (BezierCurve curve in curves)
				curve.UpdateNumberSubDivisions(NumSubDivisions);

			_numSubDivisions = NumSubDivisions;
		}	
	}
	
	// Called when user clicks GUI Add Gizmo
	public void AddPoint(bool atStart)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			if (atStart)
			{
				BezierCurve next = curves[0];
				Vector3 anchor = next.GetStartPosition();
				curve.Init(NumSubDivisions, anchor, atStart, transform);

				next.SetPrev(curve);
				curve.SetNext(next);
			}
			else
			{
				BezierCurve prev = curves[numCurves - 1];
				Vector3 anchor = prev.GetEndPosition();
				curve.Init(NumSubDivisions, anchor, atStart, transform);

				prev.SetNext(curve);
				curve.SetPrev(prev);
			}
		}
		else
		{
			curve.Init(NumSubDivisions, Vector3.zero, atStart, transform);
		}
		
		if (atStart)
			curves.Insert(0, curve);
		else
			curves.Add(curve);
	}

	public void AddAbsolutePoint(Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
		
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			curve.Init(NumSubDivisions, anchor, end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			curve.Init(NumSubDivisions, transform.position, end, transform);
		}

		curves.Add(curve);
	}

	public void AddAbsolutePoint(Vector3 control1, Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			curve.Init(NumSubDivisions, anchor, control1, control2, end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			Vector3 p = transform.position;
			curve.Init(NumSubDivisions, p, control1, control2, end, transform);
		}
		
		curves.Add(curve);
	}

	// For Smooth Curve command
	public void AddAbsolutePoint(Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			Vector3 control1 = (prev.p2.position - prev.p3.position) * -1 + prev.p3.position;

			curve.Init(NumSubDivisions, anchor, control1, control2, end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			Vector3 p = transform.position;
			curve.Init(NumSubDivisions, p, p, control2, end, transform);
		}
		
		curves.Add(curve);
	}

	public void AddRelativePoint(Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			curve.Init(NumSubDivisions, anchor, anchor + end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			curve.Init(NumSubDivisions, transform.position, transform.position + end, transform);
		}
		
		curves.Add(curve);
	}

	public void AddRelativePoint(Vector3 control1, Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{

			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			curve.Init(NumSubDivisions, anchor, anchor + control1, anchor + control2, anchor + end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			Vector3 p = transform.position;
			curve.Init(NumSubDivisions, p, p + control1, p + control2, p + end, transform);
		}
		
		curves.Add(curve);
	}

	// For Smooth Curve command
	public void AddRelativePoint(Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			Vector3 control1 = (prev.p2.position - prev.p3.position) * -1;

			curve.Init(NumSubDivisions, anchor, anchor + control1, anchor + control2, anchor + end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			Vector3 p = transform.position;
			curve.Init(NumSubDivisions, p, p, p + control2, p + end, transform);
		}

		curves.Add(curve);
	}

	void RemovePoint(bool atStart)
	{
		// If only one curve in spline, don't remove it
		if (curves.Count == 1)
			return;

		// TODO: hide minus gizmos with one curve?
		
		BezierCurve curve;
		if (atStart)
			curve = curves[0];
		else
			curve = curves[curves.Count - 1];

		Object.DestroyImmediate(curve.gameObject);
	}

	public void ClosePath()
	{
		int numCurves = curves.Count;

		if (numCurves == 0)
		{
			Debug.Log("Error: Cannot close empty path.");
			return;
		}

		BezierCurve curve = GameObject.Instantiate(BezierCurvePrefab).GetComponent<BezierCurve>();
		
		BezierCurve startCurve = curves[0];
		BezierCurve endCurve = curves[curves.Count - 1];

		curve.Init(NumSubDivisions, endCurve.GetEndPosition(), startCurve.GetStartPosition(), transform);
		curves.Add(curve);

		endCurve.SetNext(curve);
		curve.SetPrev(endCurve);
	}

	public int Size()
	{
		return curves.Count;
	}

#endregion

	public Vector3 EndPosition()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[count - 1].GetEndPosition();
	}

	Vector3 EndTangent()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[count - 1].GetEndTangent();
	}
	Vector3 EndNormal()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[count - 1].GetEndNormal();
	}

	Vector3 EndDirection()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		Vector3 e = curves[count - 1].GetEndPosition();
		Vector3 s = curves[count - 1].GetStartPosition();

		return (e - s).normalized;
	}

	Vector3 EndOrientation()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[count - 1].GetEndOrientation();
	}

	public Vector3 StartPosition()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[0].GetStartPosition();
	}

	Vector3 StartDirection()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		Vector3 e = curves[0].GetEndPosition();
		Vector3 s = curves[0].GetStartPosition();

		return (s - e).normalized;
	}

	Vector3 StartOrientation()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[0].GetStartOrientation();
	}

	Vector3 StartTanget()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[0].GetStartTangent();
	}

	Vector3 StartNormal()
	{
		int count = curves.Count;
		if (count == 0)
			return Vector3.zero;
		
		return curves[0].GetStartNormal();
	}

	public void CurveDestroyed(BezierCurve curve)
	{
		int index = curves.IndexOf(curve);

		if (index == -1)
		{
			Debug.Log("Tried to remove curve that wasn't there");
			return;
		}
			
		bool first = index == 0;
		bool last = index == curves.Count - 1;
		BezierCurve prev = first ? null : curves[index - 1];
		BezierCurve next = last ? null : curves[index + 1];

		if (!first)
			prev.SetNext(next);
		if (!last)
			next.SetPrev(prev);

		curves.Remove(curve);
	}

	void OnDrawGizmos()
	{
		Vector3 offset = new Vector3(0, 0.8f, 0);

		addEndGizmoPosition = EndPosition() + EndDirection() * 2 + offset;
		addStartGizmoPosition = StartPosition() + StartDirection() * 2 + offset;
		removeEndGizmoPosition = EndPosition() + EndDirection() * 2 - offset;
		removeStartGizmoPosition = StartPosition() + StartDirection() * 2 - offset;

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

	public SplineData NextDataPoint(float t)
	{
		int index = (int)(t * curves.Count);
		float offset = (t * curves.Count) % 1f;
		BezierCurve curve = curves[index];
		
		return curve.GetData(offset);
	}

		// TODO: Remove this for better solution
	void HardReset()
	{
		curves.RemoveRange(0, curves.Count);

		foreach(Transform t in transform)
		{
			if (t.tag == "BezierCurve")
			{
				curves.Add(t.GetComponent<BezierCurve>());
			}
		}
	}

	public void RaycastGizmos()
	{
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitData = new RaycastHit();
		
		if (addEndCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			AddPoint(false);
		}
		else if (addStartCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			AddPoint(true);
		}
		else if (removeEndCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			RemovePoint(false);
		}
		else if (removeStartCurveDetector.GetComponent<BoxCollider>().Raycast(ray, out hitData, 200))
		{
			RemovePoint(true);
		}
	}

}

// ----------------------------------------------------------------------

[System.Serializable]
public struct SplineData
{
	public Vector3 Position;
	public Vector3 Tangent;
	public Vector3 Normal;
}

