import 'package:dio/dio.dart';

import '../api/api_client.dart';
import 'customer_models.dart';

class CustomerService {
  CustomerService(this._apiClient);

  final ApiClient _apiClient;

  Dio get _dio => _apiClient.dio;

  Future<CustomerDetail?> lookupByMobile(String mobile) async {
    try {
      final response = await _dio.get<Map<String, dynamic>>(
        '/api/customers/lookup',
        queryParameters: {'mobile': mobile},
      );
      return CustomerDetail.fromJson(response.data!);
    } on DioException catch (error) {
      if (error.response?.statusCode == 404) {
        return null;
      }
      rethrow;
    }
  }

  Future<CustomerDetail> getCustomer(String id) async {
    final response = await _dio.get<Map<String, dynamic>>('/api/customers/$id');
    return CustomerDetail.fromJson(response.data!);
  }

  Future<CustomerDetail> createCustomer({
    required String name,
    required String mobileNumber,
    String? email,
    String? address,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/customers',
      data: {
        'name': name,
        'mobileNumber': mobileNumber,
        'email': email,
        'address': address,
      },
    );
    return CustomerDetail.fromJson(response.data!);
  }

  Future<CustomerDevice> addDevice({
    required String customerId,
    required String variantId,
    String? imei,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/customers/$customerId/devices',
      data: {
        'variantId': variantId,
        'imei': imei,
      },
    );
    return CustomerDevice.fromJson(response.data!);
  }
}
