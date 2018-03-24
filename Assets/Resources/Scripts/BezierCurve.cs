using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] 
[System.Serializable]
public class BezierCurve : MonoBehaviour 
{
	public Transform p0;
	public Transform p1;
	public Transform p2;
	public Transform p3;

	[SerializeField]
	BezierCurve next;
	[SerializeField]
	BezierCurve prev;

	[SerializeField]
	private Transform _guideLine;
	Transform guideLine
	{
		get
		{
			if (_guideLine == null)
			{
				_guideLine = (new GameObject()).transform;
				_guideLine.SetParent(transform);
				_guideLine.hideFlags = HideFlags.HideInHierarchy;
			}

			return _guideLine;
		}

		set
		{
			_guideLine = value;
		}
	}

	[SerializeField]
	private List<GuideLine> _guideLines;
	List<GuideLine> guideLines
	{
		get
		{
			if (_guideLines == null)
				_guideLines = new List<GuideLine>();

			return _guideLines;
		}
		set
		{
			_guideLines = value;
		}
	}

	[SerializeField]
	float curveLength;

	public void Init(int numSubDivisions, Vector3 origin, bool atStart, Transform parent)
	{
		transform.SetParent(parent);
		gameObject.tag = "BezierCurve";
	
		if (atStart)
		{
			p0.position = origin - Vector3.left * 3;
			p1.position = origin - Vector3.left * 2;
			p2.position = origin - Vector3.left * 1;
			p3.position = origin;
		}
		else
		{
			p0.position = origin;
			p1.position = origin + Vector3.left * 1;
			p2.position = origin + Vector3.left * 2;
			p3.position = origin + Vector3.left * 3;
		}

		UpdateNumberSubDivisions(numSubDivisions);
	}

	public void Init(int divisions, Vector3 _p0, Vector3 _p3, Transform parent)
	{
		transform.SetParent(parent);
		gameObject.tag = "BezierCurve";

		// TODO: Solution for setting tangent points here
		Vector3 dir = (_p3 - _p0).normalized;

		p0.position = _p0;
		p1.position = _p0 + dir * 2;
		p3.position = _p3;
		p2.position = _p3 - dir * 2;

		UpdateNumberSubDivisions(divisions);		
	}

	public void Init(int divisions, Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, Transform parent)
	{
		transform.SetParent(parent);
		gameObject.tag = "BezierCurve";

		// TODO: Solution for setting tangent points here
		p0.position = _p0;
		p1.position = _p1;
		p2.position = _p2;
		p3.position = _p3;

		UpdateNumberSubDivisions(divisions);		
	}

	public void SetNext(BezierCurve _next)
	{
		next = _next;
		if (next != null)
			p3.position = _next.GetStartPosition();
	}

	public void SetPrev(BezierCurve _prev)
	{
		prev = _prev;
		if (prev != null)
		{
			// Link p0 control point to p3 of neighbor
			p0.position = _prev.GetEndPosition();
		}

	}

	public void UpdateControlPoints(ControlPoint point)
	{
		DrawGuideLine();
		
		if (point.transform == p3 && next != null)
		{
			//Tell next curve to update p0 to match p3
			next.UpdateStartPoint(p3.position);
		}
		else if (point.transform == p0 && prev != null)
		{
			prev.UpdateEndPoint(p0.position);
		}

		UpdateCurveLength();
		GetComponentInParent<Spline>().UpdateSplineLength();
	}

	public void UpdateNumberSubDivisions(int num)
	{
		if (num != guideLines.Count)
		{
			while (num < guideLines.Count)
			{
				GuideLine line = guideLines[0];
				guideLines.Remove(line);
				GameObject.DestroyImmediate(line.gameObject);
			}

			while (num > guideLines.Count)
			{
				guideLines.Add(CreateGuideLineSegment());
			}

			DrawGuideLine();
		}

		UpdateCurveLength();
	}

	void UpdateCurveLength()
	{
		curveLength = 0;

		foreach(var line in guideLines)
		{
			curveLength += line.GetDistance();
		}
	}

	void UpdateStartPoint(Vector3 position)
	{
		p0.position = position;

		UpdateCurveLength();
		GetComponentInParent<Spline>().UpdateSplineLength();
	}

	void UpdateEndPoint(Vector3 position)
	{
		p3.position = position;

		UpdateCurveLength();
		GetComponentInParent<Spline>().UpdateSplineLength();
	}

	void OnDestroy()
	{
		Spline spline = transform.parent.GetComponent<Spline>();
		spline.CurveDestroyed(this);
	}

	void DrawGuideLine()
	{
		float step = 1f/ guideLines.Count;

		for (int i = 0; i < guideLines.Count; i++)
		{
			GuideLine line = guideLines[i];
			line.SetPositions(GetPoint(step * i), GetPoint(step * (i + 1)));
		}
	}

	public void GuidelineSelected()
	{
		Selection.objects = new GameObject[4]{p0.gameObject, p1.gameObject, p2.gameObject, p3.gameObject};
		foreach (GuideLine g in _guideLines)
			g.SetSelected(true);
	}

	public void UpdateGizmos()
	{
		bool flag = false;
		GameObject[] cps = new GameObject[4]{p0.gameObject, p1.gameObject, p2.gameObject, p3.gameObject};
		foreach (GameObject cp in cps)
		{
			if (!Selection.Contains(cp))
			{
				flag = true;
				break;
			}
		}

		if (flag)
		{
			foreach (GuideLine g in _guideLines)
			g.SetSelected(false);	
		}
		
	}
	public void ControlPointDeselected()
	{
		// If entire segment was highlighted, this will clear it
		foreach (GuideLine g in _guideLines)
			g.SetSelected(false);
	}

	GuideLine CreateGuideLineSegment()
	{
		GameObject segment = new GameObject();

		segment.transform.SetParent(guideLine);	
		
		GuideLine line = segment.AddComponent<GuideLine>();

		return line;
	}

	// -----------------------------------------------------------------------------------

	public float GetLength()
	{
		return curveLength;
	}

	public SplineData GetData(float d)
	{
		float alpha = d / curveLength;

		SplineData data = new SplineData();
		data.Position = GetPoint(alpha);
		data.Tangent = GetTangent(alpha);
		data.Normal = GetNormal(alpha, Vector3.up);
		
		return data;
	}

	public Vector3 GetEndPosition()
	{
		return p3.position;
	}

	public Vector3 GetEndTangent()
	{
		return GetTangent(1);
	}

	public Vector3 GetEndOrientation()
	{
		return GetOrientation(1, Vector3.up).eulerAngles;
	}

	public Vector3 GetStartOrientation()
	{
		return GetOrientation(0, Vector3.up).eulerAngles;
	}

	public Vector3 GetStartPosition()
	{
		return p0.position;
	}
	
	public Vector3 GetStartTangent()
	{
		return GetTangent(0);
	}

	public Vector3 GetStartNormal()
	{
		return GetNormal(0, Vector3.up);
	}
	
	public Vector3 GetEndNormal()
	{
		return GetNormal(1, Vector3.up);
	}

	// -----------------------------------------------------------------------------------
	Vector3 GetPoint(float t)
	{
		float alpha = 1f - t;
		float alpha_2 = alpha * alpha;
		float t_2 = t * t;

		return p0.position * (alpha_2 * alpha) +
			   p1.position * (3f * alpha_2 * t) +
			   p2.position * (3f * alpha * t_2) +
			   p3.position * (t_2 * t);
	}

	Vector3 GetTangent(float t)
	{
		float alpha = 1f - t;
		float alpha_2 = alpha * alpha;
		float t_2 = t * t;

		Vector3 tangent = p0.position * (-alpha_2) +
						  p1.position * (3f * alpha_2 - 2 * alpha) +
						  p2.position * (-3f * t_2 + 2 * t) +
						  p3.position * (t_2);

		return tangent.normalized;
	}

	Vector3 GetNormal(float t, Vector3 up)
	{
		Vector3 tangent = GetTangent(t);
		Vector3 binormal = Vector3.Cross(up, tangent).normalized;

		return Vector3.Cross(tangent, binormal);
	}

	Quaternion GetOrientation(float t, Vector3 up)
	{
		Vector3 tangent = GetTangent(t);
		Vector3 normal = GetNormal(t, up);

		return  Quaternion.LookRotation(tangent, normal);
	}
}
