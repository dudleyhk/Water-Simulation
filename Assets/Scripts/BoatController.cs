using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoatController : MonoBehaviour
{
    public Rigidbody boatRigidbody { get { return GetComponent<Rigidbody>(); } }
     public List<GameObject> boyancyAids = new List<GameObject>();
    public WaveController waveController;

    // have 8 boyancy aids places at each corner of the boat. 
    // every second the aids will tell the boat if it is above or below the waters surface. 


    private void Update()
    {
        Vector3 pos = waveController.GetVertexPosition(transform.position);

        var boatHeight = this.transform.position.y;
        var waveHeight = pos.y;

        if (waveHeight > boatHeight)
        {
            Debug.DrawLine(this.transform.position, pos, Color.red);
            // Apply force up.
            boatRigidbody.AddForce(Vector3.up * 100f, ForceMode.Acceleration);
        }
        else
        {
            Debug.DrawLine(this.transform.position, pos, Color.yellow);
            // Let gravity do its bit.
        }
    }

}
