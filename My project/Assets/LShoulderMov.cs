/*using System;
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
            transform.localRotation = Quaternion.Euler(roll, pitch, yaw);
            lineIndex = lineIndex + 1;

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
}
*//*


using UnityEngine;

public class LShoulderMov : MonoBehaviour
{
    private Vector3 initialPosition;
    private float offsetX, offsetY, offsetZ;
    private Vector3 offset;
    private bool isOffsetSet = false;
    private Vector3 sensorData; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnLeftShoulderSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnLeftShoulderSensorDataReceived -= HandleSensorDataReceived;
    }

    void Start()
    {
        initialPosition = transform.localEulerAngles;
        initialPosition = NormalizeAngles(initialPosition);
    }

    void Update()
    {
        Quaternion sensorRotation = Quaternion.Euler(sensorData);
        Quaternion offsetRotation = Quaternion.Euler(offset);
        transform.localRotation = sensorRotation * offsetRotation;
    }


    void HandleSensorDataReceived(string bodyPart, Vector3 newSensorData)
    {
        // Convert the sensor data to match Unity's coordinate system
        // Negate the pitch axis to invert the up and down movements
        Vector3 convertedSensorData = new Vector3(-newSensorData.y, -newSensorData.z, newSensorData.x);

        // Store the received sensor data
        sensorData = convertedSensorData;
        if (!isOffsetSet)
        {
            // Calculate the offset based on the initial position and the first sensor data
            offset = initialPosition - sensorData;
            isOffsetSet = true;
            *//*Debug.Log("OFFFFFFFFFFFFFFFFFFSEEEEEEEEEEEEEEEEEETTTTTTTTTTTTTTT 22222222222222222");*//*
        }
        *//*Debug.Log("Sensor data: " + sensorData);*//*
    }


    Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
}
*//*



using UnityEngine;

public class LShoulderMov : MonoBehaviour
{
    private Vector3 initialPosition;
    private float offsetX, offsetY, offsetZ;
    private bool isOffsetSet = false;
    private Vector3 sensorData; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnLeftShoulderSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnLeftShoulderSensorDataReceived -= HandleSensorDataReceived;
    }

    void Start()
    {
        Debug.Log("Sensor dataaaaaaaaaaaaaaaaaaaaaaaaaaaa: " + sensorData);
        initialPosition = transform.localEulerAngles;

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
    }

    void Update()
    {
        transform.localRotation = Quaternion.Euler(sensorData.x - offsetX, sensorData.y - offsetY, sensorData.z - offsetZ);
       *//* transform.localRotation = Quaternion.Euler(-sensorData.x, -sensorData.y, -sensorData.z);*//*
    }

    void HandleSensorDataReceived(string bodyPart, Vector3 newSensorData)
    {
        // Convert the sensor data to match Unity's coordinate system
        Vector3 convertedSensorData = new Vector3(newSensorData.y, newSensorData.z, newSensorData.x);

        // Store the received sensor data
        sensorData = convertedSensorData;
        if (!isOffsetSet)
        {
            offsetX = initialPosition.x - sensorData.x;
            offsetY = initialPosition.y - sensorData.y;
            offsetZ = initialPosition.z - sensorData.z;
            isOffsetSet = true;
        }
        Debug.Log("Sensor data: " + sensorData);
    }
}
*/