using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EndlessWaterSquare : MonoBehaviour
{
    public GameObject boatObj;
    public GameObject waterSquareObj;

    // Water square data
    private float squareWidth = 800f;
    private float innerSquareResolution = 5f;
    private float outerSquareResolution = 25f;

    List<WaterSquare> waterSquares = new List<WaterSquare>();

    // For Thread
    private float secondsSinceStart;
    private Vector3 boatPos;

    // The position of the ocean has to be updated in the thread because it follows
    //  the boat. Is not the same as pos of boat because it moves with the same resolution as the smallest water square resolution
    private Vector3 oceanPos;

    private bool hasThreadUpdatedWater;

    private void Start()
    {
        CreateEndlessSea();

        secondsSinceStart = Time.time;

        //ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));

        //StartCoroutine(UpdateWater());
    }


    private void Update()
    {
        UpdateWaterNoThread();

        secondsSinceStart = Time.time;

        boatPos = boatObj.transform.position;
    }


    private void UpdateWaterNoThread()
    {
        boatPos = boatObj.transform.position;

        MoveWaterToBoat();

        transform.position = oceanPos;

       for(int i = 0; i < waterSquares.Count; i++)
       {
           waterSquares[i].MoveSea(oceanPos, Time.time);
       }
    }

    //The loop that gives the updated vertices from the thread to the meshes
    //which we can't do in its own thread
    private IEnumerator UpdateWater()
    {
        while(true)
        {
            // has the thread finsihed updating the water
            if(hasThreadUpdatedWater)
            {
                // Move the water to the boat
                transform.position = oceanPos;

                // Add the updated vertices to the water meshes
                for(int i = 0; i < waterSquares.Count; i++)
                {
                    waterSquares[i].terrainMeshFilter.mesh.vertices = waterSquares[i].vertices;
                    waterSquares[i].terrainMeshFilter.mesh.RecalculateNormals();
                }

                // stop looping until we have updated the water in the thread
                hasThreadUpdatedWater = false;

                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));

            }

            /// Dont need to update the water every frame. 
            yield return new WaitForSeconds(Time.deltaTime * 3f);
        }
    }


    private void UpdateWaterWithThreadPooling(object state)
    {
        MoveWaterToBoat();

        // Loop through all the water squares. 
        for(int i = 0; i < waterSquares.Count; i++)
        {
            // the local centre pos of this square
            Vector3 centrePos = waterSquares[i].centrePos;
            Vector3[] vertices = waterSquares[i].vertices;
            
            // Update the vertices in this square
            for(int j = 0; j < vertices.Length; j++)
            {
                Vector3 vertexPos = vertices[j];

                //Can't use transformpoint in a thread, so to find the global position of the vertex
                //we just add the position of the ocean and the square because rotation and scale is always 0 and 1
                Vector3 vertexPosGlobal = vertexPos + centrePos + oceanPos;

                // Get the water height
                vertexPos.y = WaterController.instance.GetWaveYPos(vertexPosGlobal, secondsSinceStart);

                // Save the new y coordinate but x and z are still in local position
                vertices[j] = vertexPos;
            }
        }
        hasThreadUpdatedWater = true;
    }


    private void MoveWaterToBoat()
    {
        // Round to nearest resolution
        float x = innerSquareResolution * (int)Mathf.Round(boatPos.x / innerSquareResolution);
        float z = innerSquareResolution * (int)Mathf.Round(boatPos.z / innerSquareResolution);

        // Should we move the water?
        if(oceanPos.x != x || oceanPos.z != z)
        {
            oceanPos = new Vector3(x, oceanPos.y, z);
        }
    }


    private void CreateEndlessSea()
    {
        // the centre piece
        AddWaterPlane(0f, 0f, 0f, squareWidth, innerSquareResolution);

        // The 8 squares around the centre square. 
        for(int x = -1; x <= 1; x += 1)
        {
            for(int z = -1; z <= 1; z += 1)
            {
                // ignore the centre square
                if (x == 0 && z == 0)
                {
                    continue;
                }

                // the y-pos should be lower than the square with high res to avoid an ugly seam
                float yPos = -0.5f;
                AddWaterPlane(x * squareWidth, z * squareWidth, yPos, squareWidth, outerSquareResolution);
            }
        }
    }



    private void AddWaterPlane(float xCoord, float zCoord, float yPos, float squareWidth, float spacing)
    {
        var waterPlane = Instantiate(waterSquareObj, transform.position, transform.rotation) as GameObject;

        waterPlane.SetActive(true);

        // change its position
        Vector3 centrePos = transform.position;
        centrePos.x += xCoord;
        centrePos.y = yPos;
        centrePos.z += zCoord;

        waterPlane.transform.position = centrePos;

        // parent it.
        waterPlane.transform.parent = transform;

        // give it moving water properties and set its width and resolution to generate the water mesh.
        WaterSquare tempWaterSquare = new WaterSquare(waterPlane, squareWidth, spacing);
        waterSquares.Add(tempWaterSquare);





    }




}
