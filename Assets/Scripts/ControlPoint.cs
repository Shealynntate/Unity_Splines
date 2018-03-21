using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum POINT_TYPE { NONE, P0, P1, P2, P3 }

[ExecuteInEditMode]
[System.Serializable]
public class ControlPoint : MonoBehaviour 
{
	public Transform PrimaryControlPoint;

	public POINT_TYPE Type;
	private BezierCurve _bezierCurve;
	BezierCurve bezierCurve
	{
		get
		{
			if (_bezierCurve == null)
				_bezierCurve = transform.parent.GetComponent<BezierCurve>();
			
			return _bezierCurve;
		}
		set
		{
			_bezierCurve = value;
		}
	}

	Vector3 gizmoSize = new Vector3(0.3f, 0.3f, 0.3f);
	float sphereRadius = 0.25f;

	Vector3 origin;

	public void SetType(POINT_TYPE _type)
	{
		Type = _type;
	}

	void OnDrawGizmos()
	{
		// TODO: This updates highlighted line correctly, but seems like over kill for every CP to call ...
		bezierCurve.UpdateGizmos();

		if (PrimaryControlPoint != null)
		{
			Gizmos.color = UTIL.GIZMO_CONTROL_POINT;
			Gizmos.DrawLine(transform.position, PrimaryControlPoint.position);
		}
		if (Type == POINT_TYPE.P0)
		{
			Gizmos.color = UTIL.GIZMO_PASTEL_ORANGE;
			Gizmos.DrawCube(transform.position, gizmoSize);
		}
		else if (Type == POINT_TYPE.P3)
		{
			Gizmos.color = UTIL.GIZMO_PASTEL_ORANGE;
			Gizmos.DrawCube(transform.position, gizmoSize);
		}
		else
		{
			Gizmos.color = UTIL.GIZMO_CONTROL_POINT;
			Gizmos.DrawSphere(transform.position, sphereRadius);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = UTIL.GIZMO_HIGHLIGHT_LINE;
		if (Type == POINT_TYPE.P0 || Type == POINT_TYPE.P3)
			Gizmos.DrawWireCube(transform.position, gizmoSize);
		else
			Gizmos.DrawWireSphere(transform.position, sphereRadius);
	}

	void Update()
	{
		Vector3 position = transform.position;

		if (position != origin)
		{
			origin = position;
			bezierCurve.UpdateControlPoints(this);
		}
	}
}
