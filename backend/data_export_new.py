import asyncio
import websockets
import socket


import movelladot_pc_sdk


from xdpchandler import *


bodyPart = {
        "D4:22:CD:00:76:DB" : "leftForearm",
        "D4:22:CD:00:76:E4" : "rightForearm",
        "D4:22:CD:00:76:8C" : "leftArm",
        "D4:22:CD:00:76:84" : "rightArm",
        "D4:22:CD:00:76:8D" : "lowerback",
    }


def sensorMap(bluetoothAddress):
    return bodyPart[bluetoothAddress]


def setupDots(xdpcHandler):
    if not xdpcHandler.initialize():
        xdpcHandler.cleanup()
        exit(-1)


    xdpcHandler.scanForDots()
    if len(xdpcHandler.detectedDots()) == 0:
        print("No Movella DOT device(s) found. Aborting.")
        xdpcHandler.cleanup()
        exit(-1)


    xdpcHandler.connectDots()


    if len(xdpcHandler.connectedDots()) == 0:
        print("Could not connect to any Movella DOT device(s). Aborting.")
        xdpcHandler.cleanup()
        exit(-1)


    for device in xdpcHandler.connectedDots():
        filterProfiles = device.getAvailableFilterProfiles()
        print("Available filter profiles:")
        for f in filterProfiles:
            print(f.label())


        print(f"Current profile: {device.onboardFilterProfile().label()}")
        if device.setOnboardFilterProfile("General"):
            print("Successfully set profile to General")
        else:
            print("Setting filter profile failed!")


        print("Setting quaternion CSV output")
        device.setLogOptions(movelladot_pc_sdk.XsLogOptions_Quaternion)


        logFileName = "logfile_" + device.bluetoothAddress().replace(':', '-') + ".csv"
        print(f"Enable logging to: {logFileName}")
        if not device.enableLogging(logFileName):
            print(f"Failed to enable logging. Reason: {device.lastResultText()}")


        print("Putting device into measurement mode.")
        if not device.startMeasurement(movelladot_pc_sdk.XsPayloadMode_ExtendedEuler):
            print(f"Could not put device into measurement mode. Reason: {device.lastResultText()}")
            continue




def collect_and_send_data(xdpcHandler):
    setupDots(xdpcHandler)  # Assuming setupDots can be run synchronously or is adapted to async
   
    print("\nMain loop. Recording data for 100 seconds.")
    print("-----------------------------------------")


    orientationResetDone = False
    startTime = movelladot_pc_sdk.XsTimeStamp_nowMs()
    with open("datalog.txt", "w") as mylog:
        mylog.write("")
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        print("Opening connection to server...")
        s.connect(('127.0.0.1', 8081))
        # s.connect(('26.98.153.121', 3000))
        print("Connection opened.")

        while movelladot_pc_sdk.XsTimeStamp_nowMs() - startTime <= 300000:
            if xdpcHandler.packetsAvailable():
                for device in xdpcHandler.connectedDots():
                    bluetooth_address = device.bluetoothAddress()
                    sensorName = sensorMap(bluetooth_address)
                    packet = xdpcHandler.getNextPacket(device.portInfo().bluetoothAddress())

                    if packet.containsOrientation():
                        euler = packet.orientationEuler()
                        # quat = packet.orientationQuaternion()
                        data = f"Roll:{euler.x():5.2f}, Pitch:{euler.y():5.2f}, Yaw:{euler.z():5.2f} {sensorName} |"
                        # data = f"W:{quat[0]:5.2f}, X:{quat[1]:5.2f}, Y:{quat[2]:5.2f}, Z:{quat[3]:5.2f} {sensorName} |"
                        s.sendall(data.encode())
                        print(f"Client sent: {data}")
                        time.sleep(0.1)  # Sleep for a short time
                with open("datalog.txt", "a") as mylog:
                    mylog.write(data+'\n')
        print("Closing connection to server...")
    print("Connection closed.")


    print("\n-----------------------------------------", end="", flush=True)


    for device in xdpcHandler.connectedDots():
        print(f"\nResetting heading to default for device {device.portInfo().bluetoothAddress()}: ", end="", flush=True)
        if device.resetOrientation(movelladot_pc_sdk.XRM_DefaultAlignment):
            print("OK", end="", flush=True)
        else:
            print(f"NOK: {device.lastResultText()}", end="", flush=True)
    print("\n", end="", flush=True)


    print("\nStopping measurement...")
    for device in xdpcHandler.connectedDots():
        if not device.stopMeasurement():
            print("Failed to stop measurement.")
        if not device.disableLogging():
            print("Failed to disable logging.")


    xdpcHandler.cleanup()


def main():
    xdpcHandler = XdpcHandler()
    collect_and_send_data(xdpcHandler)


if __name__ == "__main__":
    main()
