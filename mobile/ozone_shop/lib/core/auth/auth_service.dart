import 'package:dio/dio.dart';

import '../api/api_client.dart';
import 'models.dart';
import 'token_storage.dart';

class AuthService {
  AuthService(this._apiClient, this._tokenStorage);

  final ApiClient _apiClient;
  final TokenStorage _tokenStorage;

  Dio get _dio => _apiClient.dio;

  Future<AuthSession> login(String email, String password) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '/api/auth/login',
      data: {'email': email, 'password': password},
    );

    final tokens = TokenResponse.fromJson(response.data!);
    await _tokenStorage.saveTokens(
      accessToken: tokens.accessToken,
      refreshToken: tokens.refreshToken,
    );

    return _loadSession();
  }

  Future<AuthSession?> restoreSession() async {
    final accessToken = await _tokenStorage.readAccessToken();
    if (accessToken == null || accessToken.isEmpty) {
      return null;
    }

    try {
      return await _loadSession();
    } on DioException catch (error) {
      if (error.response?.statusCode == 401) {
        await _tokenStorage.clear();
      }
      return null;
    }
  }

  Future<UserProfile> fetchProfile() async {
    final response = await _dio.get<Map<String, dynamic>>('/api/auth/me');
    return UserProfile.fromJson(response.data!);
  }

  Future<void> logout() => _tokenStorage.clear();

  Future<AuthSession> _loadSession() async {
    final profile = await fetchProfile();

    if (profile.isPlatformAdmin || profile.tenantId == null) {
      await _tokenStorage.clear();
      throw const AuthException(
        'Platform admin accounts use the web console. Sign in with a shop user.',
      );
    }

    final branch = await _pinBranch();
    return AuthSession(profile: profile, branch: branch);
  }

  Future<BranchSummary> _pinBranch() async {
    final response = await _dio.get<List<dynamic>>('/api/tenancy/branches');
    final branches = response.data!
        .cast<Map<String, dynamic>>()
        .map(BranchSummary.fromJson)
        .toList();

    if (branches.isEmpty) {
      throw const AuthException('No branch assigned to this account.');
    }

    final storedBranchId = await _tokenStorage.readBranchId();
    BranchSummary? storedMatch;
    if (storedBranchId != null) {
      for (final item in branches) {
        if (item.id == storedBranchId) {
          storedMatch = item;
          break;
        }
      }
    }

    final selected = storedMatch ?? branches.first;
    await _tokenStorage.saveBranchId(selected.id);
    return selected;
  }
}

class AuthException implements Exception {
  const AuthException(this.message);

  final String message;

  @override
  String toString() => message;
}
