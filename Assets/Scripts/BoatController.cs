//TODO: remove unused Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// - The BoatContoller class would be better as a few Systems that 
///     handle updating the Boat data and applying bouyancy physics IE 
///     BoatPhysicsSystem class which runs through a list of the 
///     IBoatControllerPhysics objects. 
/// - Create IBoatPhysics interface which has a Rigidbody property. 
/// - The BoatController would become a wrapper for the cached Transform and 
///     Rigidbody components.
/// - Create Data strcuture for WaterDensity and other global data.
/// 
/// Notes on GetVertexPosition:
/// - The current solution requires a reference to the WaveController which 
///      is something I want to avoid so the BoatController and WaveController 
///      systems can run independently. In addition this calculation is a copy 
///      of what is happening in the WaveShader.shader.
/// - There are two alternative solutions I think would be good to solve the 
///     issue of getting the height of a vertex in the Ocean/ Wave Mesh.
/// - The first, which is similar to the current method of copying the 
///     calculation from the shader but instead run it on-demand in a Job that 
///     lives in a BoatPhysicsSystem (MonoBehaviour). This 
///     would be incrediably fast and could pull Ocean information for the 
///     calculation from the OceanData class. However, any changes to the calcuation 
///     in the Shader would have to be copied to the Job. This creates a weak link in that 
///     whoever changes the Shader has to have knowledge of the VertexPosition 
///     function otehrwise it might not be changed.
/// - The second solution is to pass a texture into the WaveShader and use it 
///     to create a height map which can be referenced in the BoatController 
///     class or Data class, and read from when needed. In this instance the 
///     only Data that would be needed in the OceanData class would be the Texture. 
///     This option means the same calculation will be shared (removing the weaklink)
///     and has the speed advantage of running the calulation on the GPU. 
///     It does have the overhead of rquiring a texture which may not scale well 
///     if the Ocean is very large and there are many Boats.
/// </summary>
public class BoatController : MonoBehaviour
{
    // TODO: Cache component in private variable.
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
        // TODO: Cache the transform component.
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
