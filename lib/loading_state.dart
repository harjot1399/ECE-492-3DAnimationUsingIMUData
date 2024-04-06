import 'package:flutter/material.dart';
import 'package:frontend/start_assessment.dart';
import 'dart:io';
import 'package:path/path.dart' as p;
import 'dart:convert';


class LoadingState extends StatefulWidget {
  const LoadingState({super.key});

  @override
  State<LoadingState> createState() => _LoadingStateState();
}

class _LoadingStateState extends State<LoadingState> {

  @override
  void initState() {
    super.initState();
    runfiles();
  }

  void runfiles() async {
    try {
      Directory current = Directory.current;
      var scriptPath = p.join(current.path, 'backend', 'data_export_new.py');
      var unityPath = p.join(current.path, 'unity_project', 'My project.exe');
      print (scriptPath);
      print (unityPath);
      Process pythonProcess = await Process.start('python', [scriptPath]);
      pythonProcess.stdout.transform(utf8.decoder).listen((data) {
        print("Python stdout: $data");
      });
      pythonProcess.stderr.transform(utf8.decoder).listen((data) {
        print("Python stderr: $data");
      });
      print("Python script started");

      
      Process unityProcess = await Process.start(unityPath, []);
      unityProcess.stdout.transform(utf8.decoder).listen((data) {
        print("Unity stdout: $data");
      });
      unityProcess.stderr.transform(utf8.decoder).listen((data) {
        print("Unity stderr: $data");
      });
      print("Unity script started");
      if (mounted) {
        Future.microtask(() =>
        Navigator.pushReplacement(context, MaterialPageRoute(builder: (_) => const StartAssessment())));
      }
    } catch (e) {
      print("Path Error");
      print(e);
    }
  }

 
  @override
  Widget build(BuildContext context) {
    // Always return a Scaffold (or another appropriate widget) to avoid returning null
    return const Scaffold(
      body: Center(
        child: CircularProgressIndicator(),
      ),
    );
  }
}