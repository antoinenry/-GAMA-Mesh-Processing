using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeshOperationGUI
{
    void Display();

    void ApplyTo(ref MeshGenerator mg, MeshFilter mf = null);
}
