using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UnityServer : MonoBehaviour
{

    // public static bool isDataHere = false;
    public static event Action<string, Vector3> OnLeftForearmSensorDataReceived;
    public static event Action<string, Vector3> OnRightForearmSensorDataReceived;
    public static event Action<string, Vector3> OnLeftArmSensorDataReceived;
    public static event Action<string, Vector3> OnRightArmSensorDataReceived;
    public static event Action<string, Vector3> OnLeftLegSensorDataReceived;
    public static event Action<string, Vector3> OnRightLegSensorDataReceived; // Adjusted to include body part identifier
    private TcpListener tcpListener;
    private const int Port = 8081;

    void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        Debug.Log("Server started on port " + Port + ".");
        tcpListener.BeginAcceptTcpClient(OnClientConnected, null);
    }

    private void OnClientConnected(IAsyncResult ar)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(ar);
        Debug.Log("Client connected.");
        NetworkStream stream = client.GetStream();
        BeginRead(stream);
    }

    private void BeginRead(NetworkStream stream)
    {
        // Create a new buffer for each read operation
        byte[] buffer = new byte[1024];
        stream.BeginRead(buffer, 0, buffer.Length, ar =>
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead > 0)
            {
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // isDataHere = true;
                Debug.Log($"Raw data received: {receivedData}");
                ProcessData(receivedData);

               /* // Clear the buffer
                Array.Clear(buffer, 0, buffer.Length);*/

                // Continue reading data from the stream
                BeginRead(stream);
            }
        }, null);
    }

    private void ProcessData(string data)
     {
         string[] readings = data.Split('|', StringSplitOptions.RemoveEmptyEntries);
         foreach (var reading in readings)
         {
             string[] parts = reading.Trim().Split(' ');
             //*if (parts.Length < 5) continue; // Expected format: Roll: X.XX, Pitch: X.XX, Yaw: X.XX bodyPart*//*

             string bodyPart = parts[^1]; // Last part is the body part identifier
             Vector3 sensorData = ParseSensorData(string.Join(" ", parts, 0, parts.Length - 1));

             // Log the parsed Vector3 data to the console
             //*Debug.Log($"{bodyPart} Sensor Data: {sensorData}");*//*

             // Trigger the appropriate event based on the body part
             if(bodyPart.Equals("leftForearm", StringComparison.OrdinalIgnoreCase))
             {
                 OnLeftForearmSensorDataReceived?.Invoke("leftForearm", sensorData);
             }
             else if (bodyPart.Equals("rightForearm", StringComparison.OrdinalIgnoreCase))
             {
                 OnRightForearmSensorDataReceived?.Invoke("rightForearm", sensorData);
             }
             else if (bodyPart.Equals("leftArm", StringComparison.OrdinalIgnoreCase))
             {
                 OnLeftArmSensorDataReceived?.Invoke("leftArm", sensorData);
             }
             else if (bodyPart.Equals("rightArm", StringComparison.OrdinalIgnoreCase))
             {
                 OnRightArmSensorDataReceived?.Invoke("rightArm", sensorData);
             }
             else if (bodyPart.Equals("leftLeg", StringComparison.OrdinalIgnoreCase))
             {
                 OnLeftLegSensorDataReceived?.Invoke("leftLeg", sensorData);
             }
             else if (bodyPart.Equals("rightLeg", StringComparison.OrdinalIgnoreCase))
             {
                 OnRightLegSensorDataReceived?.Invoke("rightLeg", sensorData);
             }
         }
     }


    // private void ProcessData(string data)
    // {
    //     string[] readings = data.Split('|', StringSplitOptions.RemoveEmptyEntries);
    //     foreach (var reading in readings)
    //     {
    //         string[] parts = reading.Trim().Split(' ');
    //         // Expecting format: W: X.XX, X: X.XX, Y: X.XX, Z: X.XX bodyPart

    //         string bodyPart = parts[^1]; // Get the body part identifier
    //         Quaternion sensorQuaternion = ParseSensorData(string.Join(" ", parts, 0, parts.Length - 1));

    //         Debug.Log($"{bodyPart} Sensor Data: {sensorQuaternion}");

    //         // Trigger the appropriate event based on the body part
    //         switch (bodyPart.ToLower())
    //         {
    //             case "leftforearm":
    //                 OnLeftForearmSensorDataReceived?.Invoke("leftForearm", sensorQuaternion);
    //                 break;
    //             case "rightforearm":
    //                 OnRightForearmSensorDataReceived?.Invoke("rightForearm", sensorQuaternion);
    //                 break;
    //             case "leftshoulder":
    //                 OnLeftShoulderSensorDataReceived?.Invoke("leftShoulder", sensorQuaternion);
    //                 break;
    //             case "rightshoulder":
    //                 OnRightShoulderSensorDataReceived?.Invoke("rightShoulder", sensorQuaternion);
    //                 break;
    //             case "leftleg":
    //                 OnLeftLegSensorDataReceived?.Invoke("leftLeg", sensorQuaternion);
    //                 break;
    //             case "rightleg":
    //                 OnRightLegSensorDataReceived?.Invoke("rightLeg", sensorQuaternion);
    //                 break;
    //         }
    //     }
    // }




    private Vector3 ParseSensorData(string dataLine)
    {
        string[] parts = dataLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
        float roll = 0, pitch = 0, yaw = 0;

        if (parts.Length == 3)
        {
            float.TryParse(parts[0].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out roll);
            float.TryParse(parts[1].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out pitch);
            float.TryParse(parts[2].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out yaw);
        }

        Vector3 parsedData = new Vector3(roll, pitch, yaw); 

        return parsedData;
    }

    // private Quaternion ParseSensorData(string dataLine)
    // {
    //     string[] parts = dataLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
    //     float w = 0, x = 0, y = 0, z = 0;

    //     if (parts.Length == 4)
    //     {
    //         float.TryParse(parts[0].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out w);
    //         float.TryParse(parts[1].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out x);
    //         float.TryParse(parts[2].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out y);
    //         float.TryParse(parts[3].Split(':')[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out z);
    //     }

    //     Quaternion parsedData = new Quaternion(w, x, y, z);
    //     Debug.Log($"Parsed Sensor Data: {parsedData}");

    //     return parsedData;
    // }



    void OnDestroy()
    {
        if (tcpListener != null) tcpListener.Stop();
    }
}