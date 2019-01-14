using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombineMeshGUI : IMeshOperationGUI
{
    private MeshFilter otherMesh;

    public void Display()
    {
        otherMesh = EditorGUILayout.ObjectField(" + ", otherMesh, typeof(MeshFilter), true) as MeshFilter;
    }

    public void ApplyTo(ref MeshGenerator mg, MeshFilter mf)
    {
        MeshGenerator otherMg = new MeshGenerator();
        otherMg.vertices = new List<Vector3>(otherMesh.sharedMesh.vertices);
        otherMg.triangles = new List<MeshGenerator.Triangle>(MeshGenerator.Triangle.FromInt(otherMesh.sharedMesh.triangles));

        mg = mg + otherMg;
    }
}
