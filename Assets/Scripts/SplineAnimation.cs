using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineAnimation : MonoBehaviour 
{
	public float Speed = 10.0f;
	public bool OrientToPath = true;
	public Spline Spline;

	double distanceTraveled;

	void Start() 
	{
		distanceTraveled = 0;	
	}
	
	void Update() 
	{
		distanceTraveled += Time.deltaTime * Speed;

		SplineData data = Spline.NextDataPoint(distanceTraveled);

		transform.position = data.Position;
		
		if (OrientToPath)
		{
			// For some reason calling SetLookRotation directly on transform.localRotation doesn't update anything
			Quaternion rot = Quaternion.LookRotation(data.Tangent, data.Normal);
			transform.localRotation = rot;
		}
		
	}
}
