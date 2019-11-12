using UnityEngine;
using UnityEditor;
using Endless2DTerrain;

[CustomEditor(typeof(TerrainDisplayer))]
public class TerrainEditor : Editor
{
    private TerrainDisplayer td;

    void OnEnable()
    {
        td = target as TerrainDisplayer;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10f);
        if (GUILayout.Button("Preview Terrain"))
        {
            td.Setup();
            td.GenerateTerrain(td.PreviewLength);
        }
    }
}
