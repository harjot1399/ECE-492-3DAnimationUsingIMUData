/*using System;
using System.IO;
using UnityEngine;

public class RForearmMov : MonoBehaviour
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

        initialRoll = float.Parse(temp_parts[1]);
        initialPitch = float.Parse(temp_parts[3]);
        initialYaw = float.Parse(temp_parts[5]);

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

            float roll = float.Parse(parts[1]);
            float pitch = float.Parse(parts[3]);
            float yaw = float.Parse(parts[5]);

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

public class RForearmMov : MonoBehaviour
{
    private Vector3 initialPosition;
    private float offsetX, offsetY, offsetZ;
    private bool isOffsetSet = false;
    private Vector3 sensorData; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
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
        Vector3 convertedSensorData = new Vector3(-newSensorData.y, -newSensorData.z, newSensorData.x);

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


*//*Transform GetBodyPartTransform(string bodyPart)
{
    // This method returns the corresponding Transform for a given body part identifier.
    // Add logic here to return the correct Transform based on the body part name.
    switch (bodyPart)
    {
        case "leftForearm":
            return leftForearm;
        case "rightForearm":
            return rightForearm;
        case "leftLeg":
            return leftLeg;
        case "rightLeg":
            return rightLeg;
        // Add more cases for additional body parts
        default:
            Debug.LogWarning($"Unknown body part: {bodyPart}");
            return null;
    }
}*/
/*}*/



/*using UnityEngine;

public class RForearmMov : MonoBehaviour
{
    private Vector3 initialPosition;
    private float offsetX, offsetY, offsetZ;
    private Vector3 offset;
    private bool isOffsetSet = false;
    private Vector3 sensorData; // Variable to store the latest sensor data

    void OnEnable()
    {
        UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
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
*/

// using UnityEngine;

// public class RForearmMov : MonoBehaviour
// {
//     private Quaternion initialRotation;
//     private Quaternion offsetQuaternion;
//     private bool isOffsetSet = false;
//     private Quaternion sensorDataQuaternion; // Variable to store the latest sensor data

//     void OnEnable()
//     {
//         // Subscribe to the right forearm sensor data event
//         UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
//     }

//     void OnDisable()
//     {
//         // Unsubscribe from the right forearm sensor data event
//         UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
//     }

//     void Start()
//     {
//         initialRotation = transform.localRotation;
//     }

//     void Update()
//     {
//         if (isOffsetSet)
//         {
//             transform.localRotation = offsetQuaternion * sensorDataQuaternion;
//         }
//     }

//     void HandleSensorDataReceived(string bodyPart, Quaternion newSensorQuaternion)
//     {
//         // Store the received sensor data
//         sensorDataQuaternion = newSensorQuaternion;

//         if (!isOffsetSet)
//         {
//             // Calculate the offset quaternion based on the initial and received sensor data
//             offsetQuaternion = Quaternion.Inverse(sensorDataQuaternion) * initialRotation;
//             isOffsetSet = true;
//         }

//         Debug.Log("Sensor quaternion: " + sensorDataQuaternion);
//     }
// }


// using UnityEngine;

// public class RForearmMov : MonoBehaviour
// {
//     private Quaternion initialRotation;
//     private Quaternion offsetQuaternion;
//     private bool isOffsetSet = false;
//     private Quaternion sensorDataQuaternion; // Variable to store the latest sensor data

//     void OnEnable()
//     {
//         UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
//     }

//     void OnDisable()
//     {
//         UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
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


// using UnityEngine;

// public class RForearmMov : MonoBehaviour
// {
//     private Vector3 sensorDataEulerAngles;

//     void OnEnable()
//     {
//         UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
//     }

//     void OnDisable()
//     {
//         UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
//     }


//     void Update()
//     {
//         Vector3 targetEulerAngles = sensorDataEulerAngles;
//         Vector3 newEulerAngles = new Vector3(targetEulerAngles.y, targetEulerAngles.x, targetEulerAngles.z);

//         transform.localEulerAngles = newEulerAngles;
//     }

//     void HandleSensorDataReceived(string bodyPart, Vector3 newSensorEulerAngles)
//     {
//          sensorDataEulerAngles = new Vector3(
//         newSensorEulerAngles.x, // Roll stays the same
//         -newSensorEulerAngles.y, // Invert Pitch
//         -newSensorEulerAngles.z  // Invert Yaw
//     );

//         Debug.Log("Sensor Euler angles: " + sensorDataEulerAngles);
//     }
// }


using UnityEngine;

public class RForearmMov : MonoBehaviour
{
    private Vector3 sensorDataEulerAngles;

    // Set these limits to the desired min and max rotation for each axis
    public float minRoll = -45f; // Min roll limit
    public float maxRoll = 45f;  // Max roll limit
    public float minPitch = -45f; // Min pitch limit
    public float maxPitch = 45f;  // Max pitch limit
    public float minYaw = -45f;   // Min yaw limit
    public float maxYaw = 45f;    // Max yaw limit

    void OnEnable()
    {
        UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
    }

    void Update()
    {
        // Clamp the sensor data to limit the rotation range
        float clampedRoll = Mathf.Clamp(sensorDataEulerAngles.x, minRoll, maxRoll);
        float clampedPitch = Mathf.Clamp(-sensorDataEulerAngles.y, minPitch, maxPitch); // Inverting pitch
        float clampedYaw = Mathf.Clamp(-sensorDataEulerAngles.z, minYaw, maxYaw); // Inverting yaw

        // Apply the clamped angles to the forearm
        Vector3 newEulerAngles = new Vector3(clampedPitch, clampedRoll, clampedYaw);
        transform.localEulerAngles = newEulerAngles;
    }

    void HandleSensorDataReceived(string bodyPart, Vector3 newSensorEulerAngles)
    {
        sensorDataEulerAngles = newSensorEulerAngles;
        Debug.Log("Sensor Euler angles: " + sensorDataEulerAngles);
    }
}
