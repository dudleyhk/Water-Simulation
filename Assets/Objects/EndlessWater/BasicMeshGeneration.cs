
using UnityEngine;
using System.Collections.Generic;


class BasicMeshGeneration : MonoBehaviour
{
    public float size = 10;
    public float spacing = 1;

    private int width;
    private MeshFilter meshFilter;



    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        // Calc the data we need to generate the water mesh
        width = (int)(size / spacing);

        // because each square is 2 vertices, so we need one more. 
        width += 1;

        GenerateMesh();
    }



    public void GenerateMesh()
    {
        List<Vector3[]> verts = new List<Vector3[]>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int z = 0; z < width; z++)
        {
            verts.Add(new Vector3[width]);
            for (int x = 0; x < width; x++)
            {
                Vector3 currentPoint = new Vector3();

                // Get the coordinates of the vertices;
                currentPoint.x = x * spacing;
                currentPoint.z = z * spacing;
                currentPoint.y = this.transform.position.y;

                verts[z][x] = currentPoint;

                uvs.Add(new Vector2(x, z));

                // Dont generate a triangle the first coordinate on each row.
                // Because thats just one point. 
                if (x <= 0 || z <= 0)
                {
                    continue;
                }

                // Create the quad.
                // each square consists of 2 triangles
                // The triangle south-west of the vertice
                tris.Add(x + z * width);                       
                tris.Add(x + (z - 1) * width);                 
                tris.Add((x - 1) + (z - 1) * width);           
                                                               
                tris.Add(x + z * width);                       
                tris.Add((x - 1) + (z - 1) * width);           
                tris.Add((x - 1) + z * width);                                                                       
            }
        }

        // Unfold the 2D array of vertices into a 1D array
        Vector3[] unfoldedVerts = new Vector3[width * width];

        int idx = 0;
        foreach (var v in verts)
        {
            //copies all the elements of the current 1D-array to the specifies 1D array.
            v.CopyTo(unfoldedVerts, idx * width);
            idx++;
        }

        // Generate the mesh object.
        Mesh mesh = new Mesh();
        mesh.vertices = unfoldedVerts;
        // newMesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();

        // Ensure the bounding volume is correct.
        mesh.RecalculateBounds();

        // Update the normals to reflect the change. 
        mesh.RecalculateNormals();

        // Add the generated mesh to this GameObject.
        meshFilter.mesh.Clear();
        meshFilter.mesh = mesh;
        meshFilter.mesh.name = "Water Mesh";

        print("Number of vertices in Water Mesh: " + meshFilter.mesh.vertices.Length);
    }

}
