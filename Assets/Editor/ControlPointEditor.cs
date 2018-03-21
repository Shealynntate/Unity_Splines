using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControlPoint))]
public class ControlPointEditor : Editor 
{
	//ControlPoint ControlPoint;

	public void OnEnable()
	{
		//ControlPoint = (ControlPoint)target;
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}

	void OnDisable()
	{
		
	}
}
