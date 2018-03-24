using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;


public static class SVGHandler
{		
	// SVG state variables
	static int widthSVG = -1;
	static int heightSVG = -1;
	static Vector2 scaleSVG = new Vector2(1, 1);

	public static void ConvertToSplines(string filePath)
	{
		var paths = ParseFile(filePath);

		foreach (var path in paths)
		{
			ParsePath(path);
		}

		// Reset state for next SVG path.
		widthSVG = -1;
		heightSVG = -1;
		scaleSVG = new Vector2(1, 1);
	}

	static SplineGenerator CreateSpline()
	{
		var prefab = (GameObject)Resources.Load("Prefabs/Spline", typeof(GameObject));
		return GameObject.Instantiate(prefab).GetComponent<SplineGenerator>();
	}

	// Extract all paths from SVG file.
    static List<string> ParseFile(string filePath)
	{
		Regex fullPathReg = new Regex(@"\s*<path.*\sd=""([^""\']+)"".*/>");
        Regex partialPathReg = new Regex(@"\s*<path.*\sd=""");

        List<string> paths = new List<string>();
        StreamReader reader = new StreamReader(filePath); 
		
        string line = " ";
		Match m;

		while(!reader.EndOfStream)
		{
			line += " " + reader.ReadLine();

			if (scaleSVG.x == 1 && scaleSVG.y == 1 && tryMatch(UTIL.scaleReg, line, out m))
			{
				scaleSVG.x = float.Parse(m.Groups[1].ToString());
				scaleSVG.y = float.Parse(m.Groups[2].ToString());
			}

			if (widthSVG < 0 && tryMatch(UTIL.widthReg, line, out m))
				widthSVG = int.Parse(m.Groups[1].ToString());
			
			if (heightSVG < 0 && tryMatch(UTIL.heightReg, line, out m))
				heightSVG = int.Parse(m.Groups[1].ToString());
			
			m = fullPathReg.Match(line);

			if (m.Success)
            {
                paths.Add(m.Groups[1].ToString());
                line = " ";
            }
			else
			{
				// Check if line contains start of path - if not, throw away
				m = partialPathReg.Match(line);
				if (!m.Success)
				{
					line = " ";
				}
			}			
		}

        reader.Close();

        return paths;
	}

	public static void ParsePath(string path)
	{		
		Match m;
		List<float> coords = null;

		Vector3 currentPoint = Vector3.zero;
		Vector3 startPoint = Vector3.zero;
		
		SplineGenerator spline = null;
		
		while (path.Length > 0)
		{
			if (spline == null)
				spline = CreateSpline();

			if (tryMatch(UTIL.moveToAbs, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);

				if (spline.GetComponent<Spline>().Size() == 0)
					spline.transform.position = vectors[0];

				currentPoint = spline.AbsoluteMoveTo(vectors, currentPoint);
				startPoint = currentPoint;
			}
			else if (tryMatch(UTIL.moveToRel, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				if (spline.GetComponent<Spline>().Size() == 0)
					spline.transform.position = vectors[0] + currentPoint;

				currentPoint = spline.RelativeMoveTo(vectors, currentPoint);
				startPoint = currentPoint;
			}
			else if (tryMatch(UTIL.horzLineAbs, ref path, out coords))
			{
				var vectors = floatXToAbsVectors(coords, currentPoint.y, false);

				currentPoint = spline.AbsoluteLineTo(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.horzLineRel, ref path, out coords))
			{
				var vectors = floatXToRelVectors(coords, 0);

				currentPoint = spline.RelativeLineTo(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.vertLineAbs, ref path, out coords))
			{
				var vectors = floatYToAbsVectors(coords, currentPoint.x);
					
				currentPoint = spline.AbsoluteLineTo(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.vertLineRel, ref path, out coords))
			{
				var vectors = floatYToRelVectors(coords, 0);

				currentPoint = spline.RelativeLineTo(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.lineToAbs, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);

				currentPoint = spline.AbsoluteLineTo(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.lineToRel, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				currentPoint = spline.RelativeLineTo(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.curveAbsZ, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);

				if (vectors.Count % 3 == 0)
				{	
					currentPoint = spline.AbsoluteCurve(vectors, currentPoint);	
					currentPoint = spline.AbsoluteLineTo(new List<Vector3>{startPoint}, currentPoint);
				}
				else
				{
					vectors.Add(startPoint);
					currentPoint = spline.AbsoluteCurve(vectors, currentPoint);	
				}
				
				startPoint = currentPoint;
				spline = null;
			}
			else if (tryMatch(UTIL.curveRelZ, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				if (vectors.Count % 3 == 0)
				{	
					currentPoint = spline.RelativeCurve(vectors, currentPoint);
					currentPoint = spline.AbsoluteLineTo(new List<Vector3>{startPoint}, currentPoint);
				}
				else
				{
					vectors.Add(startPoint - currentPoint);
					currentPoint = spline.RelativeCurve(vectors, currentPoint);
				}

				startPoint = currentPoint;
				spline = null;
			}
			else if (tryMatch(UTIL.curveAbs, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);
				
				currentPoint = spline.AbsoluteCurve(vectors, currentPoint);	
			}
			else if (tryMatch(UTIL.curveRel, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				currentPoint = spline.RelativeCurve(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.smoothCurveAbs, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);

				currentPoint = spline.AbsoluteSmoothCurve(vectors, currentPoint);	
			}
			else if (tryMatch(UTIL.smoothCurveRel, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				currentPoint = spline.RelativeSmoothCurve(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.quadAbs, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);

				currentPoint = spline.AbsoluteQuad(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.quadRel, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				currentPoint = spline.RelativeQuad(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.smoothQuadAbs, ref path, out coords))
			{
				var vectors = floatXYToAbsVectors(coords);

				currentPoint = spline.AbsoluteSmoothQuad(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.smoothQuadRel, ref path, out coords))
			{
				var vectors = floatXYToRelVectors(coords);

				currentPoint = spline.RelativeSmoothQuad(vectors, currentPoint);
			}
			else if (tryMatch(UTIL.arcToAbs, ref path, out coords))
			{
				//TODO: Invert coords 5 & 6
				currentPoint = spline.AbsoluteArc(coords, currentPoint);
			}
			else if (tryMatch(UTIL.arcToRel, ref path, out coords))
			{
				//TODO: Invert coords 5 & 6
				currentPoint = spline.RelativeArc(coords, currentPoint);
			}
			else if (tryMatch(UTIL.endPath, path, out m))
			{
				spline.ClosePath();
				spline = null;
				path = m.Groups[1].ToString();
			}
			else
			{
				Debug.Log("Couldn't Match: " + path);
				path = "";
			}
		}
	}

	static bool tryMatch(Regex regex, string input, out Match match)
	{
		match = regex.Match(input);
		
		return match.Success;
	}

	static bool tryMatch(Regex reg, ref string path, out List<float> coords)
	{
		Match m = reg.Match(path);

		coords = m.Success ? Input2Floats(m.Groups[1].ToString()) : null;
		path = m.Success ? m.Groups[2].ToString() : path;

		return m.Success;
	}

	// First splits on commas. 
	// Then breaks apart numbers where +/- sign was separation point.
	static List<float> Input2Floats(string inputs)
	{
		var result = new List<float>();

		string[] tokens = inputs.Split(new char[] {',', ' ', '\t'});

		foreach (string t in tokens)
		{
			int start = 0;

			for (int i = 0; i < t.Length; ++i)
			{
				if (i > 0 && (t[i] == '-' && t[i - 1] != 'e') || (t[i] == '+' && t[i - 1] != 'e'))
				{
					result.Add(float.Parse(t.Substring(start, i - start)));
					start = i;
				}
			}

			if (start < t.Length)
			{
				result.Add(float.Parse(t.Substring(start, t.Length - start)));
			}
		}

		return result;
	}

	static List<Vector3> floatXToAbsVectors(List<float> inputs, float y, bool shouldInvert = true)
	{
		var result = new List<Vector3>();

		for (int i = 0; i < inputs.Count; ++i)
			result.Add(new Vector3(inputs[i] * scaleSVG.x, (shouldInvert ? invertAbs(y) : y) * scaleSVG.y));

		return result;
	}

	static List<Vector3> floatXToRelVectors(List<float> inputs, float y)
	{
		var result = new List<Vector3>();

		for (int i = 0; i < inputs.Count; ++i)
			result.Add(new Vector3(inputs[i] * scaleSVG.x, invertRel(y) * scaleSVG.y ));

		return result;
	}

	static List<Vector3> floatYToAbsVectors(List<float> inputs, float x)
	{
		var result = new List<Vector3>();

		for (int i = 0; i < inputs.Count; ++i)
			result.Add(new Vector3(x * scaleSVG.x, invertAbs(inputs[i]) * scaleSVG.y));

		return result;
	}

	static List<Vector3> floatYToRelVectors(List<float> inputs, float x)
	{
		var result = new List<Vector3>();

		for (int i = 0; i < inputs.Count; ++i)
			result.Add(new Vector3(x * scaleSVG.x, invertRel(inputs[i]) * scaleSVG.y));

		return result;
	}

	static List<Vector3> floatXYToAbsVectors(List<float> inputs)
	{
		var result = new List<Vector3>();

		for (int i = 0; i < inputs.Count; i += 2)
		{
			if (i + 1 == inputs.Count)
			{
				Debug.Log("Incorrect number of arguments");
				break;
			}

			result.Add(new Vector3(inputs[i] * scaleSVG.x, invertAbs(inputs[i + 1]) * scaleSVG.y));
		}

		return result;
	}

	static List<Vector3> floatXYToRelVectors(List<float> inputs)
	{
		var result = new List<Vector3>();

		for (int i = 0; i < inputs.Count; i += 2)
		{
			if (i + 1 == inputs.Count)
			{
				Debug.Log("Incorrect number of arguments");
				break;
			}

			result.Add(new Vector3(inputs[i] * scaleSVG.x, invertRel(inputs[i + 1]) * scaleSVG.y));
		}

		return result;
	}

	// Invert y-axis since it points down in SVG files.
	static float invertAbs(float y)
	{
		return (heightSVG > 0) ? heightSVG - y : -y;
	}

	static float invertRel(float y)
	{
		return -y;
	}
}
