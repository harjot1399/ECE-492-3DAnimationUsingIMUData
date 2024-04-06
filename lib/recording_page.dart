import 'dart:io';
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:path/path.dart' as p;

class RecordingPage extends StatefulWidget {
  const RecordingPage({super.key});

  @override
  State<RecordingPage> createState() => _RecordingPageState();
}

class _RecordingPageState extends State<RecordingPage> {
  Future<List<File>> fetchFilesFromStoredDirectory() async {
    final SharedPreferences prefs = await SharedPreferences.getInstance();
    final String? directoryPath = prefs.getString('selectedDirectoryPath');
    final List<File> files = [];

    if (directoryPath != null) {
      final Directory directory = Directory(directoryPath);
      List<FileSystemEntity> entities = await directory.list().toList();

      for (FileSystemEntity entity in entities) {
        if (entity is File && p.extension(entity.path) == '.mp4'){
          files.add(entity);
        }
      }
    }

    return files;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFfcfcfb),
      body: Padding(
        padding: const EdgeInsets.only(left: 20, top: 20, right: 20),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text("Recordings", style: TextStyle(fontSize: 30)),
            const SizedBox(height: 30),
            Expanded(
              child: FutureBuilder<List<File>>(
                future: fetchFilesFromStoredDirectory(),
                builder: (BuildContext context, AsyncSnapshot<List<File>> snapshot) {
                  if (snapshot.connectionState == ConnectionState.waiting) {
                    return const Center(child: CircularProgressIndicator());
                  } else if (snapshot.hasError) {
                    return Center(child: Text('Error: ${snapshot.error}'));
                  } else if (snapshot.data == null || snapshot.data!.isEmpty) {
                    return const Center(child: Text('No recordings found'));
                  } else {
                    return ListView.builder(
                      itemCount: snapshot.data!.length,
                      itemBuilder: (BuildContext context, int index) {
                        String fileName = snapshot.data![index].path.split('/').last;
                        return Padding(
                          padding: const EdgeInsets.only(bottom: 20),
                          child: Card(
                            color: const Color(0XFF1c1b1f),
                            child: ListTile(
                              title: Text(fileName, style: const TextStyle(color: Color(0xFFfcfcfb)),),
                              leading: const Icon(Icons.video_call, color: Color(0xFFfcfcfb) ,),
                              shape: RoundedRectangleBorder(
                                borderRadius: BorderRadius.circular(8), // Rounded corners
                                side: BorderSide(color: Colors.grey.shade300), // Border
                              ),
                              onTap: () {
                              },
                            ),
                          ),
                        );
                      },
                    );
                  }
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}

