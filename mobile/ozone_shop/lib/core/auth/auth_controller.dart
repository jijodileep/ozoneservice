import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../api/api_client.dart';
import 'auth_service.dart';
import 'models.dart';
import 'token_storage.dart';

final tokenStorageProvider = Provider<TokenStorage>((ref) => TokenStorage());

final apiClientProvider = Provider<ApiClient>((ref) {
  return ApiClient(tokenStorage: ref.watch(tokenStorageProvider));
});

final authServiceProvider = Provider<AuthService>((ref) {
  return AuthService(
    ref.watch(apiClientProvider),
    ref.watch(tokenStorageProvider),
  );
});

final authControllerProvider =
    AsyncNotifierProvider<AuthController, AuthSession?>(AuthController.new);

class AuthController extends AsyncNotifier<AuthSession?> {
  AuthService get _auth => ref.read(authServiceProvider);

  @override
  Future<AuthSession?> build() => _auth.restoreSession();

  Future<void> login(String email, String password) async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(() => _auth.login(email, password));
  }

  Future<void> refreshProfile() async {
    final current = state.value;
    if (current == null) {
      return;
    }

    state = const AsyncLoading();
    state = await AsyncValue.guard(() async {
      final profile = await _auth.fetchProfile();
      return AuthSession(profile: profile, branch: current.branch);
    });
  }

  Future<void> logout() async {
    await _auth.logout();
    state = const AsyncData(null);
  }
}
