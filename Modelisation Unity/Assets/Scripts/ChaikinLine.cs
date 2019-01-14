using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaikinLine : MonoBehaviour {
    
    [Range (0, 10)]
    public int subdivisionLevel;

    private int _subdivisionLevel;
    private LineRenderer line;
    private EdgeCollider2D col;
    private List<Vector3[]> subdivisions;

	void Start ()
    {
        line = GetComponent<LineRenderer>();
        col = GetComponent<EdgeCollider2D>();
        InitSubdivisions();
        SetLine(subdivisions[subdivisionLevel]);
    }

    public void InitSubdivisions()
    {
        subdivisions = new List<Vector3[]>();

        Vector3[] previousSub = new Vector3[line.positionCount];
        line.GetPositions(previousSub);
        subdivisions.Add(previousSub);

        for (int i = 0; i < 10; i++)
        {
            previousSub = Subdivide(previousSub);
            subdivisions.Add(previousSub);
        }
    }

    void Update ()
    {
        if (subdivisionLevel >= 0 && subdivisionLevel < 10 && _subdivisionLevel != subdivisionLevel)
        {
            _subdivisionLevel = subdivisionLevel;
            SetLine(subdivisions[subdivisionLevel]);
        }
    }

    private void SetLine (Vector3[] positions)
    {
        line.positionCount = positions.Length;
        line.SetPositions(positions);

        Vector2 [] pos2D = new Vector2[positions.Length + 1];
        for (int i = 0; i < positions.Length; i++)
            pos2D[i] = new Vector2(positions[i].x, positions[i].y);
        pos2D[positions.Length] = new Vector2(positions[0].x, positions[0].y);
        col.points = pos2D;
    }

    private Vector3 [] Subdivide (Vector3 [] positions)
    {
        List<Vector3> newPositions = new List<Vector3>();
        for (int i = 0, imax = positions.Length; i < imax; i++)
        {
            Vector3[] sub = DivideAngle(positions[i], positions[(i + 1) % imax], positions[(i + 2) % imax]);
            newPositions.AddRange(sub);
        }
        return newPositions.ToArray();
    }

    private Vector3[] DivideAngle (Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3[] output = new Vector3[2];
        output[0] = A / 4f + 3f * B / 4f;
        output[1] = 3f * B / 4f + C / 4f;

        return output;
    }
}
