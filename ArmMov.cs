using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ArmMov : MonoBehaviour
{
    // public Transform rightArm;
    public Transform rightForearm;
    private Queue<Vector3> sensorDataQueue = new Queue<Vector3>();
    private Vector3 currentRotation, targetRotation, initialRotation, initialOffset;

    private float sensorDataInterval = 0.05f; // Adjust this to match the sensor data recording rate
    private float timeSinceLastUpdate = 0;

    void Start()
    {
        string filePath = "datalog.txt";
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            Vector3 sensorData = ParseSensorData(line);
            sensorDataQueue.Enqueue(sensorData);
        }

        initialRotation = sensorDataQueue.Peek();
        initialOffset = initialRotation - (initialRotation - transform.localEulerAngles);

  /*      if (sensorDataQueue.Count > 0)
        {
            currentRotation = sensorDataQueue.Dequeue();
            if (sensorDataQueue.Count > 0)
            {
                targetRotation = sensorDataQueue.Dequeue();
            }
        }*/
    }

    void Update()
    {
        if (rightForearm != null && sensorDataQueue.Count > 0)
        {
            timeSinceLastUpdate += Time.deltaTime;

            if (timeSinceLastUpdate >= sensorDataInterval)
            {
                //currentRotation = targetRotation;
                targetRotation = sensorDataQueue.Dequeue() - initialOffset;
                timeSinceLastUpdate = 0;
            }

            // Interpolate between currentRotation and targetRotation
            //float lerpFactor = timeSinceLastUpdate / sensorDataInterval;
            //Vector3 interpolatedRotation = Vector3.Lerp(currentRotation-initialOffset, targetRotation-initialOffset, 1);
            rightForearm.localRotation = Quaternion.Euler(targetRotation);
        }
    }

    Vector3 ParseSensorData(string dataLine)
    {
        string[] parts = dataLine.Split(new char[] { ':', ',', '|' }, System.StringSplitOptions.RemoveEmptyEntries);
        float roll = float.Parse(parts[1]);
        float pitch = float.Parse(parts[3]);
        float yaw = float.Parse(parts[5]);
        return new Vector3(pitch, yaw, roll);
    }
}
