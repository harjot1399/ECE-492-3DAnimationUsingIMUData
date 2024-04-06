import 'dart:io';
import 'dart:async';
import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';
import 'package:fl_chart/fl_chart.dart';
import 'package:frontend/fading_image.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:frontend/home_page.dart'; // Ensure this is correctly imported
import 'package:path/path.dart' as p;
import 'package:shared_preferences/shared_preferences.dart';
import 'package:intl/intl.dart';

class StartAssessment extends StatefulWidget {
  const StartAssessment({Key? key}) : super(key: key);

  @override
  State<StartAssessment> createState() => _StartAssessmentState();
}



class _StartAssessmentState extends State<StartAssessment> {
  final TextEditingController filename = TextEditingController();
  final StreamController<Image> _imageStreamController = StreamController.broadcast();
  List<FlSpot> rollSpots = [];
  List<FlSpot> pitchSpots = [];
  List<FlSpot> yawSpots = [];
  Image? latestImage;
   String imageDataBuffer = '';
  Process? killPythonProcess;
  Socket? socket;
  bool isloaded = false;
  bool pythonClientFinished = false;
  bool isVideoDone = false;


  Future<void> saveCSV() async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final String? directoryPath = prefs.getString('selectedDirectoryPath');
    Directory current = Directory.current;
    var files = <File>[];
    await for (var entity in current.list()) {
      if (entity is File && p.extension(entity.path) == '.csv') {
        files.add(entity);
      }
    }
    for (var file in files) {
     String timeNow = DateFormat('yyyyMMdd_HHmmss').format(DateTime.now());
    final fileNameWithoutExtension = p.basenameWithoutExtension(file.path);
    final fileExtension = p.extension(file.path);
    final newFileName = '${fileNameWithoutExtension}_$timeNow$fileExtension';
    final newPath = p.join(directoryPath!, newFileName);
    await file.copy(newPath);
    print('Copied ${file.path} to $newPath');
    }
  }

  @override
  void initState() {
    super.initState();
    // runUnityProject();
    runfiles();
  }

  void connectToUnityServer() async {
    try {
      socket = await Socket.connect('127.0.0.1', 8080);
      print('Connected to Unity TCP Server');
      socket!.listen(dataHandler,
          onError: errorHandler, onDone: doneHandler, cancelOnError: false);
    } catch (e) {
      print("Failed to connect: $e");
    }
  }



  void dataHandler(Uint8List data) {
    if (!mounted) return; // Check if the widget is still mounted

    // Decode the incoming raw bytes to a string and add it to the buffer
    String base64Chunk = utf8.decode(data);
    imageDataBuffer += base64Chunk;

    if (imageDataBuffer.contains('<EOF>')) {
      final parts = imageDataBuffer.split('<EOF>');
      String base64Image = parts.first;

      _imageStreamController.add(Image.memory(base64.decode(base64Image)));
      imageDataBuffer = parts.length > 1 ? parts[1] : '';
    }
  }

  Future<void> runvideomaker() async {
    try {
      Directory current = Directory.current;
      var scriptPath = p.join(current.path, 'backend', 'videomaker.py');
      final SharedPreferences prefs = await SharedPreferences.getInstance();
      final String? directoryPath = prefs.getString('selectedDirectoryPath');
      // var unityPath = p.join(current.path, 'unity_project', 'My project.exe');
      print (scriptPath);
      // print (unityPath);
      Process process = await Process.start('python', ['-u', scriptPath, directoryPath!]);
      process.stderr.transform(utf8.decoder).listen((data) {
      print("Python stderr: $data");
      });

      int exitCode = await process.exitCode;
      if (exitCode == 0) {
        print('Video creation successful');
        setState(() {
          isVideoDone = true;
        });
      } else {
        print('Video creation failed');
      }

      
    } catch (e) {
      print("Path Error");
      print(e);
    }
  }

  void runUnityProject() async {
    try {
      Directory current = Directory.current;
      var unityPath = p.join(current.path, 'unity_project', 'My project.exe');
      print (unityPath);
      Process unityProcess = await Process.start('"$unityPath"', []);
      unityProcess.stdout.transform(utf8.decoder).listen((data) {
        print("Unity stdout: $data");
      });
      unityProcess.stderr.transform(utf8.decoder).listen((data) {
        print("Unity stderr: $data");
      });
      print("Unity script started");
    } catch (e) {
      print("Path Error");
      print(e);
    }
  }

   void runfiles() async {
    try {
      Directory current = Directory.current;
      var scriptPath = p.join(current.path, 'backend', 'data_export_new.py');
      // var unityPath = p.join(current.path, 'unity_project', 'My project.exe');
      print (scriptPath);
      // print (unityPath);
      Process pythonProcess = await Process.start('python', ['-u', scriptPath]);
      killPythonProcess = pythonProcess;
      pythonProcess.stdout.transform(utf8.decoder).listen((data) {
        print("Python stdout: $data");
        if(data.contains("Roll")){
          connectToUnityServer();
        }
      });
      pythonProcess.stderr.transform(utf8.decoder).listen((data) {
        print("Python stderr: $data");
      });
      print("Python script started");
      if (mounted) {
        setState(() {
          isloaded =  true;
        });
      }
    } catch (e) {
      print("Path Error");
      print(e);
    }
  }



  void errorHandler(error, StackTrace trace) {
    print(error);
  }

  void doneHandler() {
    _imageStreamController.close();
    socket!.destroy();
    print('Connection has been closed');
  }

  @override
  void dispose() {
    socket?.close();
    killPythonProcess?.kill();
    super.dispose();
  }

//   List<FlSpot> subsampleData(List<FlSpot> data, int step) {
//   return List.generate(
//     (data.length / step).ceil(),
//     (index) => data[index * step],
//   );
// }

Future<void> loadChartData() async {
  try {
    Directory current = Directory.current;
    final file = File(p.join(current.path, 'datalog.txt'));
    final lines = await file.readAsLines();

    List<double> rollAngles = [];
    List<double> pitchAngles = [];
    List<double> yawAngles = [];

    for (var line in lines) {
      var parts = line.split(',');
      var roll = double.parse(parts[0].split(':')[1]);
      var pitch = double.parse(parts[1].split(':')[1]);
      var yaw = double.parse(parts[2].split(':')[1].substring(0, 6));
      rollAngles.add(roll);
      pitchAngles.add(pitch);
      yawAngles.add(yaw);
    }

    // Determine the subsample step based on the number of data points
    int subsampleStep = 1; // Default to no subsampling
    int maxPoints = 200; // The maximum number of points you want to plot

    if (rollAngles.length > maxPoints) {
      subsampleStep = (rollAngles.length / maxPoints).ceil();
    }

    setState(() {
      rollSpots = List.generate(
        (rollAngles.length / subsampleStep).ceil(),
        (index) => FlSpot(index * subsampleStep.toDouble(), rollAngles[index * subsampleStep]),
      );

      pitchSpots = List.generate(
        (pitchAngles.length / subsampleStep).ceil(),
        (index) => FlSpot(index * subsampleStep.toDouble(), pitchAngles[index * subsampleStep]),
      );

      yawSpots = List.generate(
        (yawAngles.length / subsampleStep).ceil(),
        (index) => FlSpot(index * subsampleStep.toDouble(), yawAngles[index * subsampleStep]),
      );
    });

  } catch (e) {
    print("Error loading chart data: $e");
  }
}


  Future<void> startRecording() async {
    final uri = Uri.parse('http://localhost:8082/startrecording');
    await http.post(uri, body: jsonEncode({'command': 'start'}), headers: {'Content-Type': 'application/json'});
  }

  Future<void> stopRecording() async {
    final uri = Uri.parse('http://localhost:8082/stoprecording');
    await http.post(uri, body: jsonEncode({'command': 'stop'}), headers: {'Content-Type': 'application/json'});
    setState(() {
      pythonClientFinished = true;
    });
    loadChartData();
    killPythonProcess?.kill();
    saveCSV();
  }

  Future<void> disposeRecording() async{
    final uri = Uri.parse('http://localhost:8082/disposerecording');
    await http.post(uri, body: jsonEncode({'command': 'dispose'}), headers: {'Content-Type': 'application/json'});
  }
  

  Future<void> saveRecording() async{
    await runvideomaker();
    final uri = Uri.parse('http://localhost:8082/saverecording');
    if (isVideoDone){
      await http.post(uri, body: jsonEncode({'command': 'save'}), headers: {'Content-Type': 'application/json'});
    }
  }

  // final List<FlSpot> data = [
  //   const FlSpot(0, 1),
  //   const FlSpot(1, 3),
  //   const FlSpot(2, 10),
  //   // Add as many points as you need
  // ];

  List<Image> images = [];




  Future<void> _dialogBuilder(BuildContext context) {
    return showDialog<void>(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Do you want to save the session'),
          // content: TextField(controller: filename,),
          actions: <Widget>[
            TextButton(
              style: TextButton.styleFrom(
                textStyle: Theme.of(context).textTheme.labelLarge,
              ),
              child: const Text('Cancel'),
              onPressed: () {
                disposeRecording();
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              style: TextButton.styleFrom(
                textStyle: Theme.of(context).textTheme.labelLarge,
              ),
              child: const Text('Save'),
              onPressed: () {
                saveRecording();
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );
  }


  @override
  Widget build(BuildContext context) {
    if (!isloaded){
       return const Scaffold(
        body: Center(
          child: CircularProgressIndicator(),
        ),
      );

    }else{
      return Scaffold(
      backgroundColor: const Color(0xFFfcfcfb),
      body: SingleChildScrollView(
        scrollDirection: Axis.vertical,
        child: SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Padding(
                padding: const EdgeInsets.only(left: 15.0, top: 10.0),
                child: Row(
                  children: [
                    const Text("Assessment", style: TextStyle(fontSize: 30),),
                    const SizedBox(width: 1070,),
                    Card(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(20.0), // Adjust the radius here
                      ),
                      color: Colors.grey,
                      elevation: 5.0,
                      child: Row(
                        children: [
                          IconButton(onPressed: (){if (kDebugMode) {
                            print("start recording");
                          }
                          startRecording();

                          }, icon: const Icon(Icons.play_arrow_sharp, color: Colors.white,)),
                          IconButton(onPressed: (){if (kDebugMode) {
                            print("stop recording");
                          }
                          stopRecording();
                          _dialogBuilder(context);
                            }, icon: const Icon(Icons.pause, color: Colors.white,)),

                        ],
                      ),
                    )

                  ],
                ),
              ),
              const SizedBox(height: 20),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 15.0),
                child: Card(
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20.0), // Adjust the radius here
                  ),
                  color: Colors.grey,
                  elevation: 5.0,
                  child: Container(
                    width: 1325, // Specify your custom width
                    height: 500, // Specify your custom height
                    padding: const EdgeInsets.all(16.0), // Add some padding if needed
                    // child: latestImage ?? const Center(child: Text('Waiting for image...', style: TextStyle(color: Colors.white))),
                     child: StreamBuilder<Image>(
                      stream: _imageStreamController.stream,
                      builder: (context, snapshot) {
                        if (snapshot.hasData) {
                          return CrossFadeImages(newImage: snapshot.data!);
                        } else {
                          return const Center(child: Text('Waiting for image...', style: TextStyle(color: Colors.white)));
                        }
                      },
                    ),
                  ),
                ),
              ),
              const SizedBox(height: 30,),
              Row(
                children: [
                  Column(
                    children: [
                      const Padding(
                    padding: EdgeInsets.only(left: 15.0, top: 10.0),
                    child: Text(
                      "Roll Chart", // Chart title
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                        color: Colors.black,
                      ),
                    ),
                  ),
                    Card(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(20.0), // Adjust the radius here
                      ),
                      elevation: 5.0,
                      color: Colors.grey,
                      child: Container(
                        width: 430, // Specify your custom width
                        height: 300, // Specify your custom height
                        padding: const EdgeInsets.all(8.0), // Add some padding if needed
                        child: LineChart(
                          LineChartData(
                            lineBarsData: [
                              LineChartBarData(
                                spots: rollSpots,
                                isCurved: true,
                                barWidth:3,
                                color: Colors.black,
                                belowBarData: BarAreaData(show: false),
                                dotData: const FlDotData(show: false),
                              )
                            ]
                          )
                        ),
                      ),
                    ),

                    ],
                  ),
                  
                  const SizedBox(width: 10,),
                  Column(
                    children: [
                       const Padding(
                    padding: EdgeInsets.only(left: 15.0, top: 10.0),
                    child: Text(
                      "Pitch Chart", // Chart title
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                         color: Colors.black,
                      ),
                    ),
                  ),
                  Card(
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(20.0), // Adjust the radius here
                    ),
                    elevation: 5.0,
                    color: Colors.grey,
                    child: Container(
                      width: 430, // Specify your custom width
                      height: 300, // Specify your custom height
                      padding: const EdgeInsets.all(16.0), // Add some padding if needed
                      child: LineChart(
                          LineChartData(
                              lineBarsData: [
                                LineChartBarData(
                                  spots: pitchSpots,
                                  isCurved: true,
                                  barWidth:3,
                                  color: Colors.black,
                                  belowBarData: BarAreaData(show: false),
                                  dotData: const FlDotData(show: false),
                                )
                              ]
                          )
                      ),
                    ),
                  ),

                    ],
                  ),
                 
                  const SizedBox(width: 10,),
                  Column(
                    children: [
                      const Padding(
                    padding: EdgeInsets.only(left: 15.0, top: 10.0),
                    child: Text(
                      "Yaw Chart", // Chart title
                      style: TextStyle(
                        fontSize: 24,
                        fontWeight: FontWeight.bold,
                         color: Colors.black,
                      ),
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.only(right: 15.0),
                    child: Card(
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(20.0), // Adjust the radius here
                      ),
                      elevation: 5.0,
                      color: Colors.grey,
                      child: Container(
                        width: 430, // Specify your custom width
                        height: 300, // Specify your custom height
                        padding: const EdgeInsets.all(16.0), // Add some padding if needed
                        child: LineChart(
                            LineChartData(
                                lineBarsData: [
                                  LineChartBarData(
                                    spots: yawSpots,
                                    isCurved: true,
                                    barWidth:3,
                                    color: Colors.black,
                                    belowBarData: BarAreaData(show: false),
                                    dotData: const FlDotData(show: false),
                                  )
                                ]
                            )
                        ),
                      ),
                    ),
                  ),

                    ],
                  )
                  
                ],
              )
            ],
          ),
        ),
      ),
    );
      
    }
    
  }
}
