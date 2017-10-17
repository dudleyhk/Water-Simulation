using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create a small cube and check if any of the waters vertices are inside the bounds. 
/// </summary>
public class BoyancyAid : MonoBehaviour
{
    public MeshFilter boatMeshFilter;
    public GameObject waterObj;
    public List<Vector3> pointsInBounds = new List<Vector3>();
    public float timer = 0f;
    public float maxTime = 5f;

    private Bounds bounds;
    private Color boundsColor = Color.red;
    private Vector3 frontTopLeft;
    private Vector3 frontTopRight;
    private Vector3 frontBottomLeft;
    private Vector3 frontBottomRight;
    private Vector3 backTopLeft;
    private Vector3 backTopRight;
    private Vector3 backBottomLeft;
    private Vector3 backBottomRight;


    private void Start()
    {
        bounds = new Bounds(Vector3.zero, transform.lossyScale);
        waterObj = GameObject.FindGameObjectWithTag("Water");
    }


    private void Update()
    {


        CalculatePositions();
        DrawBounds();
        if (timer > maxTime)
        {
            CheckVertices();
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }



    private void CalculatePositions()
    {
        bounds.center = transform.position;

        Vector3 boundsCentre = bounds.center;
        Vector3 boundsExtents = bounds.extents;

        // calc the positoins based on bounds.
        frontTopLeft = new Vector3(boundsCentre.x - boundsExtents.x, boundsCentre.y + boundsExtents.y, boundsCentre.z - boundsExtents.z);
        frontTopRight = new Vector3(boundsCentre.x + boundsExtents.x, boundsCentre.y + boundsExtents.y, boundsCentre.z - boundsExtents.z);
        frontBottomLeft = new Vector3(boundsCentre.x - boundsExtents.x, boundsCentre.y - boundsExtents.y, boundsCentre.z - boundsExtents.z);
        frontBottomRight = new Vector3(boundsCentre.x + boundsExtents.x, boundsCentre.y - boundsExtents.y, boundsCentre.z - boundsExtents.z);
        backTopLeft = new Vector3(boundsCentre.x - boundsExtents.x, boundsCentre.y + boundsExtents.y, boundsCentre.z + boundsExtents.z);
        backTopRight = new Vector3(boundsCentre.x + boundsExtents.x, boundsCentre.y + boundsExtents.y, boundsCentre.z + boundsExtents.z);
        backBottomLeft = new Vector3(boundsCentre.x - boundsExtents.x, boundsCentre.y - boundsExtents.y, boundsCentre.z + boundsExtents.z);
        backBottomRight = new Vector3(boundsCentre.x + boundsExtents.x, boundsCentre.y - boundsExtents.y, boundsCentre.z + boundsExtents.z);
    }


    private void CheckVertices()
    {
        pointsInBounds.Clear();
        Vector3[] verticesToCheck = waterObj.GetComponent<MeshFilter>().sharedMesh.vertices;
        foreach (var vert in verticesToCheck)
        {
            Vector3 pos = waterObj.transform.TransformPoint(vert);
            if(bounds.Contains(pos))
            {
                pointsInBounds.Add(pos);
                print("Point in bound: " + pos);
            }
        }
    }


    // Methods for Drawing Debug-lines and Gizmos
    void DrawBounds()
    {
        Debug.DrawLine(frontTopLeft, frontTopRight, boundsColor);
        Debug.DrawLine(frontTopRight, frontBottomRight, boundsColor);
        Debug.DrawLine(frontBottomRight, frontBottomLeft, boundsColor);
        Debug.DrawLine(frontBottomLeft, frontTopLeft, boundsColor);

        Debug.DrawLine(backTopLeft, backTopRight, boundsColor);
        Debug.DrawLine(backTopRight, backBottomRight, boundsColor);
        Debug.DrawLine(backBottomRight, backBottomLeft, boundsColor);
        Debug.DrawLine(backBottomLeft, backTopLeft, boundsColor);

        Debug.DrawLine(frontTopLeft, backTopLeft, boundsColor);
        Debug.DrawLine(frontTopRight, backTopRight, boundsColor);
        Debug.DrawLine(frontBottomRight, backBottomRight, boundsColor);
        Debug.DrawLine(frontBottomLeft, backBottomLeft, boundsColor);
    }
}
