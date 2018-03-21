using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineAnimation : MonoBehaviour 
{
	public float Speed = 0.25f;
	public bool OrientToPath = true;

	public Spline Spline;
	float time;

	void Start() 
	{
		time = 0;	
	}
	
	void Update() 
	{
		time += Time.deltaTime * Speed;

		if (time > 1)
			time = 0;

		SplineData data = Spline.NextDataPoint(time);

		transform.position = data.Position;
		
		if (OrientToPath)
		{
			// For some reason calling SetLookRotation directly on transform.localRotation doesn't update anything
			Quaternion rot = Quaternion.LookRotation(data.Tangent, data.Normal);
			transform.localRotation = rot;
		}
		
	}
}
