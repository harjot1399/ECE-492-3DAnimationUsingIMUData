using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Unity.LiveCapture;

namespace Movella.Xsens
{
    class XsensClient
    {
        public bool IsStarted => m_Thread is { IsAlive: true };
        
        public FrameRate FrameRate { get; set; } = StandardFrameRate.FPS_24_00;

        public bool IsConnected { get; private set; }
        public int Port { get; private set; }

        public event Func<int, FrameData, bool> FrameDataReceivedAsync;
        public event Action Disconnected;

        enum StreamingProtocol
        {
            SPPoseEuler = 1,
            SPPoseQuaternion = 2,
            SPPosePositions = 3,
            SPTagPositionsLegacy = 4,
            SPPoseUnity3D = 5,
            SPMetaScalingLegacy = 10,
            SPMetaPropInfoLegacy = 11,
            SPMetaMoreMeta = 12,
            SPMetaScaling = 13,
            SPTimecode = 25,
        };

        Thread m_Thread;
        UdpClient m_Client;
        (Timecode tc, FrameRate rate) m_Timecode;

        FrameData[] m_Frames = new FrameData[XsensConstants.NumStreams];
        object[] m_FrameMutexes = new object[XsensConstants.NumStreams];

        public XsensClient()
        {
            for (int i = 0; i < XsensConstants.NumStreams; i++)
                m_FrameMutexes[i] = new object(); 
        }

        public void Connect(int port)
        {
            if (port <= 0 || port > 0xFFFF)
            {
                Debug.LogError($"{nameof(XsensClient)}: Tried to connect with an invalid port: {port}");
                return;
            }

            if (!IsConnected)
            {
                try
                {
                    m_Thread = new Thread(() => ListenForDataAsync(port));
                    m_Thread.IsBackground = true;
                    m_Thread.Start();
                }
                catch (Exception e)
                {
                    m_Thread = null;
                    IsConnected = false;

                    Debug.Log($"{nameof(XsensClient)}({port}): Thread start exception {e}");
                }
            }
        }

        public void Disconnect()
        {
            IsConnected = false;

            if (m_Thread != null)
            {
                if (!m_Thread.Join(2000))
                    m_Thread.Abort();

                m_Thread = null;
            }

            m_Client?.Close();
            m_Client = null;

            Disconnected?.Invoke();
        }

        public FrameData GetFrame(int characterID)
        {
            if (IsConnected &&
                characterID >= 0 &&
                characterID < XsensConstants.NumStreams)
            {
                lock (m_FrameMutexes[characterID])
                    return m_Frames[characterID];
            }

            return default;
        }

        void ListenForDataAsync(int port)
        {
            try 
            {
                Port = port;

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

                m_Client = new UdpClient(port);

                IsConnected = true;

                while (IsConnected)
                {
                    var receiveBytes = m_Client.Receive(ref endPoint);

                    string result = "";
                    result += (char)receiveBytes[4];
                    result += (char)receiveBytes[5];
                    StreamingProtocol packId = (StreamingProtocol)int.Parse(result);

                    switch (packId)
                    {
                        case StreamingProtocol.SPPoseQuaternion:
                        {
                            if (receiveBytes.Length > 15)
                            {
                                int characterID = receiveBytes[16];

                                if (characterID >= 0 && characterID < XsensConstants.NumStreams)
                                {
                                    var frame = ParsePacket(receiveBytes);

                                    if (!(FrameDataReceivedAsync?.Invoke(characterID, frame) ?? false))
                                    {
                                        lock (m_FrameMutexes[characterID])
                                            m_Frames[characterID] = frame;
                                    }
                                }
                            }
                            break;
                        }

                        case StreamingProtocol.SPTimecode:
                        {
                            // 12 byte string formatted as such HH:MM:SS.mmm
                            // MVN strings are UTF-8 encoded
                            if (receiveBytes.Length > 35)
                            {
                                var timecode = Encoding.UTF8.GetString(receiveBytes[28..39]);

                                if (DateTime.TryParse(timecode, out var timestamp))
                                {
                                    var totalSeconds = timestamp.Hour * 3600 + timestamp.Minute * 60 + timestamp.Second + (float)timestamp.Millisecond / 1000;
                                    m_Timecode.tc = Timecode.FromSeconds(FrameRate, totalSeconds);
                                    m_Timecode.rate = FrameRate;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.LogError($"{nameof(XsensClient)}({port}): " + socketException.Message);
            }
            catch (System.IO.IOException ioException)
            {
                if (IsConnected) // if not connected, then ignore this exception because it's a normal stream.Read interruption caused by closing the socket in Disconnect().
                    Debug.LogError($"{nameof(XsensClient)}({port}): " + ioException.Message);
            }
            finally
            {
                m_Client?.Close();
                m_Client = null;
                IsConnected = false; 
            }
        }

        FrameData ParsePacket(byte[] data)
        {
            XsDataPacket dataPacket = new XsQuaternionPacket(data);
            XsMvnPose pose = dataPacket.getPose();

            if (pose != null)
            {
                var frame = new FrameData()
                {
                    TC = m_Timecode.tc,
                    FrameRate = m_Timecode.rate,
                    SegmentCount = data.Length / 32,
                    Positions = pose.positions,
                    Orientations = pose.orientations,
                    NumProps = pose.MvnCurrentPropCount
                };

                return frame;
            }

            return default;
        }
    }
}