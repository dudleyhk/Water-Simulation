//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WaveController : MonoBehaviour
//{
//    public MeshFilter waterMeshFilter;

//    public float gravity;
//    public Vector3 waveDirection;
//    public Vector3 waveAmplitude;
//    public Vector3 waveLength;


//    private void Update()
//    {
//        gravity       = Shader.GetGlobalFloat("_Gravity");
//        waveDirection = Shader.GetGlobalVector("_Wave_Direction");
//        waveAmplitude = Shader.GetGlobalVector("_Wave_Amplitude");
//        waveLength    = Shader.GetGlobalVector("_Wave_Length");
//    }



//    private Vector3 BlowWind()
//    {
//        return Vector3.zero;
//    }


//    /// <summary>
//    /// Pass in the vertex you want to get teh height of. 
//    /// Apply roughtly the same equation to it.
//    /// </summary>
//    /// <param name="verts"></param>
//    public void CalcWavePos(Vector3[] verts)
//    {
//        foreach (var vert in verts)
//        {
//            Vector3 windDirection1 = waveDirection;
//            Vector3 windDirection2 = new Vector3(1f, 0f, 0.25f);

//            /* Amplitudes */
//            float amplitudeX1 = waveAmplitude.x;
//            float amplitudeY1 = waveAmplitude.y;
//            float amplitudeZ1 = waveAmplitude.z;
//            // ----------------------


//            /* Wave Length */
//            float waveLengthX1 = waveLength.x;
//            float waveLengthY1 = waveLength.y;
//            float waveLengthZ1 = waveLength.z;
//            // ----------------------


//            /* Magnitudes */
//            float magnitudeX1 = (2f * Mathf.PI) / waveLengthX1;
//            float magnitudeY1 = (2f * Mathf.PI) / waveLengthY1;
//            float magnitudeZ1 = (2f * Mathf.PI) / waveLengthZ1;
//            // ----------------------



//            /* Frequencis */
//            float freqX1 = Mathf.Sqrt(gravity * magnitudeX1);
//            float freqY1 = Mathf.Sqrt(gravity * magnitudeY1);
//            float freqZ1 = Mathf.Sqrt(gravity * magnitudeZ1);
//            // ----------------------


//            /* Gerstner Calculations */
//            float waveX1 = (windDirection1 / magnitudeX1) * amplitudeX1 * Mathf.Sin(Vector3.Dot(windDirection1, vert) - (freqX1 * (Time.de / 20)));
//            float waveY1 = amplitudeY1 * Mathf.Cos(Vector3.Dot(windDirection1, vert) - (freqY1 - Time.timeScale));
//            float waveZ1 = (windDirection1 / magnitudeZ1) * amplitudeZ1 * Mathf.Sin(dot(windDirection1, worldPos.z) - (freqZ1 * (Time.time / 20)));
//            // ----------------------


//            /* Totals */
//            float totalX = waveX1;
//            float totalY = waveY1 * (blowWind(worldPos));
//            float totalZ = waveZ1;
//            // ----------------------

//            /* Set values */
//            float X = worldPos.x - totalX;
//            float Y = totalY;
//            float Z = worldPos.z - totalZ;
//            // ----------------------
//        }
//    }





//}






















