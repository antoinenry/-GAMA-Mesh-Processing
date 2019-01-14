using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateMeshGUI: IMeshOperationGUI
{
    public enum MESH_TYPE
    {
        LOAD_FILE = 0,
        QUAD = 1,
        CUBE = 2,
        SPHERE = 3,
        CYLINDER = 4,
        CONE = 5,
    }

    public MESH_TYPE meshTypeSelection;
    public string meshFilePath;

    public float fParameter1;
    public float fParameter2;
    public int iParameter1;
    public int iParameter2;

    public void Display ()
    {        
        meshTypeSelection = (MESH_TYPE)EditorGUILayout.EnumPopup("Mesh type:", meshTypeSelection);

        if (meshTypeSelection == MESH_TYPE.LOAD_FILE)
        {
            if (GUILayout.Button("Open file"))
            {
                meshFilePath = EditorUtility.OpenFolderPanel("Load mesh", "", "");
            }
            if (meshFilePath != "")
                EditorGUILayout.TextArea(meshFilePath);
        }
        else
        {
            MeshSetupGUI(meshTypeSelection);
        }
    }

    public void ApplyTo (ref MeshGenerator mg, MeshFilter mf)
    {
        SetMesh(meshTypeSelection, ref mg);
    }

    private void MeshSetupGUI(MESH_TYPE mt)
    {
        switch (mt)
        {
            case MESH_TYPE.QUAD:
            case MESH_TYPE.CUBE:
                fParameter1 = EditorGUILayout.FloatField("Size:", fParameter1);
                iParameter1 = EditorGUILayout.IntField("Meridians count:", iParameter1);
                break;

            case MESH_TYPE.SPHERE:
                fParameter1 = EditorGUILayout.FloatField("Radius:", fParameter1);
                iParameter1 = EditorGUILayout.IntField("Meridians count:", iParameter1);
                iParameter2 = EditorGUILayout.IntField("Parallels count: ", iParameter2);
                break;

            case MESH_TYPE.CYLINDER:
            case MESH_TYPE.CONE:
                fParameter1 = EditorGUILayout.FloatField("Height:", fParameter1);
                fParameter2 = EditorGUILayout.FloatField("Radius:", fParameter2);
                iParameter1 = EditorGUILayout.IntField("Meridians count: ", iParameter1);
                break;
        }
    }

    private void SetMesh(MESH_TYPE mt, ref MeshGenerator mg)
    {
        switch (mt)
        {
            case MESH_TYPE.QUAD:
                mg = MeshGenerator.Quad(fParameter1, iParameter1);
                break;

            case MESH_TYPE.CUBE:
                mg = MeshGenerator.Cube(fParameter1, iParameter1);
                break;

            case MESH_TYPE.SPHERE:
                mg = MeshGenerator.Sphere(fParameter1, iParameter1, iParameter2);
                break;

            case MESH_TYPE.CYLINDER:
                mg = MeshGenerator.Cylinder(fParameter1, fParameter2, iParameter1);
                break;

            case MESH_TYPE.CONE:
                mg = MeshGenerator.Cone(fParameter1, fParameter2, iParameter1);
                break;

            default:
                return;
        }
    }
}
