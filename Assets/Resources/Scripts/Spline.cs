using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public class Spline : MonoBehaviour 
{
#region Public Properties

	[SerializeField]
    private GameObject _curvePrefab;
    GameObject curvePrefab
    {
        get
        {
            if (_curvePrefab == null)
                _curvePrefab = (GameObject)Resources.Load("Prefabs/BezierCurve", typeof(GameObject));

            return _curvePrefab;
        }
    }

#endregion

	[Tooltip("The number of subdivisons in each curve. A higher number means smoother curves, but at a performance cost.")]
	[Range(1, 100)]public int NumSubDivisions = 10;

	[SerializeField]
	List<BezierCurve> curves;
	int _numSubDivisions;
	[SerializeField]
	double splineLength;

	void Awake()
	{
		if (curves == null)
			curves = new List<BezierCurve>();
	}

#region Public API

	// Called when number of subdivisions is adjusted in editor window
	public void UpdateNumberSubDivisions() 
	{
		splineLength = 0;

		if (_numSubDivisions != NumSubDivisions)
		{
			foreach (BezierCurve curve in curves)
			{
				curve.UpdateNumberSubDivisions(NumSubDivisions);
				splineLength += curve.GetLength();
			}

			_numSubDivisions = NumSubDivisions;
		}	
	}
	
	// Called when user clicks GUI Add Gizmo
	public void AddPoint(bool atStart)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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

		splineLength += curve.GetLength();
	}

	public void AddAbsoluteSmoothPoint(Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			Vector3 control1 = (prev.p2.position - prev.p3.position) * -1 + prev.p3.position;

			curve.Init(NumSubDivisions, anchor, control1, control1, end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			Vector3 p = transform.position;
			curve.Init(NumSubDivisions, p, end, transform);
		}
		
		curves.Add(curve);
		splineLength += curve.GetLength();
	}

	public void AddAbsolutePoint(Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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
		splineLength += curve.GetLength();
	}

	public void AddAbsolutePoint(Vector3 control1, Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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
		splineLength += curve.GetLength();
	}

	// For Smooth Curve command
	public void AddAbsolutePoint(Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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
		splineLength += curve.GetLength();
	}

	public void AddRelativePoint(Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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
		splineLength += curve.GetLength();
	}

	public void AddRelativeSmoothPoint(Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
		int numCurves = curves.Count;

		if (numCurves > 0)
		{
			BezierCurve prev = curves[numCurves - 1];
			Vector3 anchor = prev.GetEndPosition();
			Vector3 control1 = (prev.p2.position - prev.p3.position) * -1;

			curve.Init(NumSubDivisions, anchor, anchor + control1, anchor + control1, anchor + end, transform);

			prev.SetNext(curve);
			curve.SetPrev(prev);
		}
		else
		{
			Vector3 p = transform.position;
			curve.Init(NumSubDivisions, p, p + end, transform);
		}

		curves.Add(curve);
		splineLength += curve.GetLength();
	}

	public void AddRelativePoint(Vector3 control1, Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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
		splineLength += curve.GetLength();
	}

	// For Smooth Curve command
	public void AddRelativePoint(Vector3 control2, Vector3 end)
	{
		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
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
		splineLength += curve.GetLength();
	}

	public void RemovePoint(bool atStart)
	{
		// If only one curve in spline, don't remove it
		// TODO: hide minus gizmos with one curve
		if (curves.Count == 1)
			return;

		BezierCurve	curve = atStart ? curves[0] : curves[curves.Count - 1];

		splineLength -= curve.GetLength();
		
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

		BezierCurve curve = GameObject.Instantiate(curvePrefab).GetComponent<BezierCurve>();
		
		BezierCurve startCurve = curves[0];
		BezierCurve endCurve = curves[curves.Count - 1];

		curve.Init(NumSubDivisions, endCurve.GetEndPosition(), startCurve.GetStartPosition(), transform);
		curves.Add(curve);

		splineLength += curve.GetLength();

		endCurve.SetNext(curve);
		curve.SetPrev(endCurve);
	}

	public int Size()
	{
		return curves.Count;
	}

#endregion

	// Called by curves when Control Point/Position adjusted
	public void UpdateSplineLength()
	{
		splineLength = 0;

		foreach(var curve in curves)
			splineLength += curve.GetLength();
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

	public SplineData NextDataPoint(double dist)
	{
		float d = (float)(dist % splineLength);
		
		foreach (var curve in curves)
		{
			if (d < curve.GetLength())
				return curve.GetData(d);

			d -= curve.GetLength();
		}

		return curves[0].GetData(0);
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

#region Start and End Accessors

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

	public Vector3 EndDirection()
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

	public Vector3 StartDirection()
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

#endregion

}

// ----------------------------------------------------------------------

[System.Serializable]
public struct SplineData
{
	public Vector3 Position;
	public Vector3 Tangent;
	public Vector3 Normal;
}

