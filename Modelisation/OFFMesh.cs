using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;

namespace Modelisation
{
    class OFFMesh
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

            public bool ContainsIndex (int index)
            {
                return (A == index || B == index || C == index);
            }

            public void ReplaceIndex (int from, int to)
            {
                if (A == from) A = to;
                if (B == from) B = to;
                if (C == from) C = to;
            }
        }

        public List<Vector3> vertices;
        public List<Triangle> triangles;

        public OFFMesh ()
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
                content.Add( v.X.ToString(decimalPrecision) + " " + v.Y.ToString(decimalPrecision) + " " + v.Z.ToString(decimalPrecision));

            foreach (Triangle t in triangles)
                content.Add("3 " + t.A.ToString() + " " + t.B.ToString() + " " + t.C.ToString());

            return content.ToArray();
        }
        public void SaveFile(string path)
        {
            File.WriteAllLines(path, this.ToStrings());
        }

        public static Triangle[] Quad2Triangles (int a, int b, int c, int d)
        {
            Triangle[] t = new Triangle[2];

            t[0] = new Triangle(a, b, d);
            t[1] = new Triangle(b, c, d);

            return t;
        }

        /*public static OFFMesh Cube (float a, int m)
        {
            OFFMesh mesh = new OFFMesh();

            float step = a / (float)m;
            for (int x = 0; x < m; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    mesh.vertices.Add(new Vector3(x*step - a/2f, y*step - a/2f, a/2f));
                }
            }

            return mesh;
        }
        */
        public static OFFMesh Cylinder (float h, float r, int m)
        {
            OFFMesh mesh = new OFFMesh();

            float angStep = 2 * MathF.PI / (float)m;
            for (int n = 0; n < m; n++)
            {
                float a = (float)n * angStep;
                mesh.vertices.Add(new Vector3(r*MathF.Cos(a), r*MathF.Sin(a), h/2f));
                mesh.vertices.Add(new Vector3(r*MathF.Cos(a), r*MathF.Sin(a), -h/2f));
            }

            mesh.vertices.Add(new Vector3(0f, 0f, h / 2f));
            mesh.vertices.Add(new Vector3(0f, 0f, -h / 2f));

            for (int n = 0; n < 2*m; n+=2)
            {
                Triangle[] tQuad = Quad2Triangles(n, (n + 1) % (2*m), (n + 3) % (2*m), (n + 2) % (2*m));
                mesh.triangles.Add(tQuad[0]);
                mesh.triangles.Add(tQuad[1]);
                mesh.triangles.Add(new Triangle(m, n, (n + 2) % (2 * m)));
                mesh.triangles.Add(new Triangle(m+1, (n + 3) % (2 * m), (n + 1) % (2 * m)));
            }

            return mesh;
        }

        public static OFFMesh Sphere (float r, int m, int p)
        {
            OFFMesh mesh = new OFFMesh();

            float mStep = 2 * MathF.PI / (float)m;
            float pStep = MathF.PI / (float)(p+1);

            for (int q = 1; q < p+1; q++)
            {
                float ap = (float)q * pStep;
                float yp = r * MathF.Cos(ap);
                float rp = r * MathF.Sin(ap);

                for (int n = 0; n < m; n++)
                {
                    float am = (float)n * mStep;
                    mesh.vertices.Add(new Vector3(rp * MathF.Cos(am), rp * MathF.Sin(am), yp));
                }
            }

            for (int q = 0; q < p-1; q++)
            {
                for (int n = 0; n < m; n++)
                {
                    Triangle[] tQuad = Quad2Triangles(q*m + n, q * m + (n + 1) % m, (q + 1) % p * m + (n + 1) % m, (q+1)%p * m + n);
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
                mesh.triangles.Add(new Triangle(p * m + 1, ((p-1) * m) + (n + 1) % m, (p-1) * m + n));
                mesh.triangles.Add(new Triangle((n + 1) % m, p * m, n));
            }

            return mesh;
        }
        public static OFFMesh Cone(float h, float r, int m)
        {
            OFFMesh mesh = new OFFMesh();

            float angStep = 2 * MathF.PI / (float)m;
            for (int n = 0; n < m; n++)
            {
                float a = (float)n * angStep;
                mesh.vertices.Add(new Vector3(r * MathF.Cos(a), r * MathF.Sin(a), -h / 2f));
            }

            mesh.vertices.Add(new Vector3(0f, 0f, h / 2f));
            mesh.vertices.Add(new Vector3(0f, 0f, -h / 2f));

            for (int n = 0; n < m; n ++)
            {
                mesh.triangles.Add(new Triangle (n, (n+1)%m, m));
                mesh.triangles.Add(new Triangle(n, m+1, (n + 1) % m));
            }

            return mesh;
        }
    }
}
