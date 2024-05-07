using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;



public class SendFrames : MonoBehaviour
{
    public Camera cameraToCapture;
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private RenderTexture renderTexture;
    private Texture2D texture2D;
    private int width = 1920;
    private int height = 1080;
    private bool clientConnected = false;

    void Start()
    {
        renderTexture = new RenderTexture(width, height, 24);
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
        server.Start();
        Debug.Log("Server started.");
        
        AcceptClient();
    }

    async void AcceptClient()
    {
        client = await server.AcceptTcpClientAsync();
        stream = client.GetStream();
        clientConnected = true;
        Debug.Log("Client connected.");
    }

    void Update()
    {
        if (clientConnected)
        {
            CaptureAndSendFrame();
        }
    }

    void CaptureAndSendFrame()
{
    cameraToCapture.targetTexture = renderTexture;
    cameraToCapture.Render();
    RenderTexture.active = renderTexture;
    texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    texture2D.Apply();

    // Encode the texture to a JPG byte array
    byte[] bytes = texture2D.EncodeToJPG();

    // Convert the byte array to a Base64 string
    string base64String = Convert.ToBase64String(bytes);

    // Convert the Base64 string to a byte array. This is necessary because the NetworkStream
    // writes byte arrays, not strings. Additionally, consider appending a unique delimiter
    // to the Base64 string before converting it to a byte array if you plan to send multiple
    // frames consecutively. The delimiter will help the client-side code determine where one
    // frame ends and the next begins.
    byte[] base64Bytes = System.Text.Encoding.UTF8.GetBytes(base64String + "<EOF>");

    // Check if the client is still connected before attempting to send data
    if (client.Connected)
    {
        // Send the Base64-encoded frame
        stream.Write(base64Bytes, 0, base64Bytes.Length);
    }
    else
    {
        Debug.Log("Client disconnected.");
    }

    cameraToCapture.targetTexture = null;
    RenderTexture.active = null;
}

   void sendVideo()
{
    cameraToCapture.targetTexture = renderTexture;
    cameraToCapture.Render();
    RenderTexture.active = renderTexture;
    texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    texture2D.Apply();

    // Encode the texture to a JPG byte array
    byte[] bytes = texture2D.EncodeToJPG();

    // Convert the byte array to a Base64 string
    string base64String = Convert.ToBase64String(bytes);

    // Convert the Base64 string to a byte array. This is necessary because the NetworkStream
    // writes byte arrays, not strings. Additionally, consider appending a unique delimiter
    // to the Base64 string before converting it to a byte array if you plan to send multiple
    // frames consecutively. The delimiter will help the client-side code determine where one
    // frame ends and the next begins.
    byte[] base64Bytes = System.Text.Encoding.UTF8.GetBytes(base64String + "<EOF>");

    // Check if the client is still connected before attempting to send data
    if (client.Connected)
    {
        // Send the Base64-encoded frame
        stream.Write(base64Bytes, 0, base64Bytes.Length);
    }
    else
    {
        Debug.Log("Client disconnected.");
    }

    cameraToCapture.targetTexture = null;
    RenderTexture.active = null;
}


    void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }
        if (server != null)
        {
            server.Stop();
        }
        Destroy(renderTexture);
        Destroy(texture2D);
    }
}
