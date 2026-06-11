import 'package:dio/dio.dart';

import '../api/api_client.dart';
import 'mobile_master_models.dart';

class MobileMasterService {
  MobileMasterService(this._apiClient);

  final ApiClient _apiClient;

  Dio get _dio => _apiClient.dio;

  Future<List<MobileBrandOption>> searchActiveBrands(
    String search, {
    int pageSize = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/mobile-masters/brands',
      queryParameters: {
        'activeOnly': true,
        if (search.trim().isNotEmpty) 'search': search.trim(),
        'page': 1,
        'pageSize': pageSize,
      },
    );
    final items = response.data!['items'] as List<dynamic>;
    return items
        .cast<Map<String, dynamic>>()
        .map(MobileBrandOption.fromJson)
        .toList();
  }

  Future<List<MobileModelOption>> getActiveModels(String brandId) async {
    final response = await _dio.get<List<dynamic>>(
      '/api/mobile-masters/brands/$brandId/models',
      queryParameters: {'activeOnly': true},
    );
    return response.data!
        .cast<Map<String, dynamic>>()
        .map(MobileModelOption.fromJson)
        .toList();
  }

  Future<List<MobileVariantOption>> getActiveVariants(String modelId) async {
    final response = await _dio.get<List<dynamic>>(
      '/api/mobile-masters/models/$modelId/variants',
      queryParameters: {'activeOnly': true},
    );
    return response.data!
        .cast<Map<String, dynamic>>()
        .map(MobileVariantOption.fromJson)
        .toList();
  }
}
