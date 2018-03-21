using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class GuideLine : MonoBehaviour 
{
	[SerializeField]
	Vector3 start;	
	[SerializeField]
	Vector3 end;

	BezierCurve parent;
	bool isSelected;

	void SetParent()
	{
		parent = transform.parent.parent.GetComponent<BezierCurve>();
	}

	public void SetPositions(Vector3 s, Vector3 e)
	{
		start = s;
		end = e;
	}

	void OnDrawGizmos()
	{
		if (isSelected)
			Gizmos.color = UTIL.GIZMO_HIGHLIGHT_LINE;
		else
			Gizmos.color = UTIL.GIZMO_LINE;
		
		float epsilon = 0.01f;
		Vector3 deltaUp = new Vector3(0, epsilon, 0);
		Vector3 deltaDown = new Vector3(0, -epsilon, 0);
		Gizmos.DrawLine(start, end);
		Gizmos.DrawLine(start + deltaUp, end + deltaUp);
		Gizmos.DrawLine(start + deltaDown, end + deltaDown);
	}

	void OnDrawGizmosSelected()
	{
		if (parent == null)
			SetParent();

		// Only send message if GuideLine was selected directly, not as part of the heirarchy
		if (Selection.objects.Length == 1 && (Selection.objects[0] as GameObject) == gameObject)
		{
			parent.GuidelineSelected();
		}
	}

	public Vector3 GetStart()
	{
		return start;
	}

	public Vector3 GetEnd()
	{
		return end;
	}

	public void SetSelected(bool flag)
	{
		isSelected = flag;
	}
}
