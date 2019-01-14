using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

class MeshProcessor
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
    public static MeshGenerator VertexClustering (MeshGenerator mesh, float clusterSize)
    {
        // Grouping vertices in clusters
        List<Cluster> clusters = new List<Cluster>();
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 floor = vertex / clusterSize;
            floor = new Vector3(Mathf.Floor(floor.x), Mathf.Floor(floor.y), Mathf.Floor(floor.z)) * clusterSize;

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
        List<MeshGenerator.Triangle> averageTriangles = new List<MeshGenerator.Triangle>();
        foreach (MeshGenerator.Triangle triangle in mesh.triangles)
        {
            // A triangle of clusters
            Cluster clusterA = clusters.Find(c => c.vertices.Contains(mesh.vertices[triangle.A]));
            Cluster clusterB = clusters.Find(c => c.vertices.Contains(mesh.vertices[triangle.B]));
            Cluster clusterC = clusters.Find(c => c.vertices.Contains(mesh.vertices[triangle.C]));

            // A triangle of new vertices
            int averageA = averageVertices.IndexOf(clusterA.AverageVertex);
            int averageB = averageVertices.IndexOf(clusterB.AverageVertex);
            int averageC = averageVertices.IndexOf(clusterC.AverageVertex);
            MeshGenerator.Triangle averageTriangle = new MeshGenerator.Triangle(averageA, averageB, averageC);

            // Ignore flat triangles (if two or more vertices are in the same cluster) and avoid doubles
            if (averageA != averageB && averageB != averageC && averageC != averageA
                && averageTriangles.Find(t => t.Equals(averageTriangle)) == null)
                averageTriangles.Add(averageTriangle);
        }

        // Output mesh
        MeshGenerator filteredMesh = new MeshGenerator();
        filteredMesh.vertices = averageVertices;
        filteredMesh.triangles = averageTriangles;

        return filteredMesh;
    }

    public static MeshGenerator Refine(MeshGenerator mesh)
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<MeshGenerator.Triangle> newTriangles = new List<MeshGenerator.Triangle>();
        int verticesCpt = 0;

        foreach (MeshGenerator.Triangle t in mesh.triangles)
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

            newTriangles.Add(new MeshGenerator.Triangle(verticesCpt, verticesCpt + 1, verticesCpt + 2));
            newTriangles.Add(new MeshGenerator.Triangle(verticesCpt + 2, verticesCpt + 3, verticesCpt + 4));
            newTriangles.Add(new MeshGenerator.Triangle(verticesCpt + 4, verticesCpt + 5, verticesCpt + 0));
            newTriangles.Add(new MeshGenerator.Triangle(verticesCpt + 0, verticesCpt + 2, verticesCpt + 4));

            verticesCpt += 6;
        }

        MeshGenerator newMesh = new MeshGenerator();
        newMesh.vertices = newVertices;
        newMesh.triangles = newTriangles;
        return MeshProcessor.Clean(newMesh);
    }

    public static MeshGenerator Refine(MeshGenerator mesh, int times)
    {
        if (times < 1) return mesh;

        MeshGenerator mg = Refine (mesh);

        for (int i = 1; i < times; i++)
        {
            mg = Refine(mg);
        }

        return mg;
    }

    public static MeshGenerator Clean (MeshGenerator mesh)
    {
        List<Vector3> newVertices = new List<Vector3>();
        List<MeshGenerator.Triangle> newTriangles = new List<MeshGenerator.Triangle>(mesh.triangles.ToArray());
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

        foreach (MeshGenerator.Triangle t in newTriangles)
        {
            t.A = newIndices[t.A];
            t.B = newIndices[t.B];
            t.C = newIndices[t.C];
        }

        MeshGenerator newMesh = new MeshGenerator();
        newMesh.vertices = newVertices;
        newMesh.triangles = newTriangles;
        return newMesh;
    }

    public static List<Vector3> Voxelize (MeshGenerator mg, float vxSize)
    {
        List<Vector3> centers = new List<Vector3>();

        return centers;
    }
}