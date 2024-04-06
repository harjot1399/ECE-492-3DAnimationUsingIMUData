using System;
using System.IO;
using UnityEngine;

public class RShoulderMov : MonoBehaviour
{
    // Start is called before the first frame update
    private string[] lines;
    private int lineIndex;
    private string currentPosition;
    private string tempCurrentPosition;
    private Vector3 initialPosition;
    private float initialRoll, initialPitch, initialYaw;
    private float offsetX, offsetY, offsetZ;

    void Start()
    {
        lines = File.ReadAllLines("datalog_10.txt");
        initialPosition = transform.localEulerAngles;

        lineIndex = 0;

        tempCurrentPosition = lines[0];
        var temp_parts = tempCurrentPosition.Split(new char[] { ',', ':', '|' });

        initialRoll = float.Parse(temp_parts[7]);
        initialPitch = float.Parse(temp_parts[9]);
        initialYaw = float.Parse(temp_parts[11]);

        if (initialPosition.x > 180)
        {
            initialPosition.x = -(360 - initialPosition.x);
        }
        if (initialPosition.y > 180)
        {
            initialPosition.y = -(360 - initialPosition.y);
        }
        if (initialPosition.z > 180)
        {
            initialPosition.z = -(360 - initialPosition.z);
        }

        offsetX = initialRoll - initialPosition.x;
        offsetY = initialPitch - initialPosition.y;
        offsetZ = initialYaw - initialPosition.z;
    }

    // Update is called once per frame
    async void Update()
    {
        try
        {
            currentPosition = lines[lineIndex];
            var parts = currentPosition.Split(new char[] { ',', ':', '|' });

            float roll = float.Parse(parts[7]);
            float pitch = float.Parse(parts[9]);
            float yaw = float.Parse(parts[11]);

            transform.localRotation = Quaternion.Euler(roll - offsetX, pitch - offsetY, yaw - offsetZ);
/*            transform.localRotation = Quaternion.Euler(roll, pitch, yaw);
*/            lineIndex = lineIndex + 1;

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
}
