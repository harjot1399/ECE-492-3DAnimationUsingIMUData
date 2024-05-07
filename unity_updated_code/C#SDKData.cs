
// //  Copyright (c) 2003-2023 Movella Technologies B.V. or subsidiaries worldwide.
// //  All rights reserved.
// //  
// //  Redistribution and use in source and binary forms, with or without modification,
// //  are permitted provided that the following conditions are met:
// //  
// //  1.	Redistributions of source code must retain the above copyright notice,
// //  	this list of conditions and the following disclaimer.
// //  
// //  2.	Redistributions in binary form must reproduce the above copyright notice,
// //  	this list of conditions and the following disclaimer in the documentation
// //  	and/or other materials provided with the distribution.
// //  
// //  3.	Neither the names of the copyright holders nor the names of their contributors
// //  	may be used to endorse or promote products derived from this software without
// //  	specific prior written permission.
// //  
// //  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
// //  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// //  MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
// //  THE COPYRIGHT HOLDERS OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// //  SPECIAL, EXEMPLARY OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
// //  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// //  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY OR
// //  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// //  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// //  

// using System;
// using MovellaDotPcSdk;
// using Movella;

// namespace ExampleReceiveData
// {
// 	static class Program
// 	{
// 		static void Main()
// 		{
//             XdpcHandler xdpcHandler = new XdpcHandler();

//             if (!xdpcHandler.initialize())
//                 return;

//             xdpcHandler.scanForDots();

//             if (xdpcHandler.detectedDots().empty())
//             {
//                 Console.WriteLine("No Movella DOT device(s) found. Aborting.");
//                 xdpcHandler.cleanup();
//                 return;
//             }

//             xdpcHandler.connectDots();

//             if (xdpcHandler.ConnectedDots.Count == 0)
//             {
//                 Console.WriteLine("Could not connect to any Movella DOT device(s). Aborting.");
//                 xdpcHandler.cleanup();
//                 return;
//             }

//             foreach (XsDotDevice device in xdpcHandler.ConnectedDots)
//             {
//                 XsFilterProfileArray filterProfiles = device.getAvailableFilterProfiles();

//                 Console.WriteLine("Available filter profiles: ");
//                 for (uint j = 0; j < filterProfiles.size(); j++)
//                     Console.WriteLine(filterProfiles.at(j).label());

//                 Console.WriteLine("Current filter profile: {0}", device.onboardFilterProfile().label());
//                 if (device.setOnboardFilterProfile(new XsString("General")))
//                     Console.WriteLine("Successfully set filter profile to General");
//                 else
//                     Console.WriteLine("Failed to set filter profile!");

//                 Console.WriteLine("Setting quaternion CSV output");
//                 device.setLogOptions(XsLogOptions.Euler);

//                 XsString logFileName = new XsString("logfile_" + device.bluetoothAddress().toString().Replace(':', '-') + ".csv");
//                 Console.WriteLine("Enable logging to: {0}", logFileName.toString());
//                 if (!device.enableLogging(logFileName))
//                 {
//                     Console.WriteLine("Failed to enable logging. Reason: {0}", device.lastResultText().toString());
//                     continue;
//                 }

//                 Console.WriteLine("Putting device into measurement mode. ");
//                 if (!device.startMeasurement(XsPayloadMode.ExtendedEuler))
//                 {
//                     Console.WriteLine("Could not put device into measurement mode. Reason: {0}", device.lastResultText().toString());
//                     continue;
//                 }
//             }

//             Console.WriteLine("Starting measurement...");

//             Console.WriteLine("Main loop. Measuring data for 10 seconds.");
//             Console.WriteLine("-----------------------------------------");

//             // First printing some headers so we see which data belongs to which device
//             foreach (XsDotDevice device in xdpcHandler.ConnectedDots)
//                 Console.Write("{0,-42}", device.bluetoothAddress().toString());
//             Console.Write("\n");

//             bool orientationResetDone = false;
//             long startTime = XsTimeStamp.nowMs();
//             while (XsTimeStamp.nowMs() - startTime <= 10000)
//             {
//                 if (xdpcHandler.packetsAvailable())
//                 {
//                     Console.Write("\r");
//                     foreach (XsDotDevice device in xdpcHandler.ConnectedDots)
//                     {
//                         // Retrieve a packet
//                         XsDataPacket packet = xdpcHandler.getNextPacket(device.bluetoothAddress().toString());

//                         if (packet.containsOrientation())
//                         {
//                             XsEuler euler = packet.orientationEuler();
//                             Console.Write("Roll:{0,7:f2}, Pitch:{1,7:f2}, Yaw:{2,7:f2}| ", euler.roll(), euler.pitch(), euler.yaw());
//                         }

//                         packet.Dispose();
//                     }
//                     if (!orientationResetDone && (XsTimeStamp.nowMs() - startTime) > 5000)
//                     {
//                         foreach (XsDotDevice device in xdpcHandler.ConnectedDots)
//                         {
//                             Console.Write("\nResetting heading for device {0}: ", device.bluetoothAddress().toString());
//                             if (device.resetOrientation(XsResetMethod.XRM_Heading))
//                                 Console.Write("OK");
//                             else
//                                 Console.Write("NOK: {0}", device.lastResultText().toString());
//                         }
//                         Console.WriteLine("");
//                         orientationResetDone = true;
//                     }
//                 }
//             }
//             Console.WriteLine("\n-----------------------------------------");

//             foreach (XsDotDevice device in xdpcHandler.ConnectedDots)
//             {
//                 Console.Write("\nResetting heading to default for device {0}: ", device.bluetoothAddress().toString());
//                 if (device.resetOrientation(XsResetMethod.XRM_DefaultAlignment))
//                     Console.Write("OK");
//                 else
//                     Console.Write("NOK: {0}", device.lastResultText().toString());
//             }
//             Console.WriteLine("\n");

//             Console.WriteLine("Stopping measurement...");
//             foreach (XsDotDevice device in xdpcHandler.ConnectedDots)
//             {
//                 if (!device.stopMeasurement())
//                     Console.Write("Failed to stop measurement.");
//                 if (!device.disableLogging())
//                     Console.Write("Failed to disable logging.");
//             }

//             xdpcHandler.cleanup();
//         }
//     }
// }
