using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineGenerator : MonoBehaviour 
{
	Spline _spline;
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

	public Vector3 AbsoluteMoveTo(List<Vector3> vectors, Vector3 currentPoint)
	{
		if (vectors.Count == 0)
		{
			Debug.Log("Error: Bad Number of inputs to 'Absolute Move To' ");
			return currentPoint;
		}

		Vector3 current = vectors[0];
		vectors.RemoveAt(0);

		if (vectors.Count > 0)
			return AbsoluteLineTo(vectors, current);
		
		return current;
	}

	public Vector3 RelativeMoveTo(List<Vector3> vectors, Vector3 currentPoint)
	{
		if (vectors.Count == 0)
		{
			Debug.Log("Error: Bad Number of inputs to 'Relative Move To' ");
			return currentPoint;
		}
		
		Vector3 current = vectors[0] + currentPoint;
		vectors.RemoveAt(0);

		if (vectors.Count > 0)
			return RelativeLineTo(vectors, current);
		
		return current;
	}

	public Vector3 AbsoluteLineTo(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; ++i)
			spline.AddAbsolutePoint(vectors[i]);

		return vectors[vectors.Count - 1];
	}

	public Vector3 RelativeLineTo(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; ++i)
			spline.AddRelativePoint(vectors[i]);
		
		return vectors[vectors.Count - 1] + currentPoint;
	}

	public Vector3 AbsoluteCurve(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; i += 3)
		{
			if (i + 2 >= vectors.Count)
			{
				Debug.Log("Error: Bad number of inputs to Curve command.");
				break;
			}

			spline.AddAbsolutePoint(vectors[i], vectors[i + 1], vectors[i + 2]);
		}

		return vectors[vectors.Count - 1];
	}

	public Vector3 RelativeCurve(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; i += 3)
		{
			if (i + 2 >= vectors.Count)
			{
				Debug.Log("Error: Bad number of inputs to curve command.");
				break;
			}

			spline.AddRelativePoint(vectors[i], vectors[i + 1], vectors[i + 2]);
		}

		return vectors[vectors.Count - 1] + currentPoint;
	}

	public Vector3 AbsoluteSmoothCurve(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; i += 2)
		{
			if (i + 1 >= vectors.Count)
			{
				Debug.Log("Error: Bad number of inputs to Smooth Curve command.");
				break;
			}

			spline.AddAbsolutePoint(vectors[i], vectors[i + 1]);
		}

		return vectors[vectors.Count - 1];
	}

	public Vector3 RelativeSmoothCurve(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; i += 2)
		{
			if (i + 1 >= vectors.Count)
			{
				Debug.Log("Error: Bad number of inputs to smooth curve command.");
				break;
			}

			spline.AddRelativePoint(vectors[i], vectors[i + 1]);
		}

		return vectors[vectors.Count - 1] + currentPoint;
	}

	public Vector3 AbsoluteArc(List<float> inputs, Vector3 currentPoint)
	{
		Vector3 endPoint = currentPoint;
		
		int i = 0;
		while (i < inputs.Count)
		{
			if (i + 6 >= inputs.Count)
			{
				if (i + 5 >= inputs.Count)
				{
					Debug.Log("Error: Bad number of inputs to Arc command.");
					break;
				}
				else
				{
					// TODO: Verify if this is correct - seems to be able to skip x-rotation argument
					endPoint = new Vector3(inputs[i + 4], inputs[i + 5]);
					spline.AddAbsolutePoint(endPoint);

					i += 6;
					continue;
				}
			}

			//float rx = inputs[i];
			//float ry = inputs[i + 1];
			//float theta = inputs[i + 2];
			//bool largeAngleFlag = (int)inputs[i + 3] == 1;
			//bool sweepFlag = (int)inputs[i + 4] == 1;

			endPoint = new Vector3(inputs[i + 5], inputs[i + 6]);

			spline.AddAbsolutePoint(endPoint);
			i += 7;
		}

		return endPoint;
	}

	public Vector3 RelativeArc(List<float> inputs, Vector3 currentPoint)
	{
		Vector3 endPoint = currentPoint;

		int i = 0;
		while (i < inputs.Count)
		{
			if (i + 6 >= inputs.Count)
			{
				if (i + 5 >= inputs.Count)
				{
					Debug.Log("Error: Bad number of inputs to Arc command.");
					break;
				}
				else
				{
					// TODO: Verify if this is correct - seems to be able to skip x-rotation argument
					endPoint = new Vector3(inputs[i + 4], inputs[i + 5]) + endPoint;
					spline.AddRelativePoint(endPoint);

					i += 6;
					continue;
				}
			}

			// float rx = inputs[i];
			// float ry = inputs[i + 1];
			// float theta = inputs[i + 2];
			// bool largeAngleFlag = (int)inputs[i + 3] == 1;
			// bool sweepFlag = (int)inputs[i + 4] == 1;

			endPoint = new Vector3(inputs[i + 5], inputs[i + 6]) + endPoint;

			spline.AddRelativePoint(endPoint);
			i += 7;
		}

		return endPoint;
	}

	public Vector3 AbsoluteQuad(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; i += 2)
		{
			if (i + 1 >= vectors.Count)
			{
				Debug.Log("Error: Bad number of inputs to Absolute Quadratic command.");
				break;
			}

			spline.AddAbsolutePoint(vectors[i], vectors[i], vectors[i + 1]);
		}

		return vectors[vectors.Count - 1];
	}

	public Vector3 RelativeQuad(List<Vector3> vectors, Vector3 currentPoint)
	{
		for (int i = 0; i < vectors.Count; i += 2)
		{
			if (i + 1 >= vectors.Count)
			{
				Debug.Log("Error: Bad number of inputs to Absolute Quadratic command.");
				break;
			}

			spline.AddRelativePoint(vectors[i], vectors[i], vectors[i + 1]);
		}

		return vectors[vectors.Count - 1] + currentPoint;
	}

	public Vector3 AbsoluteSmoothQuad(List<Vector3> vectors, Vector3 currentPoint)
	{
		// TODO: When previous command isn't Q or T, control point assumed to be point
		for (int i = 0; i < vectors.Count; ++i)
			spline.AddAbsoluteSmoothPoint(vectors[i]);

		return vectors[vectors.Count - 1];
	}

	public Vector3 RelativeSmoothQuad(List<Vector3> vectors, Vector3 currentPoint)
	{
		// TODO: When previous command isn't Q or T, control point assumed to be point
		for (int i = 0; i < vectors.Count; ++i)
			spline.AddRelativeSmoothPoint(vectors[i]);

		return vectors[vectors.Count - 1] + currentPoint;
	}

	public void ClosePath()
	{
		spline.ClosePath();
	}
}
