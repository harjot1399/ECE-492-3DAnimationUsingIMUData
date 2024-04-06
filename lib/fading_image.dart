import 'dart:io';
import 'dart:async';
import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';
import 'package:fl_chart/fl_chart.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:frontend/home_page.dart';



// // Custom widget to handle image display with fade transition
// class FadingImage extends StatefulWidget {
//   final Image image;

//   const FadingImage({Key? key, required this.image}) : super(key: key);

//   @override
//   _FadingImageState createState() => _FadingImageState();
// }

// class _FadingImageState extends State<FadingImage> {
//   double _opacity = 0.0;

//   @override
//   void initState() {
//     super.initState();
//     // Start the fade in effect when the widget is first built
//     WidgetsBinding.instance.addPostFrameCallback((_) {
//       if (mounted) {
//         setState(() {
//           _opacity = 1.0;
//         });
//       }
//     });
//   }

//   @override
//   void didUpdateWidget(covariant FadingImage oldWidget) {
//     super.didUpdateWidget(oldWidget);
//     if (widget.image != oldWidget.image) {
//       // Restart the fade in effect for new images
//       _opacity = 0.0;
//       Future.delayed(Duration.zero, () {
//         if (mounted) {
//           setState(() {
//             _opacity = 1.0;
//           });
//         }
//       });
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return AnimatedOpacity(
//       opacity: _opacity,
//       duration: Duration(milliseconds: 500),
//       child: widget.image,
//     );
//   }
// }

// class CrossFadeImages extends StatefulWidget {
//   final Image newImage;
//   const CrossFadeImages({Key? key, required this.newImage}) : super(key: key);

//   @override
//   _CrossFadeImagesState createState() => _CrossFadeImagesState();
// }

// class _CrossFadeImagesState extends State<CrossFadeImages> {
//   late Image _currentImage;
//   late Image _nextImage;
//   double _nextImageOpacity = 0.0;

//   // Duration of the fade transition
//   final Duration fadeDuration = Duration(milliseconds: 500);
//   // Minimum time each image is displayed, including fade transition time
//   final Duration minDisplayTime = Duration(milliseconds: 200);

//   DateTime? _lastImageUpdateTime;

//   @override
//   void initState() {
//     super.initState();
//     _currentImage = widget.newImage;
//     _nextImage = widget.newImage;
//     _lastImageUpdateTime = DateTime.now();
//   }

//   @override
//   void didUpdateWidget(covariant CrossFadeImages oldWidget) {
//     super.didUpdateWidget(oldWidget);
//     if (widget.newImage != oldWidget.newImage) {
//       final now = DateTime.now();
//       final timeSinceLastUpdate = now.difference(_lastImageUpdateTime!);
//       if (timeSinceLastUpdate >= minDisplayTime) {
//         updateImage(now);
//       } else {
//         // If the min display time hasn't passed, wait until it has
//         Future.delayed(minDisplayTime - timeSinceLastUpdate, () {
//           if (mounted) {
//             updateImage(DateTime.now());
//           }
//         });
//       }
//     }
//   }

//   void updateImage(DateTime updateTime) {
//     // Set next image and start transition
//     _nextImage = widget.newImage;
//     _nextImageOpacity = 0.0; // Start transparent and fade in
//     setState(() {
//       _nextImageOpacity = 1.0; // Fade in the new image
//     });
//     _lastImageUpdateTime = updateTime;
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Stack(
//       alignment: Alignment.center,
//       children: [
//         _currentImage,
//         AnimatedOpacity(
//           opacity: _nextImageOpacity,
//           duration: fadeDuration,
//           child: _nextImage,
//           onEnd: () {
//             // Once the fade in is complete, set current image to next image
//             _currentImage = _nextImage;
//           },
//         ),
//       ],
//     );
//   }
// }


class CrossFadeImages extends StatefulWidget {
  final Image newImage;
  const CrossFadeImages({Key? key, required this.newImage}) : super(key: key);

  @override
  _CrossFadeImagesState createState() => _CrossFadeImagesState();
}

class _CrossFadeImagesState extends State<CrossFadeImages> with TickerProviderStateMixin {
  late Image _currentImage;
  late Image _nextImage;
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;

  @override
  void initState() {
    super.initState();
    _currentImage = widget.newImage; // Initialize with the first image.
    _nextImage = widget.newImage; // This will be updated with each new image.

    _animationController = AnimationController(duration: const Duration(milliseconds: 500), vsync: this);
    _fadeAnimation = Tween<double>(begin: 0.0, end: 1.0).animate(_animationController)
      ..addListener(() {
        setState(() {});
      })
      ..addStatusListener((status) {
        if (status == AnimationStatus.completed) {
          _currentImage = _nextImage; // Update current image after transition
          _animationController.reset();
        }
      });
  }

  @override
  void didUpdateWidget(covariant CrossFadeImages oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.newImage != oldWidget.newImage) {
      _nextImage = widget.newImage;
      _animationController.forward(); // Start the fade animation
    }
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      alignment: Alignment.center,
      children: [
        // Current image is always fully visible by default (opacity 1.0)
        _currentImage,
        // Next image fades in over the current image
        Opacity(
          opacity: _fadeAnimation.value,
          child: _nextImage,
        ),
      ],
    );
  }
}
