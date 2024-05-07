using UnityEngine;

public class RArmMov : MonoBehaviour
{
    private Vector3 sensorDataEulerAngles;

    void OnEnable()
    {
        UnityServer.OnRightArmSensorDataReceived += HandleSensorDataReceived;
    }

    void OnDisable()
    {
        UnityServer.OnRightArmSensorDataReceived -= HandleSensorDataReceived;
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
        newSensorEulerAngles.x, // Roll stays the same
        -newSensorEulerAngles.y, // Invert Pitch
        -newSensorEulerAngles.z  // Invert Yaw
    );

        Debug.Log("Sensor Euler angles: " + sensorDataEulerAngles);
    }
}
