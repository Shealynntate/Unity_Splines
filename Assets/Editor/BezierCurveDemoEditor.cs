using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(BezierCurveDemo))]
public class BezierCurveDemoEditor : Editor 
{
	private SerializedObject _Curve;
	private SerializedProperty _p0;
	private SerializedProperty _p1;
	private SerializedProperty _p2;
	private SerializedProperty _p3;
	private SerializedProperty _t;

	public UnityEvent mouseClick;

	public void OnEnable()
	{
		_Curve = new SerializedObject(target);
		_p0 = _Curve.FindProperty("p0");
		_p1 = _Curve.FindProperty("p1");
		_p2 = _Curve.FindProperty("p2");
		_p3 = _Curve.FindProperty("p3");
		_t = _Curve.FindProperty("t");

		BezierCurveDemo curve = (BezierCurveDemo)target;
		curve.Init();
	}

	public override void OnInspectorGUI()
	{
		_Curve.Update();

		EditorGUILayout.PropertyField(_p0);
		EditorGUILayout.PropertyField(_p1);
		EditorGUILayout.PropertyField(_p2);
		EditorGUILayout.PropertyField(_p3);
		EditorGUILayout.PropertyField(_t);

		_Curve.ApplyModifiedProperties();

		UpdateCurveLines();
	}

	void UpdateCurveLines()
	{
		BezierCurveDemo curve = (BezierCurveDemo)target;
		curve.UpdateLines();
	}
}
