using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

    static void Main(string[] args)
    {
        // Path to your Unity executable
        string unityPath = @"D:\git_492\ECE-492---3D-animation-using-IMU-data\unity_project\My Project.exe";

        ProcessStartInfo startInfo = new ProcessStartInfo(unityPath);
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = true;

        Process unityProcess = Process.Start(startInfo);

        // Wait for the Unity process to create a window
        System.Threading.Thread.Sleep(1000);

        // Hide the window
        ShowWindow(unityProcess.MainWindowHandle, SW_HIDE);

        Console.WriteLine("Unity process started and hidden.");
        Console.ReadLine(); // Keep the console window open
    }
}