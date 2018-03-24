using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class SplineGizmos : MonoBehaviour 
{
	#region Constants

	private Vector3 colliderSizeMinus = new Vector3(1.65f, 0.8f, 0.75f);
	private Vector3 colliderSizePlus = new Vector3(1.65f, 1.65f, 0.75f);

	Vector3 gizmoSizeX = new Vector3(1.5f, 0.5f, 0.25f);
	Vector3 gizmoSizeY = new Vector3(0.5f, 1.5f, 0.25f);
	Vector3 gizmoOffset = new Vector3(0, 0.8f, 0);

	#endregion

	[SerializeField][HideInInspector]
	private BoxCollider _addEndGizmo;
	BoxCollider addEndGizmo
	{
		get
		{
			if (_addEndGizmo == null)
				_addEndGizmo = CreateGizmoDetector(colliderSizePlus);
			
			return _addEndGizmo;
		}
	}

	[SerializeField][HideInInspector]
	private BoxCollider _addStartGizmo;
	BoxCollider addStartGizmo
	{
		get
		{
			if (_addStartGizmo == null)
				_addStartGizmo = CreateGizmoDetector(colliderSizePlus);
			
			return _addStartGizmo;
		}
	}

	[SerializeField][HideInInspector]
	private BoxCollider _removeEndGizmo;
	BoxCollider removeEndGizmo
	{
		get
		{
			if (_removeEndGizmo == null)
				_removeEndGizmo = CreateGizmoDetector(colliderSizeMinus);
			
			return _removeEndGizmo;
		}
	}

	[SerializeField][HideInInspector]
	private BoxCollider _removeStartGizmo;
	BoxCollider removeStartGizmo
	{
		get
		{
			if (_removeStartGizmo == null)
				_removeStartGizmo = CreateGizmoDetector(colliderSizeMinus);
			
			return _removeStartGizmo;
		}
	}

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

	BoxCollider CreateGizmoDetector(Vector3 size)
	{
		GameObject detector = new GameObject();

		detector.hideFlags = HideFlags.HideInHierarchy;
		detector.transform.SetParent(transform);
		
		BoxCollider box = detector.AddComponent<BoxCollider>();
		box.size = size;
		box.isTrigger = true;

		return box;
	}

	void OnDrawGizmos()
	{
		UpdateGizmoPositions();

		Gizmos.color = UTIL.GIZMO_LINE;
		
		DrawGizmoPlus(addStartGizmoPosition);
		DrawGizmoPlus(addEndGizmoPosition);
		DrawGizmoMinus(removeStartGizmoPosition);
		DrawGizmoMinus(removeEndGizmoPosition);
	}

	void OnDrawGizmosSelected()
	{
		RaycastGizmos();
	}

	void UpdateGizmoPositions()
	{
		Vector3 end = spline.EndPosition();
		Vector3 start = spline.StartPosition();
		Vector3 endDir = spline.EndDirection();
		Vector3 startDir = spline.StartDirection();

		// Update Gizmo positions
		addEndGizmoPosition = end + endDir * 2 + gizmoOffset;
		addStartGizmoPosition = start + startDir * 2 + gizmoOffset;
		removeEndGizmoPosition = end + endDir * 2 - gizmoOffset;
		removeStartGizmoPosition = start + startDir * 2 - gizmoOffset;

		// Update collision box positions
		addEndGizmo.transform.position = addEndGizmoPosition;
		addStartGizmo.transform.position = addStartGizmoPosition;
		removeEndGizmo.transform.position = removeEndGizmoPosition;
		removeStartGizmo.transform.position = removeStartGizmoPosition;
	}

	void DrawGizmoPlus(Vector3 center)
	{
		Gizmos.DrawCube(center, gizmoSizeX);
		Gizmos.DrawCube(center, gizmoSizeY);
	}

	void DrawGizmoMinus(Vector3 center)
	{		
		Gizmos.DrawCube(center, gizmoSizeX);
	}

	// Check to see which/if user clicked on a Gizmo - call appropriate function.
	void RaycastGizmos()
	{
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hitData = new RaycastHit();
		
		if (addEndGizmo.Raycast(ray, out hitData, 200))
		{
			spline.AddPoint(false);
		}
		else if (addStartGizmo.Raycast(ray, out hitData, 200))
		{
			spline.AddPoint(true);
		}
		else if (removeEndGizmo.Raycast(ray, out hitData, 200))
		{
			spline.RemovePoint(false);
		}
		else if (removeStartGizmo.Raycast(ray, out hitData, 200))
		{
			spline.RemovePoint(true);
		}
	}
}
