import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/loading_state.dart';
import 'package:frontend/recording_page.dart';
import 'package:frontend/sensor_info.dart';
import 'package:frontend/settings_page.dart';
import 'package:frontend/start_assessment.dart';


class HomePage extends StatefulWidget {
  const HomePage({super.key});


  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _selectedIndex = 0;
  final List<Widget> _pages = [
    const RecordingPage(),
    const StartAssessment(
  
    ),
    const SensorInfo(),
    const SettingsPage(),
  ];
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFeeedf4),
      body: SafeArea(
        child: Row(
          children: [


            NavigationRail(destinations: const <NavigationRailDestination>[

              NavigationRailDestination(icon: Icon(Icons.house_outlined)
                  , label: Text('Home'),
                  selectedIcon: Icon(Icons.house, color: Color(0xFFfcfcfb),)
              ),
              NavigationRailDestination(icon: Icon(Icons.assessment_outlined)
                  , label: Text('Assessment'),
                  selectedIcon: Icon(Icons.assessment, color: Color(0xFFfcfcfb),)
              ),
              NavigationRailDestination(icon: Icon(Icons.sensors_outlined)
                  , label: Text('Sensor Information'),
                  selectedIcon: Icon(Icons.sensors, color: Color(0xFFfcfcfb),)
              ),
              NavigationRailDestination(icon: Icon(Icons.settings_outlined)
                  , label: Text('Settings'),
                  selectedIcon: Icon(Icons.settings, color: Color(0xFFfcfcfb),)
              ),
            ],
                selectedIndex: _selectedIndex,
              onDestinationSelected: (int index) {
                  setState(() {
                    _selectedIndex = index;
                  });
              },
              backgroundColor: const Color(0XFF1c1b1f),
              useIndicator: true,
              indicatorColor: const Color(0xFF3e3e46),

              labelType: NavigationRailLabelType.selected,
              selectedLabelTextStyle: const TextStyle(color: Colors.white),
              


            ),
            Expanded(child: _pages[_selectedIndex])
            
          ],
        ),
      ),
    );
  }
}

