using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformMeshGUI : IMeshOperationGUI
{
    private Vector3 translation;
    private Vector3 rotation;
    private bool activeTranslation;
    private bool activeRotation;

    public void Display()
    {
        activeTranslation = EditorGUILayout.BeginToggleGroup("Translation", activeTranslation);
        translation = EditorGUILayout.Vector3Field("Vector:", translation);
        EditorGUILayout.EndToggleGroup();

        activeRotation = EditorGUILayout.BeginToggleGroup("Rotation", activeRotation);
        rotation = EditorGUILayout.Vector3Field("Euler angles:", rotation);
        EditorGUILayout.EndToggleGroup();
    }

    public void ApplyTo(ref MeshGenerator mg, MeshFilter mf)
    {
        if (activeTranslation) mg.Translate(translation);
        if (activeRotation) mg.Rotate(rotation.x, rotation.y, rotation.z);
    }
}
