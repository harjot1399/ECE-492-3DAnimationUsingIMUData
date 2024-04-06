import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'dart:convert';
import 'package:http/http.dart' as http;

class SettingsPage extends StatefulWidget {
  const SettingsPage({super.key});

  @override
  State<SettingsPage> createState() => _SettingsPageState();
}

class _SettingsPageState extends State<SettingsPage> {
  final TextEditingController directory = TextEditingController();

  @override
  void initState(){
    super.initState();
    _loadStoredDirectoryPath();


  }

  Future<void> _loadStoredDirectoryPath() async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final String? storedDirectory = prefs.getString('selectedDirectoryPath');
    if (storedDirectory != null) {
      setState(() {
        directory.text = storedDirectory;
      });
    }
  }

  Future<void> selectFolder() async {
  String? selectedDirectory = await FilePicker.platform.getDirectoryPath();
  if (selectedDirectory != null) {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    await prefs.setString('selectedDirectoryPath', selectedDirectory);
    final uri = Uri.parse('http://localhost:8082/$selectedDirectory');
    await http.post(uri, body: jsonEncode({'directory': selectedDirectory}), headers: {'Content-Type': 'application/json'});
    setState(() {
      directory.text = selectedDirectory;
    });
    }
  }


  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFfcfcfb),
        body: Padding(
          padding: const EdgeInsets.only(left:15.0, right: 20, top: 20),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              const Text("Settings", style: TextStyle(fontSize: 30),),
              const SizedBox(height: 40,),
              Row(
                children: [
                  const Text("Recordings Directory: ", style: TextStyle(fontSize: 20),),
                  Expanded(
                    child: TextField(
                      controller: directory,
                      decoration: InputDecoration(
                        suffixIcon: IconButton(onPressed: selectFolder, icon: const Icon(Icons.folder))
                      ),
                    ),
                  )
                ],
              )
            ],
          ),
        )
    );
  }
}

