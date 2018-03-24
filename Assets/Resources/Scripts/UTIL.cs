using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;

public static class UTIL
{

#region EDITOR COLORS

	public static Color GIZMO_PASTEL_ORANGE = new Color(0.996f, 0.502f, 0.263f);
	public static Color GIZMO_HIGHLIGHT_LINE = new Color(0.996f, 0.698f, 0.263f);
	public static Color GIZMO_LINE = new Color(0.176f, 0.667f, 0.494f);
	public static Color GIZMO_CONTROL_POINT = new Color(0.204f, 0.427f, 0.647f);

#endregion

#region SVG REGEX
	// TODO: Generalize case where last arg is Z 
	static string command = @"^\s*{0}\s*([0-9e.\+\-,\s]+)\s*(.*)";
	static string commandZ = @"^\s*{0}\s*([0-9e.\+\-,\s]+)\s*[Zz]\s*(.*)";

	public static Regex widthReg = new Regex(@"width\s*=\s*""\s*([0-9.]+)px\s*""");
	public static Regex heightReg = new Regex(@"height\s*=\s*""\s*([0-9.]+)px\s*""");

	public static Regex scaleReg = new Regex(@"scale\(([0-9.\-\+\s]+),([0-9.\-\+\s]+)\)");

	public static Regex moveToAbs = new Regex(string.Format(command, @"M"));
	public static Regex moveToRel = new Regex(string.Format(command, @"m"));
	public static Regex horzLineAbs = new Regex(string.Format(command, @"H"));
	public static Regex horzLineRel = new Regex(string.Format(command, @"h"));
	public static Regex vertLineAbs = new Regex(string.Format(command, @"V"));
	public static Regex vertLineRel = new Regex(string.Format(command, @"v"));
	public static Regex lineToAbs = new Regex(string.Format(command, @"L"));
	public static Regex lineToRel = new Regex(string.Format(command, @"l"));
	public static Regex curveAbs = new Regex(string.Format(command, @"C"));
	public static Regex curveRel = new Regex(string.Format(command, @"c"));
	public static Regex curveAbsZ = new Regex(string.Format(commandZ, @"C"));
	public static Regex curveRelZ = new Regex(string.Format(commandZ, @"c"));
	public static Regex smoothCurveAbs = new Regex(string.Format(command, @"S"));
	public static Regex smoothCurveRel = new Regex(string.Format(command, @"s"));
	public static Regex quadAbs = new Regex(string.Format(command, @"Q"));
	public static Regex quadRel = new Regex(string.Format(command, @"q"));
	public static Regex smoothQuadAbs = new Regex(string.Format(command, @"T"));
	public static Regex smoothQuadRel = new Regex(string.Format(command, @"t"));
	public static Regex arcToAbs = new Regex(string.Format(command, @"A"));
	public static Regex arcToRel = new Regex(string.Format(command, @"a"));
	public static Regex endPath = new Regex(@"^\s*[Zz]\s*(.*)");
	
#endregion

}
