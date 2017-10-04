using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// https://gamedevelopment.tutsplus.com/tutorials/creating-dynamic-2d-water-effects-in-unity--gamedev-14143
// 
public class WaterManager : MonoBehaviour
{
    private float[] xPositions;
    private float[] yPositions;
    private float[] velocities;
    private float[] accelerations;

    // Hold vertices for the surface of the water.
    private LineRenderer body;

    // Hold data for the body of water.
    private GameObject[] meshObjects;
    private Mesh[] meshes;

    // Colliders so things can interact with the water. 
    private GameObject[] colliders;

    const float SPRING_CONST = 0.02f;
    const float DAMPING = 0.04f;
    const float SPREAD = 0.05f;
    const float Z = -1f; // Z-Order of the water. 

    // Dimentions of the water. 
    private float baseHeight;
    private float left;
    private float bottom;

    public GameObject splash;
    public Material mat;
    public GameObject waterMesh;





    private void Start()
    {
        SpawnWater(-10, 20, 0, -10);
    }



    private void FixedUpdate()
    {
        /*Applying Physics */
        // Hookes Law: F = kx
        // F - force produced by spring
        // k - spring constant
        // x - is the displacement.

        // add damping factor to dampen the force. 

        for (int i = 0; i < xPositions.Length; i++)
        {
            float force = SPRING_CONST * (yPositions[i] - baseHeight) + velocities[i] * DAMPING;
            accelerations[i] = -force;
            yPositions[i] += velocities[i];
            velocities[i] += accelerations[i];
            body.SetPosition(i, new Vector3(xPositions[i], yPositions[i], Z));
            // Euler method: just add the acceleration to the velocity and the velocity to the position every frame. 
            accelerations[i] = -force / 1; // 1 is mass
        }


        float[] leftDeltas = new float[xPositions.Length];
        float[] rightDeltas = new float[xPositions.Length];

        for(int j = 0; j < 8; j++)
        {
            for(int i = 0; i < xPositions.Length; i++)
            {
                if(i > 0)
                {
                    leftDeltas[i] = SPREAD * (yPositions[i] - yPositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if(i < xPositions.Length - 1)
                {
                    rightDeltas[i] = SPREAD * (yPositions[i] - yPositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }
        }


        for(int i = 0; i < xPositions.Length; i++)
        {
            if(i > 0)
            {
                yPositions[i - 1] += leftDeltas[i];
            }
            if(i < xPositions.Length - 1)
            {
                yPositions[i + 1] += rightDeltas[i];
            }
        }


    }

    public void SpawnWater(float left, float width, float top, float bottom)
    {
        var edgeCount = Mathf.RoundToInt(width) * 5;
        var nodeCount = edgeCount + 1;

        // Select the material and set it to render above the water. 
        body = gameObject.AddComponent<LineRenderer>();
        body.material = mat;
        body.material.renderQueue = 1000;
        body.positionCount = nodeCount;
        body.startWidth = 0.1f;
        body.endWidth = 0.1f;

        xPositions = new float[nodeCount];
        yPositions = new float[nodeCount];
        velocities = new float[nodeCount];
        accelerations = new float[nodeCount];

        meshObjects = new GameObject[edgeCount];
        meshes = new Mesh[edgeCount];
        colliders = new GameObject[edgeCount];

        baseHeight = top;
        this.bottom = bottom;
        this.left = left;


        // set all ypos to top of the water. Incrementallly add all the nodes side by side. Our
        // velocities and accelecations are zero initially as the water is still.
        for (int i = 0; i < nodeCount; i++)
        {
            yPositions[i] = top;
            xPositions[i] = left + width * i / edgeCount;
            accelerations[i] = 0;
            velocities[i] = 0;

            // setting each node in LineRenderer to their correct position.
            body.SetPosition(i, new Vector3(xPositions[i], yPositions[i], Z));
        }


        /* CREATING THE MESH */
        for (int i = 0; i < edgeCount; i++)
        {
            meshes[i] = new Mesh();

            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(xPositions[i], yPositions[i], Z);
            vertices[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], Z);
            vertices[2] = new Vector3(xPositions[i], bottom, Z);
            vertices[3] = new Vector3(xPositions[i + 1], bottom, Z);

            Vector2[] texCoords = new Vector2[4];
            texCoords[0] = new Vector2(0, 1);
            texCoords[1] = new Vector2(1, 1);
            texCoords[2] = new Vector2(0, 0);
            texCoords[3] = new Vector2(1, 0);

            // triangle points. 
            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            // create the quad.
            meshes[i].vertices = vertices;
            meshes[i].uv = texCoords;
            meshes[i].triangles = tris;

            // Create the mesh and then set it as the child of the watermanager.
            meshObjects[i] = Instantiate(waterMesh, Vector3.zero, Quaternion.identity) as GameObject;
            meshObjects[i].GetComponent<MeshFilter>().mesh = meshes[i];
            meshObjects[i].transform.parent = transform;

            /*Create Our Collisions */
            colliders[i] = new GameObject();
            colliders[i].name = "Trigger";
            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].transform.parent = transform;
            colliders[i].transform.position = new Vector3(left + width * (i + 0.5f) / edgeCount, top - 0.5f, 0);
            colliders[i].transform.localScale = new Vector3(width / edgeCount, 1, 1);
            colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            // colliders[i].AddComponent<WaterDetector>();
        }
    }

    /*
    Update the water as it moves. 
    */
    private void UpdateMeshes()
    {
        for(int i = 0; i < meshes.Length; i++)
        {
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(xPositions[i], yPositions[i], Z);
            vertices[1] = new Vector3(xPositions[i + 1], yPositions[i + 1], Z);
            vertices[2] = new Vector3(xPositions[i], bottom, Z);
            vertices[3] = new Vector3(xPositions[i + 1], bottom, Z);

            meshes[i].vertices = vertices;
        }
    }
}
