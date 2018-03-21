using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor 
{
	//BezierCurve curve;

	public void OnEnable()
	{
		//curve = (BezierCurve)target;
		// TODO: Can send selected message here if needed
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}

	void OnDisable()
	{
		// TODO: Can send deselected message here if needed
	}
}
