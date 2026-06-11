import 'dart:io' show Platform;

import 'package:flutter/foundation.dart';

/// API base URL for dev/prod builds.
///
/// Override at run time:
/// `flutter run --dart-define=API_BASE_URL=http://192.168.1.10:5055`
class AppConfig {
  static const String _envUrl = String.fromEnvironment('API_BASE_URL');

  static String get apiBaseUrl {
    if (_envUrl.isNotEmpty) {
      return _envUrl;
    }

    if (kIsWeb) {
      return 'http://localhost:5055';
    }

    if (!kIsWeb && Platform.isAndroid) {
      return 'http://10.0.2.2:5055';
    }

    return 'http://localhost:5055';
  }
}
