import 'package:flutter/material.dart';


class SensorInfo extends StatelessWidget {
  const SensorInfo({super.key});

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      backgroundColor: Color(0xFFfcfcfb),
      body: Padding(
        padding: EdgeInsets.only(left: 20, top: 20),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text("Sensor Information", style: TextStyle(fontSize: 30,),),
            SizedBox(height: 40,),
            Image(image: AssetImage("assets/images/sensor.jpeg")),
            Text(
              'Xsens DOT is a wearable sensor development platform featuring sensors incorporating '
                  '3D accelerometer, gyroscope, and magnetometer to provide an accurate 3D orientation. '
                  'The embedded processor in the sensor handles sampling, calibration, strap-down '
                  'integration of inertial data, and the Xsens Kalman Filter core (XKFCore) algorithm for sensor '
                  'fusion. With wireless data transmission through Bluetooth 5.0, Xsens DOT can provide real-time '
                  '3D orientation as well as calibrated 3D linear acceleration, angular velocity, and (earth) '
                  'magnetic field data to a receiving device.\n\n'
                  'Xsens DOT is the start of a new Xsens product line bringing Xsens sensor solutions to '
                  'mobile device platforms. With a simple-to-use Software-Development-Kit (SDK) and '
                  'comprehensive documentation, system integrators can easily develop their wearable '
                  'applications.\n\n'
                  'The Bluetooth Low Energy (BLE) data transmission, lightweight form factor, and IP68 waterproof '
                  'rating widen the application areas of Xsens DOT, making it easy and durable to use '
                  'in various scenarios. Using different combinations of 5 sensors, it’s flexible to apply Xsens '
                  'DOT to customized measurement cases.\n\n'
                  'Fields of use include:\n'
                  '1. Health and rehabilitation\n'
                  '2. Sports and exercise science\n'
                  '3. Ergonomics',
              style: TextStyle(
                fontSize: 16,
                height: 1.5,
              ),
            ),
          ],),
      ),
    );
  }
}
