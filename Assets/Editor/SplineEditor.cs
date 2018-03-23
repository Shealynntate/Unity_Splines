using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spline))]
public class SplineEditor : Editor
{

	private SerializedObject _spline;
	private SerializedProperty _prefab;
	private SerializedProperty __numSubDivisions;
	private SerializedProperty _numSubDivisions;

	Spline spline;

	public void OnEnable()
	{
		_spline = new SerializedObject(target);

		spline = (Spline)target;

		_prefab = _spline.FindProperty("BezierCurvePrefab");
		_numSubDivisions = _spline.FindProperty("NumSubDivisions");
		__numSubDivisions = _numSubDivisions;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(_prefab);
		EditorGUILayout.PropertyField(_numSubDivisions);
		
		_spline.ApplyModifiedProperties();

		if (SerializedProperty.EqualContents(_numSubDivisions, __numSubDivisions))
		{
			__numSubDivisions = _numSubDivisions;
			Spline spline = (Spline)target;
			spline.UpdateNumberSubDivisions();
		}

		//DrawDefaultInspector();
		//EditorGUILayout.PropertyField(_curves);

		if (GUILayout.Button("Add Point"))
		{
			Spline spline = (Spline)target;
			spline.AddPoint(false);
		}
		
	}

	void OnSceneGUI()
	{
		// Mouse clicked - check for gizmo intersections
		if (Event.current.type == EventType.mouseDown)
		{
			spline.GetComponent<SplineGizmos>().RaycastGizmos();
		}
	}
}
