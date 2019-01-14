using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProcessMeshGUI : IMeshOperationGUI
{
    public float fParameter1;
    public float fParameter2;
    public int iParameter1;
    public bool activeClustering;
    public bool activeRefining;
    public bool activeVoxelizing;

    public void Display ()
    {
        activeClustering = EditorGUILayout.BeginToggleGroup("Vertex Clustering", activeClustering);
        fParameter1 = EditorGUILayout.FloatField("Cluster size:", fParameter1);
        EditorGUILayout.EndToggleGroup();

        activeRefining = EditorGUILayout.BeginToggleGroup("Refine", activeRefining);
        iParameter1 = EditorGUILayout.IntField("Iterations", iParameter1);
        EditorGUILayout.EndToggleGroup();

        activeVoxelizing = EditorGUILayout.BeginToggleGroup("Voxelize", activeVoxelizing);
        fParameter2 = EditorGUILayout.FloatField("Voxel size", fParameter2);
        EditorGUILayout.EndToggleGroup();
    }

    public void ApplyTo (ref MeshGenerator mg, MeshFilter mf)
    {
        mg.vertices = new List<Vector3>(mf.sharedMesh.vertices);
        mg.triangles = new List<MeshGenerator.Triangle>(MeshGenerator.Triangle.FromInt(mf.sharedMesh.triangles));

        if (activeClustering && fParameter1 > 0f) mg = MeshProcessor.VertexClustering(mg, fParameter1);
        if (activeRefining && iParameter1 > 0) mg = MeshProcessor.Refine(mg, iParameter1);
    }
}
