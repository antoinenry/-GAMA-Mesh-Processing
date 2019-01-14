using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Modelisation
{
    class MeshFilter
    {
        private class Cluster
        {
            public Vector3 floorCoordinates;
            public List<Vector3> vertices;

            public Cluster()
            {
                vertices = new List<Vector3>();
            }

            public Cluster(Vector3 floor)
            {
                vertices = new List<Vector3>();
                floorCoordinates = floor;
            }
            public Vector3 AverageVertex
            {
                get
                {
                    Vector3 av = new Vector3();
                    foreach (Vector3 v in vertices)
                        av += v;
                    return av / vertices.Count;
                }
            }
        }
        public static OFFMesh VertexClustering (OFFMesh mesh, float clusterSize)
        {
            // Grouping vertices in clusters
            List<Cluster> clusters = new List<Cluster>();
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 floor = vertex / clusterSize;
                floor = new Vector3(MathF.Floor(floor.X), MathF.Floor(floor.Y), MathF.Floor(floor.Z)) * clusterSize;

                Cluster cluster = clusters.Find(x => x.floorCoordinates == floor);
                if (cluster == null)
                {
                    cluster = new Cluster(floor);
                    clusters.Add(cluster);
                }
                cluster.vertices.Add(vertex);
            }

            // Building new vertices list from averages by cluster
            List<Vector3> averageVertices = new List<Vector3>();
            foreach (Cluster cluster in clusters)
                averageVertices.Add(cluster.AverageVertex);

            // Building new triangle list by merging old triangles
            List<OFFMesh.Triangle> averageTriangles = new List<OFFMesh.Triangle>();
            foreach (OFFMesh.Triangle triangle in mesh.triangles)
            {
                // A triangle of clusters
                Cluster clusterA = clusters.Find(c => c.vertices.Contains(mesh.vertices[triangle.A]));
                Cluster clusterB = clusters.Find(c => c.vertices.Contains(mesh.vertices[triangle.B]));
                Cluster clusterC = clusters.Find(c => c.vertices.Contains(mesh.vertices[triangle.C]));

                // A triangle of new vertices
                int averageA = averageVertices.IndexOf(clusterA.AverageVertex);
                int averageB = averageVertices.IndexOf(clusterB.AverageVertex);
                int averageC = averageVertices.IndexOf(clusterC.AverageVertex);
                OFFMesh.Triangle averageTriangle = new OFFMesh.Triangle(averageA, averageB, averageC);

                // Ignore flat triangles (if two or more vertices are in the same cluster) and avoid doubles
                if (averageA != averageB && averageB != averageC && averageC != averageA
                    && averageTriangles.Find(t => t.Equals(averageTriangle)) == null)
                    averageTriangles.Add(averageTriangle);
            }

            // Output mesh
            OFFMesh filteredMesh = new OFFMesh();
            filteredMesh.vertices = averageVertices;
            filteredMesh.triangles = averageTriangles;

            return filteredMesh;
        }

        public static OFFMesh Refine(OFFMesh mesh)
        {
            List<Vector3> newVertices = new List<Vector3>();
            List<OFFMesh.Triangle> newTriangles = new List<OFFMesh.Triangle>();
            int verticesCpt = 0;

            foreach (OFFMesh.Triangle t in mesh.triangles)
            {
                Vector3 AB = (mesh.vertices[t.A] + mesh.vertices[t.B]) / 2f;
                Vector3 BC = (mesh.vertices[t.B] + mesh.vertices[t.C]) / 2f;
                Vector3 CA = (mesh.vertices[t.C] + mesh.vertices[t.A]) / 2f;
   
                newVertices.Add(AB);
                newVertices.Add(mesh.vertices[t.B]);
                newVertices.Add(BC);
                newVertices.Add(mesh.vertices[t.C]);
                newVertices.Add(CA);
                newVertices.Add(mesh.vertices[t.A]);

                newTriangles.Add(new OFFMesh.Triangle(verticesCpt, verticesCpt + 1, verticesCpt + 2));
                newTriangles.Add(new OFFMesh.Triangle(verticesCpt + 2, verticesCpt + 3, verticesCpt + 4));
                newTriangles.Add(new OFFMesh.Triangle(verticesCpt + 4, verticesCpt + 5, verticesCpt + 0));
                newTriangles.Add(new OFFMesh.Triangle(verticesCpt + 0, verticesCpt + 2, verticesCpt + 4));

                verticesCpt += 6;
            }

            OFFMesh newMesh = new OFFMesh();
            newMesh.vertices = newVertices;
            newMesh.triangles = newTriangles;
            return MeshFilter.Clean(newMesh);
        }
        public static OFFMesh Clean (OFFMesh mesh)
        {
            List<Vector3> newVertices = new List<Vector3>();
            List<OFFMesh.Triangle> newTriangles = new List<OFFMesh.Triangle>(mesh.triangles.ToArray());
            int newVerticesCount = 0;
            List<int> newIndices = new List<int>();

            for (int i = 0; i < mesh.vertices.Count; i++)
            {
                int newIndex = newVertices.IndexOf(mesh.vertices[i]);

                if (newIndex < 0)
                {
                    newVertices.Add(mesh.vertices[i]);                                        
                    newIndices.Add(newVerticesCount++);
                }
                else
                {
                    newIndices.Add(newIndex);
                }
            }

            foreach (OFFMesh.Triangle t in newTriangles)
            {
                t.A = newIndices[t.A];
                t.B = newIndices[t.B];
                t.C = newIndices[t.C];
            }

            OFFMesh newMesh = new OFFMesh();
            newMesh.vertices = newVertices;
            newMesh.triangles = newTriangles;
            return newMesh;
        }
    }
}