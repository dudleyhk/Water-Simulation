using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaveController : MonoBehaviour
{
    public Slider mainWavesHeightSlider;
    public Slider mainWavesLengthSlider;
    public Slider windWavesHeightSlider;
    public Slider windWavesLengthSlider;
    public Slider windWavesSpeedSlider; 
    public Slider windWavesSteepnessSlider;

    public MeshFilter waterMeshFilter;
    public Material waveMaterial;

    public float   gravity;
    public Vector3 waveDirection;
    public Vector3 waveAmplitude;
    public Vector3 waveLength;
    public Vector3 boostWaveDirection;

    public float   windWaveLength;
    public float   windWaveAmplitude;
    public float   windWaveSpeed;
    public float   windWaveSteepness;
    public Vector3 windWaveDirection;



    private readonly float defaultGravity              = 9.8f;
    private readonly Vector3 defaultWaveLength         = new Vector3(0.01f, 1f, 0.01f);
    private readonly Vector3 defaultWaveAmplitude      = new Vector3(0.1f, 10f, 0.1f);
    private readonly Vector3 defaultWaveDirection      = new Vector3(0.1f, 0f, 0.5f);
    private readonly Vector3 defaultBoostWaveDirection = new Vector3(0.9f, 0f, 0.5f);
            
    private readonly float defaultWindWaveLength      = 10f;
    private readonly float defaultWindWaveAmplitude   = 5f;
    private readonly float defaultWindWaveSpeed       = 5f;
    private readonly float defaultWindWaveSteepness   = 0.25f;
    private readonly Vector3 defaultWindWaveDirection = new Vector3(1f, 0f, 1f);

    private Vector2 vertexDirection;


    private void Start()
    {
        waterMeshFilter.mesh.MarkDynamic();
        ResetWaves();
    }


    private void Update()
    {
        SetWaveValues();
        GetWaveValues();
    }


    private void SetWaveValues()
    {
        waveLength.y      = mainWavesLengthSlider.value;
        waveAmplitude.y   = mainWavesHeightSlider.value;
        windWaveLength    = windWavesLengthSlider.value;
        windWaveAmplitude = windWavesHeightSlider.value;
        windWaveSpeed     = windWavesSpeedSlider.value;
        windWaveSteepness = windWavesSteepnessSlider.value;

        
        waveMaterial.SetFloat ("_WaterTime", Time.time);
        waveMaterial.SetFloat ("_Gravity",             gravity);
        waveMaterial.SetVector("_Wave_Direction",      waveDirection);
        waveMaterial.SetVector("_BoostWave_Direction", boostWaveDirection);
        waveMaterial.SetVector("_Wave_Amplitude",      waveAmplitude);
        waveMaterial.SetVector("_Wave_Length",         waveLength);
        waveMaterial.SetFloat ("_WindWave_Length",    windWaveLength);
        waveMaterial.SetFloat ("_WindWave_Amplitude", windWaveAmplitude);
        waveMaterial.SetFloat ("_WindWave_Speed",     windWaveSpeed);
        waveMaterial.SetFloat ("_WindWave_Steepness", windWaveSteepness);
        waveMaterial.SetVector("_WindWave_Direction", windWaveDirection);
    }


    private void GetWaveValues()
    {
        gravity            = waveMaterial.GetFloat ("_Gravity");
        waveDirection      = waveMaterial.GetVector("_Wave_Direction");
        waveAmplitude      = waveMaterial.GetVector("_Wave_Amplitude");
        waveLength         = waveMaterial.GetVector("_Wave_Length");
        boostWaveDirection = waveMaterial.GetVector("_BoostWave_Direction");

        windWaveLength     = waveMaterial.GetFloat ("_WindWave_Length");
        windWaveAmplitude  = waveMaterial.GetFloat ("_WindWave_Amplitude");
        windWaveSpeed      = waveMaterial.GetFloat ("_WindWave_Speed");
        windWaveSteepness  = waveMaterial.GetFloat ("_WindWave_Steepness");
        windWaveDirection  = waveMaterial.GetVector("_WindWave_Direction");
    }



    public void ResetWaves()
    {
        gravity            = defaultGravity;
        waveDirection      = defaultWaveDirection; 
        waveAmplitude      = defaultWaveAmplitude;
        waveLength         = defaultWaveLength; 
        boostWaveDirection = defaultBoostWaveDirection;

        windWaveLength     = defaultWindWaveLength;
        windWaveAmplitude  = defaultWindWaveAmplitude;
        windWaveSpeed      = defaultWindWaveSpeed;
        windWaveSteepness  = defaultWindWaveSteepness;
        windWaveDirection  = defaultWindWaveDirection;


        mainWavesLengthSlider.value = waveLength.y;
        mainWavesHeightSlider.value = waveAmplitude.y;
        windWavesLengthSlider.value = windWaveLength;
        windWavesHeightSlider.value = windWaveAmplitude;
        windWavesSpeedSlider.value  = windWaveSpeed;
        windWavesSteepnessSlider.value = windWaveSteepness;
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

        vertexDirection = new Vector2(vertex.x, vertex.z);
        var windDirection = new Vector2(windDirection1.x, windDirection1.z);

        /* Gerstner Calculations */
        float waveX1 = (((windDirection1 / magnitudeX1) * amplitudeX1) * Mathf.Sin(Vector2.Dot(windDirection, vertexDirection) - (freqX1 * (Time.time)))).x;
        float waveY1 = amplitudeY1 * Mathf.Cos(Vector2.Dot(windDirection, vertexDirection) - (freqY1 - Time.time));
        float waveZ1 = (((windDirection1 / magnitudeZ1) * amplitudeZ1) * Mathf.Sin(Vector2.Dot(windDirection, vertexDirection) - (freqZ1 * (Time.time)))).z;
        // ----------------------


        float freq = 2 / 1;
        float phase = (2f * 0.1f) / 1f;
        float pinch = 0.1f / (freq * 0.1f);
        float boosterWave = 0.1f * Mathf.Cos((freq * Vector2.Dot(vertexDirection, new Vector2(boostWaveDirection.x, boostWaveDirection.z))) + (phase * Time.time));


        /* Set values */
        float X = vertex.x - waveX1;
        float Y = (waveY1 * boosterWave) * BlowWind(vertex);
        float Z = vertex.z - waveZ1;
        // ----------------------

        return new Vector3(X, Y, Z);
    }


    private float BlowWind(Vector3 vertex)
    {
        float freq = 2f / windWaveLength;
        float phase = (2 * windWaveSpeed) / windWaveLength;
        float pinch = windWaveSteepness / (freq * windWaveAmplitude);
        float dir = Vector2.Dot(new Vector2(windWaveDirection.x, windWaveDirection.z), vertexDirection);

        float wave = (pinch * windWaveAmplitude) * Mathf.Cos((dir - (freq - Time.time)) * phase);
        return wave;
    }
}












