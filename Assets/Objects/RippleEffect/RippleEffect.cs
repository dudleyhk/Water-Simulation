using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleEffect : MonoBehaviour 
{
	private Mesh mesh { get { return GetComponent<Mesh> (); } }

	// Update is called once per frame
	void Update () 
	{
		mesh.RecalculateNormals ();
	}
}
