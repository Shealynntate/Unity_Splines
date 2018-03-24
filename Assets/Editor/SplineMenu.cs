using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEditor.SceneManagement;

public class SplineMenu : EditorWindow 
{
    Object fileSource;
    bool toggleSVG;

    [SerializeField]
    private GameObject _splinePrefab;
    GameObject splinePrefab
    {
        get
        {
            if (_splinePrefab == null)
                _splinePrefab = (GameObject)Resources.Load("Prefabs/Spline", typeof(GameObject));

            return _splinePrefab;
        }
    }
     
    [SerializeField]
    static string svgName;

	[MenuItem ("Window/Spline Editor")]
    public static void Init()
    {
        SplineMenu window = GetWindow<SplineMenu>();
        window.titleContent.text = "Spline Editor";
    }

    void OnGUI() 
    {
        EditorGUILayout.BeginVertical();
        
        GUILayout.Space(10);

        GUILayout.Label("Add a new Spline to your scene");

        GUILayout.Space(10);

        toggleSVG = EditorGUILayout.Toggle("Use SVG", toggleSVG);
        
        GUILayout.Space(5);

        fileSource = EditorGUILayout.ObjectField(fileSource, typeof(Object), false, GUILayout.Width(200)) as Object;

        GUILayout.Space(10);

        if (GUILayout.Button("New Spline", GUILayout.Width(200)))
        {
            if (toggleSVG)
            {
                if (fileSource != null)
                {
                    SVGHandler.ConvertToSplines(AssetDatabase.GetAssetPath(fileSource));
                }
            }
            else
            {
                Instantiate(splinePrefab);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        EditorGUILayout.EndVertical();
    }
}
