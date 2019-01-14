using System;

namespace Modelisation
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder = "D:/TPOutputs";

            OFFMesh mesh = OFFMesh.Sphere(5f, 10, 10);
            //OFFMesh mesh = OFFMesh.Cylinder (5f, 1f, 100);
            //OFFMesh mesh = OFFMesh.Cone(5f, 1f, 100);

            //OFFMesh filterdMesh = MeshFilter.VertexClustering(mesh, 2f);
            OFFMesh filterdMesh = MeshFilter.Refine(mesh);
            mesh.SaveFile(folder + "/filter_before.off");
            filterdMesh.SaveFile(folder + "/filter_after.off");
        }
    }
}