using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlPoint))]
public class ControlPointEditor : Editor 
{
	private SerializedProperty _type;
	
	ControlPoint controlPoint;

	public void OnEnable()
	{
		SerializedObject cp = new SerializedObject(target);
		
		_type = cp.FindProperty("Type");
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(_type);
	}

	void OnDisable()
	{
		
	}
}
