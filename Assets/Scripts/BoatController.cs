using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoatController : MonoBehaviour
{
    public Rigidbody boatRigidbody { get { return GetComponent<Rigidbody>(); } }
    public WaveController waveController;
    public float waterDensity = 1027f;
    public float depth;
    public Vector3 currentWaveVertexPosition;

    private float boatHeight;
    private float waveHeight;



    private void FixedUpdate()
    {
        if (waveHeight > boatHeight)
        {
            ApplyBuoyancyForce();
        }
    }


    private void Update()
    {
        currentWaveVertexPosition = waveController.GetVertexPosition(transform.position);
        depth = Vector3.Distance(this.transform.position, currentWaveVertexPosition);
        DebugLines();
    }
    


    private void DebugLines()
    {
        boatHeight = this.transform.position.y;
        waveHeight = currentWaveVertexPosition.y;

        if (waveHeight < boatHeight)
        {
            Debug.DrawLine(this.transform.position, currentWaveVertexPosition, Color.red);
            depth = -depth;
        }
        else
        {
            depth = Mathf.Abs(depth);
            Debug.DrawLine(this.transform.position, currentWaveVertexPosition, Color.yellow);
        }
    }
    
    private void ApplyBuoyancyForce()
    {
        if(float.IsNaN(depth)) return;

        Bounds boatBounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        float cubeFaceSurfaceArea = (boatBounds.size.x * boatBounds.size.z);

        float force = waterDensity * Mathf.Abs(Physics.gravity.y) * (cubeFaceSurfaceArea * 2f);
        var buoyancyForce = (force * depth) * Time.deltaTime;

        boatRigidbody.AddForceAtPosition(new Vector3(0f, buoyancyForce, 0f), transform.position, ForceMode.Force);
    }
}
