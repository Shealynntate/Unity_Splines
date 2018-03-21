using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveDemo : MonoBehaviour 
{
	[Header("Control Points")]
	public Transform p0;
	public Transform p1;
	public Transform p2;
	public Transform p3;
	[Space(10)]
	[Header("Secondary Points")]
	public Transform a;
	public Transform b;
	public Transform c;
	public Transform d;
	public Transform e;
	public Transform P;

	[Space(10)]
	[Range(0, 1)]public float t;
	public int NumSubDivisions;
	public bool PrimaryLinesVisible = true;
	public bool SecondaryLinesVisible = true;
	public bool TertiaryLinesVisible = true;

	LineRenderer p0_p1;
	LineRenderer p1_p2;
	LineRenderer p2_p3;

	LineRenderer a_b;
	LineRenderer b_c;

	LineRenderer d_e;
	
	List<LineRenderer> curveLines;

	Color grey;
	float _t;
	float _numSubDivisions;

	void Start() 
	{
		Init();
		UpdateLines();
		UpdateSubDivisions();
	}

	public void Init()
	{
		grey = new Color(0.6f, 0.6f, 0.6f);
		
		if (p0_p1 == null) p0_p1 = CreateLine(p0.position, "Line 0");
		if (p1_p2 == null) p1_p2 = CreateLine(p1.position, "Line 1");
		if (p2_p3 == null) p2_p3 = CreateLine(p2.position, "Line 2");
		if (a_b == null) a_b = CreateLine(a.position, "Line 3");
		if (b_c == null) b_c = CreateLine(b.position, "Line 4");
		if (d_e == null) d_e = CreateLine(d.position, "Line 5");

		if (curveLines == null)
		{
			curveLines = new List<LineRenderer>();
			
			for (int i = 0; i < NumSubDivisions; i++)
				curveLines.Add(CreateLine(Vector3.zero, ""));
		}
	}

	LineRenderer CreateLine(Vector3 start, string name)
	{
		GameObject line = new GameObject();
		
		line.name = name;
		line.transform.SetParent(transform);
		line.transform.position = start;
		LineRenderer lr = line.AddComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Sprites/Default"));

		return lr;
	}
	
	void Update() 
	{
		p0_p1.enabled = PrimaryLinesVisible;
		p1_p2.enabled = PrimaryLinesVisible;
		p2_p3.enabled = PrimaryLinesVisible;

		a.GetComponent<SpriteRenderer>().enabled = SecondaryLinesVisible;
		b.GetComponent<SpriteRenderer>().enabled = SecondaryLinesVisible;
		c.GetComponent<SpriteRenderer>().enabled = SecondaryLinesVisible;
		a_b.enabled = SecondaryLinesVisible;
		b_c.enabled = SecondaryLinesVisible;

		d.GetComponent<SpriteRenderer>().enabled = TertiaryLinesVisible;
		e.GetComponent<SpriteRenderer>().enabled = TertiaryLinesVisible;
		d_e.enabled = TertiaryLinesVisible;

		if (_t != t)
		{
			UpdateLines();
			_t = t;
		}
		if (_numSubDivisions != NumSubDivisions)
		{
			UpdateSubDivisions();
			_numSubDivisions = NumSubDivisions;
		}
	}


	public void UpdateLines()
	{
		//Note: Ignoring z-axis for demo 

		DrawLine(p0_p1, p0.position, p1.position, grey);
		DrawLine(p1_p2, p1.position, p2.position, grey);
		DrawLine(p2_p3, p2.position, p3.position, grey);

		a.position = Vector3.Lerp(p0.position, p1.position, t);
		b.position = Vector3.Lerp(p1.position, p2.position, t);
		c.position = Vector3.Lerp(p2.position, p3.position, t);
		
		DrawLine(a_b, a.position, b.position, grey);
		DrawLine(b_c, b.position, c.position, grey);
		
		d.position = Vector3.Lerp(a.position, b.position, t);
		e.position = Vector3.Lerp(b.position, c.position, t);

		DrawLine(d_e, d.position, e.position, grey);

		P.position = Vector3.Lerp(d.position, e.position, t);

		UpdateSubDivisions();
	}

	void UpdateSubDivisions()
	{
		while (curveLines.Count > NumSubDivisions)
		{
			LineRenderer lr = curveLines[0];
			curveLines.Remove(lr);
			GameObject.Destroy(lr.gameObject);
		}
		
		while (curveLines.Count < NumSubDivisions)
			curveLines.Add(CreateLine(Vector3.zero, ""));
		
		float step = t / NumSubDivisions;
		Vector3[] points = new Vector3[4]{ p0.position, p1.position, p2.position, p3.position };

		for (int i = 0; i < NumSubDivisions; i++)
		{
			LineRenderer lr = curveLines[i];
			DrawLine(lr, GetPoint(points, step * i), GetPoint(points, step * (i + 1)), grey);
		}
	}

	void DrawLine(LineRenderer lr, Vector3 start, Vector3 end, Color color)
	{
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.02f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
	}

	Vector3 GetPoint(Vector3[] points, float t)
	{
		float alpha = 1f - t;
		float alpha_2 = alpha * alpha;
		float t_2 = t * t;
		
		return	points[0] * ( alpha_2 * alpha ) +
				points[1] * ( 3f * alpha_2 * t ) +
				points[2] * ( 3f * alpha * t_2 ) +
				points[3] * ( t_2 * t );
	}
}
