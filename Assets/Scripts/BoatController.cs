using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoatController : MonoBehaviour
{
    public Rigidbody boatRigidbody { get { return GetComponent<Rigidbody>(); } }
     public List<GameObject> boyancyAids = new List<GameObject>();
    public WaveController waveController;
    public float waterDensity = 1027f;
    public float distanceToWaterSurface;
    public Vector3 currentWaveVertexPosition;
    private float boatHeight       ;
    private float waveHeight       ;




    private void Update()
    {
        currentWaveVertexPosition = waveController.GetVertexPosition(transform.position);

        boatHeight = this.transform.position.y;
        waveHeight = currentWaveVertexPosition.y;

        distanceToWaterSurface = Vector3.Distance(this.transform.position, currentWaveVertexPosition);

        BasicFloating();
       if (waveHeight < boatHeight)
        {
            
            Debug.DrawLine(this.transform.position, currentWaveVertexPosition, Color.red);

        }
        else
        {
            Debug.DrawLine(this.transform.position, currentWaveVertexPosition, Color.yellow);
            // Let gravity do its bit.
        }


        if(distanceToWaterSurface < 1 && distanceToWaterSurface > -1)
        {
            distanceToWaterSurface = 0f;
        }


        
    }
    
    private void BasicFloating()
    {
        boatHeight = Mathf.MoveTowards(boatHeight, waveHeight, 500f * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, boatHeight, transform.position.z);
    }

    
    private Vector3 BouyancyForce()
    {
        Bounds boatBounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        float cubeFaceSurfaceArea = (boatBounds.size.x * boatBounds.size.z);

        Vector3 force = distanceToWaterSurface * Physics.gravity.y * cubeFaceSurfaceArea * Vector3.up;
        force.x = 0f;
        force.z = 0f;

        return force;

    }
}
