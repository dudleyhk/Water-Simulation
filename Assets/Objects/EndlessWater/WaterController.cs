using System.Collections;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    public static WaterController instance;
    public bool isMoving;

    // Wave height and speed
    public float scale = 0.1f;
    public float speed = 1.0f;

    // the width between the waves 
    public float waveDistance = 1f;

    // noise param
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;


    private void Start()
    {
        instance = this;
    }


    // Get the y coord from whatever wavetype we are using. 
    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        //if (isMoving)
        //{
        //    return WaveTypes.SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
        //}
        //else
        //{
        //    return 0f;
        //}

        return 0f;
    }


    //Find the distance from a vertice to water
    //Make sure the position is in global coordinates
    //Positive if above water
    //Negative if below water
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);
        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }
}