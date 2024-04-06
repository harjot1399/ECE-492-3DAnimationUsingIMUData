/*using UnityEngine;

public class LForearmMov : MonoBehaviour
{
    private Vector3 initialPosition;
    private float offsetX, offsetY, offsetZ;
    private Vector3 offset;
    private bool isOffsetSet = false;
    private Vector3 sensorData; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnLeftForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnLeftForearmSensorDataReceived -= HandleSensorDataReceived;
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
        }
      *//*  Debug.Log("Sensor data: " + sensorData);*//*
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

*/



/*using UnityEngine;

public class LForearmMov : MonoBehaviour
{
    private Vector3 initialPosition;
    private float offsetX, offsetY, offsetZ;
    private bool isOffsetSet = false;
    private Vector3 sensorData; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnLeftForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnLeftForearmSensorDataReceived -= HandleSensorDataReceived;
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
        *//*transform.localRotation = Quaternion.Euler(-sensorData.x, -sensorData.y, -sensorData.z);*//*
    }

    void HandleSensorDataReceived(string bodyPart, Vector3 newSensorData)
    {
        // Convert the sensor data to match Unity's coordinate system
        Vector3 convertedSensorData = new Vector3(newSensorData.x, newSensorData.y, newSensorData.z);

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
}*/


using UnityEngine;

public class LForearmMov : MonoBehaviour
{
    private Quaternion initialRotation;
    private Quaternion offsetQuaternion;
    private bool isOffsetSet = false;
    private Quaternion sensorDataQuaternion; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnLeftForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnLeftForearmSensorDataReceived -= HandleSensorDataReceived;
    }

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        if (isOffsetSet)
        {
            transform.localRotation = offsetQuaternion * sensorDataQuaternion;
        }
    }

    void HandleSensorDataReceived(string bodyPart, Quaternion newSensorQuaternion)
    {
        // Store the received sensor data
        sensorDataQuaternion = newSensorQuaternion;

        if (!isOffsetSet)
        {
            offsetQuaternion = Quaternion.Inverse(sensorDataQuaternion) * initialRotation;
            isOffsetSet = true;
        }

        Debug.Log("Sensor quaternion: " + sensorDataQuaternion);
    }
}
