using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPointDemo : MonoBehaviour 
{
	bool isMoving;
	Vector3 mouseStart;
	Vector3 zOffset;

	BezierCurveDemo bezierCurve;

	void Start()
	{
		bezierCurve = transform.parent.GetComponent<BezierCurveDemo>();
	}

	void OnMouseDown()
	{
		isMoving = true;
		zOffset = new Vector3(0, 0, transform.position.z - Camera.main.transform.position.z);
		mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition + zOffset);
	}

	void OnMouseUp()
	{
		isMoving = false;
	}

	void Update()
	{
		if (isMoving)
		{
			Vector3 mouseCurrent = Camera.main.ScreenToWorldPoint(Input.mousePosition + zOffset);
			Vector3 mouseDelta = mouseCurrent - mouseStart;
			transform.position = transform.position + new Vector3(mouseDelta.x, mouseDelta.y, 0);
			mouseStart = mouseCurrent;

			bezierCurve.UpdateLines();
		}
	}
}
