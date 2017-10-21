using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public MeshFilter waterMeshFilter;
    public Material waveMaterial;

    public float gravity;
    public Vector3 waveDirection;
    public Vector3 waveAmplitude;
    public Vector3 waveLength;

    public float windWaveScalar;
    public float windWaveLength;
    public float windWaveAmplitude;
    public float windWaveSpeed;
    public float windWaveSteepness;
    public Vector3 windWaveDirection;

    private void Start()
    {
        waterMeshFilter.mesh.MarkDynamic();
    }

    private void Update()
    {
         
        gravity       = waveMaterial.GetFloat("_Gravity");
        waveDirection = waveMaterial.GetVector("_Wave_Direction");
        waveAmplitude = waveMaterial.GetVector("_Wave_Amplitude");
        waveLength    = waveMaterial.GetVector("_Wave_Length");


        windWaveScalar    = waveMaterial.GetFloat("_WindWave_Scalar");
        windWaveLength    = waveMaterial.GetFloat("_WindWave_Length");
        windWaveAmplitude = waveMaterial.GetFloat("_WindWave_Amplitude");
        windWaveSpeed     = waveMaterial.GetFloat("_WindWave_Speed");
        windWaveSteepness = waveMaterial.GetFloat("_WindWave_Steepness");
        windWaveDirection = waveMaterial.GetVector("_WindWave_Direction");

        waveMaterial.SetFloat("_WaterTime", Time.time);
    }


    private Vector3 BlowWind()
    {
        return Vector3.zero;
    }


    /// <summary>
    /// Pass in the vertex you want to get teh height of. 
    /// Apply roughtly the same equation to it.
    /// </summary>
    /// <param name="verts"></param>
    public Vector3 GetVertexPosition(Vector3 vertex)
    {
        Vector3 windDirection1 = waveDirection;


        /* Amplitudes */
        float amplitudeX1 = waveAmplitude.x;
        float amplitudeY1 = waveAmplitude.y;
        float amplitudeZ1 = waveAmplitude.z;
        // ----------------------


        /* Wave Length */
        float waveLengthX1 = waveLength.x;
        float waveLengthY1 = waveLength.y;
        float waveLengthZ1 = waveLength.z;
        // ----------------------


        /* Magnitudes */
        float magnitudeX1 = (2f * Mathf.PI) / waveLengthX1;
        float magnitudeY1 = (2f * Mathf.PI) / waveLengthY1;
        float magnitudeZ1 = (2f * Mathf.PI) / waveLengthZ1;
        // ----------------------



        /* Frequencis */
        float freqX1 = Mathf.Sqrt(gravity * magnitudeX1);
        float freqY1 = Mathf.Sqrt(gravity * magnitudeY1);
        float freqZ1 = Mathf.Sqrt(gravity * magnitudeZ1);
        // ----------------------

        var vertexDirection = new Vector2(vertex.x, vertex.z);
        var windDirection = new Vector2(windDirection1.x, windDirection1.z);

        /* Gerstner Calculations */
        float waveX1 = ((windDirection1 / magnitudeX1) * amplitudeX1 * Mathf.Sin(Vector2.Dot(windDirection, vertexDirection) - (freqX1 * (Time.time)))).x;
        float waveY1 = amplitudeY1 * Mathf.Cos(Vector2.Dot(windDirection, vertexDirection) - (freqY1 - Time.time));
        float waveZ1 = ((windDirection1 / magnitudeZ1) * amplitudeZ1 * Mathf.Sin(Vector2.Dot(windDirection, vertexDirection) - (freqZ1 * (Time.time)))).x;
        // ----------------------

       //print("WaveX: " + waveX1);
       //print("WaveY: " + waveY1);
       //print("WaveZ: " + waveZ1);


        /* Set values */
        float X = vertex.x - waveX1;
        float Y = waveY1 * BlowWind(vertex).x;
        float Z = vertex.z - waveZ1;
        // ----------------------


        //Vector3 vertexLocalSpace = transform.InverseTransformDirection(new Vector3(X, Y, Z));
        //print("Position of vertex in question " + new Vector3(X, Y, Z));
        return new Vector3(X, Y, Z);
    }


    private Vector3 BlowWind(Vector3 vertex)
    {
        float freq  = 2 / windWaveLength;
        float phase = (2 * windWaveSpeed) / windWaveLength;
        float pinch = windWaveSteepness / (freq * windWaveAmplitude);

        Vector2 vertexDirection = new Vector2(vertex.x, vertex.z);
        Vector2 windDirection   = new Vector2(windWaveDirection.x, windWaveDirection.z);
        float dir = Vector2.Dot(vertexDirection, windDirection);

        float X = vertex.x + (pinch * windWaveAmplitude) * (windWaveDirection.x * Mathf.Cos(freq * dir + phase * (Time.time * windWaveScalar)));
        float Y = windWaveAmplitude * Mathf.Sin(freq * dir + phase * Time.time * windWaveScalar);
        float Z = vertex.z + (pinch * windWaveAmplitude) * (windWaveDirection.z * Mathf.Cos(freq * dir + phase * (Time.time * windWaveScalar)));


        return new Vector3(X, Y, Z);
    }
}












