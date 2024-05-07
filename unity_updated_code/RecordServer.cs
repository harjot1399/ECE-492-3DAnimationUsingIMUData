using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RecordServer : MonoBehaviour
{
    private HttpListener listener;
    private const string URL = "http://localhost:8082/";
    private bool isRunning = false;
    public string vid_location;
    private bool isRecording = false;
    private int frameCounter = 0;

    [Serializable]
    public class PostData
    {
        public string directory;
        public string command;
    }

    // Path to the Python executable
    // private string pythonInterpreter = @"C:\Users\ikashyap\AppData\Local\Programs\Python\Python310\python.exe";
    // Path to the Python script that merges images into a video
    // private string scriptPath = @"C:\Users\ikashyap\Downloads\x64-20240309T220505Z-001\videomaker.py";

    void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add(URL);
        listener.Start();
        isRunning = true;
        Debug.Log("Server started at " + URL);
        listener.BeginGetContext(OnRequestReceived, listener);
    }

    private void OnRequestReceived(IAsyncResult result)
    {
        if (!isRunning) return;

        var context = listener.EndGetContext(result);
        var request = context.Request;
        var response = context.Response;

        if (request.HttpMethod == "POST")
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string receivedData = reader.ReadToEnd();
                if (receivedData.StartsWith("{") && receivedData.EndsWith("}"))
                {
                    PostData postData = JsonUtility.FromJson<PostData>(receivedData);
                    if(postData.directory != null)
                    {
                        vid_location = postData.directory;
                        Debug.Log("Received directory: " + vid_location);
                    }
                    if (postData.command != null)
                    {
                        switch (postData.command)
                        {
                            case "start":
                                if (!isRecording)
                                {
                                    DeleteImageFiles(vid_location);
                                    isRecording = true;
                                    frameCounter = 0;
                                    Debug.Log("Start capturing frames");
                                }
                                break;
                            case "stop":
                                if (isRecording)
                                {
                                    isRecording = false;
                                    Debug.Log("Stop capturing frames");
                                }
                                break;
                            case "dispose":
                                if (!isRecording) // Ensure we don't discard while recording
                                {
                                    // Discard the frames captured so far
                                    DeleteImageFiles(vid_location);
                                    Debug.Log("Dispose");
                                }
                                break;
                            case "save":
                                if (!isRecording)
                                {
                                    DeleteImageFiles(vid_location);
                                }
                                break;
                        }
                    }
                    else if (postData.directory != null)
                    {
                        vid_location = postData.directory;
                        Debug.Log("Received directory: " + vid_location);
                    }
                }
            }
        }

        string responseString = "OK";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();

        listener.BeginGetContext(OnRequestReceived, listener);
    }

    void Update()
    {
        if (isRecording)
        {
            StartCoroutine(CaptureFrame());
        }
    }

    private IEnumerator CaptureFrame()
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        byte[] bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        string filename = Path.Combine(vid_location, $"frame_{frameCounter:D04}.png");
        File.WriteAllBytes(filename, bytes);
        frameCounter++;
    }

    // private void CreateVideoFromFrames(string directory)
    // {
    //     Debug.Log("Creating video from frames...");
    //     // ExecutePythonScript(directory);
    //     DeleteImageFiles(directory);
    // }
    private void DeleteImageFiles(string directory)
    {
        try
        {
            // Get all png files in the directory
            Debug.Log("hello hello" + directory);
            string[] pngFiles = Directory.GetFiles(directory, "*.png");
            foreach (string file in pngFiles)
            {
                File.Delete(file);  // Delete the file
            }
            Debug.Log("All PNG files have been deleted.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error deleting PNG files: " + ex.Message);
        }
    }

    // private void ExecutePythonScript(string directory)
    // {
    //     ProcessStartInfo psi = new ProcessStartInfo();
    //     psi.FileName = pythonInterpreter;
    //     psi.Arguments = $"\"{scriptPath}\" \"{directory}\"";
    //     psi.UseShellExecute = false;
    //     psi.CreateNoWindow = true;
    //     psi.RedirectStandardOutput = true;
    //     psi.RedirectStandardError = true;

    //     Process process = Process.Start(psi);
    //     process.WaitForExit();

    //     string output = process.StandardOutput.ReadToEnd();
    //     string error = process.StandardError.ReadToEnd();

    //     Debug.Log("Output: " + output);
    //     if (!string.IsNullOrEmpty(error))
    //     {
    //         Debug.LogError("Error: " + error);
    //     }
    // }

    void OnDestroy()
    {
        if (listener != null)
        {
            isRunning = false;
            listener.Close();
        }
    }
}
