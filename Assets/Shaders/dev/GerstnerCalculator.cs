using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerstnerCalculator : MonoBehaviour
{
    public Vector3 currentVertexPos;

    public Vector3 windDirection1;
    public Vector3 windDirection2;

    public float amplitude1;
    public float amplitude2;

    public float waveLength1;
    public float waveLength2;


    public float gravity = 9.8f;



    private void Update ()
    {
        currentVertexPos = Shader.GetGlobalVector("_CurrentVertexPos");

        float magnitude1 = (2 * Mathf.PI) / waveLength1;
        float magnitude2 = (2 * Mathf.PI) / waveLength2;

        float freq1 = Mathf.Sqrt(gravity * magnitude1);
        float freq2 = Mathf.Sqrt(gravity * magnitude2);


        float waveX1 = (windDirection1.x / magnitude1) * amplitude1 * Mathf.Sin(Vector3.Dot(windDirection1, new Vector3(currentVertexPos.x, 0, 0)) - (freq1 * Time.time));
        float waveX2 = (windDirection2.x / magnitude2) * amplitude2 * Mathf.Sin(Vector3.Dot(windDirection2, new Vector3(currentVertexPos.x, 0, 0)) - (freq2 * Time.time));

        float waveZ1 = (windDirection1.z / magnitude1) * amplitude1 * Mathf.Sin(Vector3.Dot(windDirection1, new Vector3(0, 0, currentVertexPos.z)) - (freq1 * Time.time));
        float waveZ2 = (windDirection2.z / magnitude2) * amplitude2 * Mathf.Sin(Vector3.Dot(windDirection2, new Vector3(0, 0, currentVertexPos.z)) - (freq2 * Time.time));


        float waveY1 = amplitude1 * Mathf.Cos(Vector3.Dot(new Vector3(windDirection1.x, 0, windDirection1.z), new Vector3(currentVertexPos.x, 0, currentVertexPos.z)) - (freq1 - Time.time));
        float waveY2 = amplitude2 * Mathf.Cos(Vector3.Dot(new Vector3(windDirection2.x, 0, windDirection2.z), new Vector3(currentVertexPos.x, 0, currentVertexPos.z)) - (freq2 - Time.time));

        float totalX = waveX1 + waveX2;
        float totalY = waveY1 + waveY2;
        float totalZ = waveZ1 + waveZ2;



        Shader.SetGlobalVector("_GerstnerWave", new Vector3(totalX, totalY, totalZ));



    }
}


/*
 // k - Magnitude of wave. 
			float magnitude = (2 * 3.1416) / _WaveLength;

			// K - direction wind is travelling. 
			float3 windDirection = _WaveDirection;


			float oceanDepth = 10;


			// w
			float freq =  sqrt(9.8 * magnitude); // sqrt(9.8 * magnitude) * tanh(magnitude * oceanDepth);// 9.8 gravitational pull, deep water sim


			float phase = (2 * _WaveSpeed) / _WaveLength;

			// Simple no loop. 
			float X = worldPos.x - (windDirection / magnitude) * _Amplitude * sin(dot(windDirection, worldPos.x) - (freq * _Time.x) + phase);
			float Y = _Amplitude * cos(dot(windDirection.xz, worldPos.xz) - (freq - _Time.y) + phase);
			float Z = worldPos.z - (windDirection / magnitude) * _Amplitude * sin(dot(windDirection, worldPos.z) - (freq * _Time.x) + phase);
     
     
     */
