using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshGeneratorWindow : EditorWindow
{   
    public enum TOOLS { Generate, Process }
    public int toolbarIndex = 0;
    string[] toolbarStrings = { "Generate", "Transform", "Process", "Combine" };

    private MeshFilter mf;
    private MeshGenerator mg = new MeshGenerator();

    private IMeshOperationGUI currentMeshGUI;
    private GenerateMeshGUI generateGUI = new GenerateMeshGUI();
    private TransformMeshGUI transformGUI = new TransformMeshGUI();
    private ProcessMeshGUI processGUI = new ProcessMeshGUI();
    private CombineMeshGUI combineGUI = new CombineMeshGUI();

    public string meshName;

    [MenuItem("Window/Mesh Generator")]
    static void Init()
    {
        MeshGeneratorWindow window = (MeshGeneratorWindow)EditorWindow.GetWindow(typeof(MeshGeneratorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Drop a mesh", EditorStyles.boldLabel);
        mf = EditorGUILayout.ObjectField("Mesh gameobject", mf, typeof(MeshFilter), true) as MeshFilter;

        EditorGUILayout.BeginVertical("box");
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);

        switch (toolbarStrings[toolbarIndex])
        {
            case "Generate": currentMeshGUI = generateGUI;
                break;
            case "Transform": currentMeshGUI = transformGUI;
                break;
            case "Process": currentMeshGUI = processGUI;
                break;
            case "Combine": currentMeshGUI = combineGUI;
                break;
        }

        currentMeshGUI.Display();
        EditorGUILayout.EndVertical();

        meshName = EditorGUILayout.TextField("Mesh name", meshName);
        if (GUILayout.Button("Generate"))
        {
            currentMeshGUI.ApplyTo(ref mg, mf);
            SetMesh(mg.vertices, MeshGenerator.Triangle.ToInt(mg.triangles), meshName);
        }
    }

    private void SetMesh(List<Vector3> positions, List<int> triangles, string name)
    {
        if (mf == null) return;

        mf.mesh = new Mesh();
        mf.sharedMesh.vertices = positions.ConvertAll(x => (Vector3)x).ToArray();
        mf.sharedMesh.triangles = triangles.ToArray();
        mf.sharedMesh.RecalculateNormals();
        mf.sharedMesh.name = name;
    }
}
