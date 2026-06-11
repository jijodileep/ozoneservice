import 'package:dio/dio.dart';

import '../auth/token_storage.dart';
import '../config/app_config.dart';

typedef TokenReader = Future<String?> Function();
typedef BranchIdReader = Future<String?> Function();

class ApiClient {
  ApiClient({
    required TokenStorage tokenStorage,
    Dio? dio,
  }) : _tokenStorage = tokenStorage,
       _dio = dio ?? Dio() {
    _dio
      ..options.baseUrl = AppConfig.apiBaseUrl
      ..options.connectTimeout = const Duration(seconds: 15)
      ..options.receiveTimeout = const Duration(seconds: 15)
      ..interceptors.add(
        InterceptorsWrapper(
          onRequest: _onRequest,
        ),
      );
  }

  final TokenStorage _tokenStorage;
  final Dio _dio;

  Dio get dio => _dio;

  Future<void> _onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    if (!options.path.contains('/api/auth/login')) {
      final accessToken = await _tokenStorage.readAccessToken();
      if (accessToken != null && accessToken.isNotEmpty) {
        options.headers['Authorization'] = 'Bearer $accessToken';
      }

      final branchId = await _tokenStorage.readBranchId();
      if (branchId != null && branchId.isNotEmpty) {
        options.headers['X-Branch-Id'] = branchId;
      }
    }

    handler.next(options);
  }
}
