using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSquare : MonoBehaviour
{
    public Transform squareTransform;

    // Add the wave mesh to the MeshFilter.
    public MeshFilter terrainMeshFilter;

    // the total size in m
    private float size;

    // Res = width of one square
    private float spacing;

    // total number of vertices we need to generate based on size and spacing.
    private int width;

    // For the thread to update the water.
    // The local centre position of this square to fake transform point in a thread.
    public Vector3 centrePos;

    // The latest vertices that belong to this square. 
    public Vector3[] vertices;

    public WaterSquare(GameObject waterSquareObj, float size, float spacing)
    {
        this.squareTransform = waterSquareObj.transform;
        this.size = size;
        this.spacing = spacing;
        this.terrainMeshFilter = squareTransform.GetComponent<MeshFilter>();

        // Calc the data we need to generate the water mesh
        width = (int)(size / spacing);

        // because each square is 2 vertices, so we need one more. 
        width += 1;

        // Centre the sea
        float offset = -((width - 1) * spacing) / 2;

        Vector3 newPos = new Vector3(offset, squareTransform.position.y, offset);

        squareTransform.position += newPos;

        //Save the centre position of the square.
        this.centrePos = waterSquareObj.transform.localPosition;

        // Genertate the sea.
        // To calculate the position of the square.
        float startTime = System.Environment.TickCount;

        GenerateMesh();

        // Calculate the time it took to generate the terrain in seconds
        float timeToGenerateSea = (System.Environment.TickCount - startTime) / 1000f;

        print("The sea was generated in " + timeToGenerateSea.ToString());

        this.vertices = terrainMeshFilter.mesh.vertices;
    }


    // If we are update the square from outside of a thread.
    public void MoveSea(Vector3 oceanPos, float timeSinceStart)
    {
        Vector3[] vertices = terrainMeshFilter.mesh.vertices;

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            // From local to global 
            // Vector3 vertexGlobal = squareTransform.TransformPoint(vertex);

            Vector3 vertexGlobal = vertex + centrePos + oceanPos;

            // DEBUG
            if(i == 0)
            {
                // Debug.Log(vertexGlobal + " " + vertexGlobalTest);
            }

            // Get the water height at this coordinate
            vertex.y = WaterController.instance.GetWaveYPos(vertexGlobal, timeSinceStart);

            vertices[i] = vertex;
        }

        terrainMeshFilter.mesh.vertices = vertices;
        terrainMeshFilter.mesh.RecalculateNormals();
    }


    public void GenerateMesh()
    {
        List<Vector3[]> verts = new List<Vector3[]>();

        // Triangle list. 
        List<int> tris = new List<int>();

        // Uvs
        // List<Vector2> uvs = new List<Vector2>();

        for (int z = 0; z < width; z++)
        {
            verts.Add(new Vector3[width]);
            for (int x = 0; x < width; x++)
            {
                Vector3 currentPoint = new Vector3();

                // Get the coordinates of the vertices;
                currentPoint.x = x * spacing;
                currentPoint.z = z * spacing;
                currentPoint.y = squareTransform.position.y;

                verts[z][x] = currentPoint;

                //UVs.Add(new Vector2(x, z));

                // Dont generate a triangle the first coordinate on each row.
                // Because thats just one point. 

                if (x <= 0 || z <= 0)
                {
                    continue;
                }
                // Create the quad.
                // each square consists of 2 triangles
                // The triangle south-west of the vertice
                tris.Add(x + z * width);                        //          
                tris.Add(x + (z - 1) * width);                  //          
                tris.Add((x - 1) + (z - 1) * width);            //          
                                                                //          
                // the trianle west-south of the vertice        //          
                tris.Add(x + z * width);                        //          
                tris.Add((x - 1) + (z - 1) * width);            //          
                tris.Add((x - 1) + z * width);                  //          
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
        Mesh newMesh = new Mesh();
        newMesh.vertices = unfoldedVerts;
        // newMesh.uv = uvs.ToArray();
        newMesh.triangles = tris.ToArray();

        // Ensure the bounding volume is correct.
        newMesh.RecalculateBounds();

        // Update the normals to reflect the change. 
        newMesh.RecalculateNormals();

        // Add the generated mesh to this GameObject.
        terrainMeshFilter.mesh.Clear();
        terrainMeshFilter.mesh = newMesh;
        terrainMeshFilter.mesh.name = "Water Mesh";

        print(terrainMeshFilter.mesh.vertices.Length);
    }
}
