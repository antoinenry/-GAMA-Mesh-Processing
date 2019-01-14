using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class MeshGenerator
{
    public class Triangle
    {
        public int A; public int B; public int C;
        public Triangle() { }
        public Triangle(int a, int b, int c) { A = a; B = b; C = c; }
        public Triangle(Triangle other) { A = other.A; B = other.B; C = other.C; }
        public void Flip()
        {
            A += C;
            C = A - C;
            A -= C;
        }
        public bool Equals(Triangle other)
        {
            return (other.ContainsIndex(A) && other.ContainsIndex(B) && other.ContainsIndex(C));
        }

        public bool ContainsIndex(int index)
        {
            return (A == index || B == index || C == index);
        }

        public void ReplaceIndex(int from, int to)
        {
            if (A == from) A = to;
            if (B == from) B = to;
            if (C == from) C = to;
        }

        public List<int> ToInt()
        {
            List<int> l = new List<int>();
            l.Add(A); l.Add(B); l.Add(C);
            return l;
        }

        public static List<int> ToInt(List<Triangle> triangles)
        {
            List<int> l = new List<int>();
            foreach (Triangle t in triangles)
            {
                l.AddRange(t.ToInt());
            }
            return l;
        }

        public static List<Triangle> FromInt (int [] indices)
        {
            List<Triangle> t = new List<Triangle>();

            for (int i = 2; i < indices.Length; i+=3)
            {
                t.Add(new Triangle(indices[i - 2], indices[i - 1], indices[i]));
            }

            return t;
        }
    }

    public List<Vector3> vertices;
    public List<Triangle> triangles;

    public MeshGenerator()
    {
        vertices = new List<Vector3>();
        triangles = new List<Triangle>();
    }

    public string[] ToStrings(string decimalPrecision = "0.00000000000000000000")
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        List<string> content = new List<string>();

        content.Add("OFF");
        content.Add(vertices.Count.ToString() + " " + triangles.Count.ToString() + " " + "0");

        foreach (Vector3 v in vertices)
            content.Add(v.x.ToString(decimalPrecision) + " " + v.y.ToString(decimalPrecision) + " " + v.z.ToString(decimalPrecision));

        foreach (Triangle t in triangles)
            content.Add("3 " + t.A.ToString() + " " + t.B.ToString() + " " + t.C.ToString());

        return content.ToArray();
    }

    public void SaveFile(string path)
    {
        File.WriteAllLines(path, this.ToStrings());
    }

    public static Triangle[] Quad2Triangles(int a, int b, int c, int d)
    {
        Triangle[] t = new Triangle[2];

        t[0] = new Triangle(a, b, d);
        t[1] = new Triangle(b, c, d);

        return t;
    }

    public static MeshGenerator Quad (float a, int m)
    {
        MeshGenerator mesh = new MeshGenerator();
        float step = a / (float)m;

        for (int x = 0; x <= m; x++)
            for (int y = 0; y <= m; y++)
                mesh.vertices.Add(new Vector3((float)x * step - a / 2f, (float)y * step - a / 2f, 0f));

        for (int x = 0; x < m; x++)
            for (int y = 0; y < m; y++)
            {
                Triangle[] t = Quad2Triangles(x + y*(m+1), x + 1 + y*(m+1), x + 1 + (y+1)*(m+1), x + (y + 1) * (m+1));
                mesh.triangles.Add(t[0]);
                mesh.triangles.Add(t[1]);
            }

        return mesh;
    }

    public static MeshGenerator Cube (float a, int m)
    {
        MeshGenerator mesh = new MeshGenerator();
        MeshGenerator faceGenerator = MeshGenerator.Quad(a, m);

        faceGenerator.Translate(a / 2f * Vector3.up);
        mesh = mesh + faceGenerator;
        
        faceGenerator.Rotate(90f, 0f, 0f);
        mesh = mesh + faceGenerator;

        faceGenerator.Rotate(90f, 0f, 0f);
        mesh = mesh + faceGenerator;

        faceGenerator.Rotate(90f, 0f, 0f);
        mesh = mesh + faceGenerator;

        faceGenerator.Rotate(0f, 90f, 0f);
        mesh = mesh + faceGenerator;

        faceGenerator.Rotate(0f, 180f, 0f);
        mesh = mesh + faceGenerator;

        return mesh;
    }
    

    public static MeshGenerator Cylinder(float h, float r, int m)
    {
        MeshGenerator mesh = new MeshGenerator();

        float angStep = 2 * Mathf.PI / (float)m;
        for (int n = 0; n < m; n++)
        {
            float a = (float)n * angStep;
            mesh.vertices.Add(new Vector3(r * Mathf.Cos(a), r * Mathf.Sin(a), h / 2f));
            mesh.vertices.Add(new Vector3(r * Mathf.Cos(a), r * Mathf.Sin(a), -h / 2f));
        }

        mesh.vertices.Add(new Vector3(0f, 0f, h / 2f));
        mesh.vertices.Add(new Vector3(0f, 0f, -h / 2f));

        for (int n = 0; n < 2 * m; n += 2)
        {
            Triangle[] tQuad = Quad2Triangles(n, (n + 1) % (2 * m), (n + 3) % (2 * m), (n + 2) % (2 * m));
            mesh.triangles.Add(tQuad[0]);
            mesh.triangles.Add(tQuad[1]);
            mesh.triangles.Add(new Triangle(m, n, (n + 2) % (2 * m)));
            mesh.triangles.Add(new Triangle(m + 1, (n + 3) % (2 * m), (n + 1) % (2 * m)));
        }

        return mesh;
    }

    public static MeshGenerator Sphere(float r, int m, int p)
    {
        MeshGenerator mesh = new MeshGenerator();

        float mStep = 2 * Mathf.PI / (float)m;
        float pStep = Mathf.PI / (float)(p + 1);

        for (int q = 1; q < p + 1; q++)
        {
            float ap = (float)q * pStep;
            float yp = r * Mathf.Cos(ap);
            float rp = r * Mathf.Sin(ap);

            for (int n = 0; n < m; n++)
            {
                float am = (float)n * mStep;
                mesh.vertices.Add(new Vector3(rp * Mathf.Cos(am), rp * Mathf.Sin(am), yp));
            }
        }

        for (int q = 0; q < p - 1; q++)
        {
            for (int n = 0; n < m; n++)
            {
                Triangle[] tQuad = Quad2Triangles(q * m + n, q * m + (n + 1) % m, (q + 1) % p * m + (n + 1) % m, (q + 1) % p * m + n);
                tQuad[0].Flip();
                tQuad[1].Flip();
                mesh.triangles.Add(tQuad[0]);
                mesh.triangles.Add(tQuad[1]);
            }
        }

        mesh.vertices.Add(new Vector3(0f, 0f, r));
        mesh.vertices.Add(new Vector3(0f, 0f, -r));

        for (int n = 0; n < m; n++)
        {
            mesh.triangles.Add(new Triangle(p * m + 1, ((p - 1) * m) + (n + 1) % m, (p - 1) * m + n));
            mesh.triangles.Add(new Triangle((n + 1) % m, p * m, n));
        }

        return mesh;
    }
    public static MeshGenerator Cone(float h, float r, int m)
    {
        MeshGenerator mesh = new MeshGenerator();

        float angStep = 2 * Mathf.PI / (float)m;
        for (int n = 0; n < m; n++)
        {
            float a = (float)n * angStep;
            mesh.vertices.Add(new Vector3(r * Mathf.Cos(a), r * Mathf.Sin(a), -h / 2f));
        }

        mesh.vertices.Add(new Vector3(0f, 0f, h / 2f));
        mesh.vertices.Add(new Vector3(0f, 0f, -h / 2f));

        for (int n = 0; n < m; n++)
        {
            mesh.triangles.Add(new Triangle(n, (n + 1) % m, m));
            mesh.triangles.Add(new Triangle(n, m + 1, (n + 1) % m));
        }

        return mesh;
    }

    public static MeshGenerator Union (MeshGenerator A, MeshGenerator B)
    {
        MeshGenerator mesh = new MeshGenerator();

        return mesh;
    }

    public void Translate (Vector3 vt)
    {
        List<Vector3> newVertices = new List<Vector3>();
        foreach (Vector3 v in vertices) newVertices.Add (v + vt);
        vertices = newVertices;
    }

    public void Rotate (float xAngle, float yAngle, float zAngle)
    {
        List<Vector3> newVertices = new List<Vector3>();
        foreach (Vector3 v in vertices) newVertices.Add(Quaternion.Euler (xAngle, yAngle, zAngle) * v);
        vertices = newVertices;
    }

    public static MeshGenerator operator+ (MeshGenerator A, MeshGenerator B)
    {
        MeshGenerator AandB = new MeshGenerator();

        AandB.vertices.AddRange (A.vertices);
        AandB.triangles.AddRange(A.triangles);

        AandB.vertices.AddRange(B.vertices);
        List<Triangle> offsetTriangles = new List<Triangle>(B.triangles);
        foreach (Triangle t in offsetTriangles)
        {
            t.A += A.vertices.Count;
            t.B += A.vertices.Count;
            t.C += A.vertices.Count;
        }
        AandB.triangles.AddRange(offsetTriangles);

        return AandB;
    }
}
