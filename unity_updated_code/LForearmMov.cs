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


// using UnityEngine;

// public class LForearmMov : MonoBehaviour
// {
//     private Quaternion initialRotation;
//     private Quaternion offsetQuaternion;
//     private bool isOffsetSet = false;
//     private Quaternion sensorDataQuaternion; // Variable to store the latest sensor data

//     void OnEnable()
//     {
//         UnityServer.OnLeftForearmSensorDataReceived += HandleSensorDataReceived;
//     }

//     void OnDisable()
//     {
//         UnityServer.OnLeftForearmSensorDataReceived -= HandleSensorDataReceived;
//     }

//     void Start()
//     {
//         initialRotation = transform.localRotation;
//     }

//     [SerializeField] private float smoothFactor = 0.1f; // Adjust this value to control the smoothing

//     void Update()
//     {
//         if (isOffsetSet)
//         {
//             Quaternion targetRotation = offsetQuaternion * sensorDataQuaternion;
//             transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothFactor);
//         }
//     }


//     void HandleSensorDataReceived(string bodyPart, Quaternion newSensorQuaternion)
// {
//     // Assuming newSensorQuaternion is already in Unity's coordinate system
//     if (!isOffsetSet)
//     {
//         // Calculate the offset once when the first piece of data is received
//         // This assumes the sensor is in the desired starting orientation at this point
//         offsetQuaternion = Quaternion.Inverse(newSensorQuaternion) * initialRotation;
//         isOffsetSet = true;
//     }
//     else
//     {
//         // Apply the offset to incoming sensor data
//         // This adjusts the sensor data to the model's initial orientation
//         sensorDataQuaternion = offsetQuaternion * newSensorQuaternion;
//     }

//     Debug.Log("Sensor quaternion: " + sensorDataQuaternion);
// }

// }



using UnityEngine;

public class LForearmMov : MonoBehaviour
{
    private Vector3 sensorDataEulerAngles;

    void OnEnable()
    {
        UnityServer.OnLeftForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnLeftForearmSensorDataReceived -= HandleSensorDataReceived;
    }


    void Update()
    {
        Vector3 targetEulerAngles = sensorDataEulerAngles;
        
        Vector3 newEulerAngles = new Vector3(targetEulerAngles.y, targetEulerAngles.x, targetEulerAngles.z);

        transform.localEulerAngles = newEulerAngles;
    }

    void HandleSensorDataReceived(string bodyPart, Vector3 newSensorEulerAngles)
    {
        sensorDataEulerAngles = new Vector3(
        -newSensorEulerAngles.x, // Roll stays the same
        newSensorEulerAngles.y, // Invert Pitch
        newSensorEulerAngles.z  // Invert Yaw
    );

        Debug.Log("Sensor Euler angles: " + sensorDataEulerAngles);
    }
}