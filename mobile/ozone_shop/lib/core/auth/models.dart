class TokenResponse {
  const TokenResponse({
    required this.accessToken,
    required this.refreshToken,
    required this.accessTokenExpiresAt,
    required this.refreshTokenExpiresAt,
  });

  final String accessToken;
  final String refreshToken;
  final DateTime accessTokenExpiresAt;
  final DateTime refreshTokenExpiresAt;

  factory TokenResponse.fromJson(Map<String, dynamic> json) {
    return TokenResponse(
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      accessTokenExpiresAt: DateTime.parse(json['accessTokenExpiresAt'] as String),
      refreshTokenExpiresAt: DateTime.parse(json['refreshTokenExpiresAt'] as String),
    );
  }
}

class UserProfile {
  const UserProfile({
    required this.id,
    required this.email,
    required this.displayName,
    required this.tenantId,
    required this.roles,
  });

  final String id;
  final String email;
  final String displayName;
  final String? tenantId;
  final List<String> roles;

  factory UserProfile.fromJson(Map<String, dynamic> json) {
    return UserProfile(
      id: json['id'] as String,
      email: json['email'] as String,
      displayName: json['displayName'] as String,
      tenantId: json['tenantId'] as String?,
      roles: (json['roles'] as List<dynamic>).cast<String>(),
    );
  }

  bool get isPlatformAdmin => roles.contains('PlatformSuperAdmin');
}

class BranchSummary {
  const BranchSummary({
    required this.id,
    required this.code,
    required this.name,
    required this.tenantId,
  });

  final String id;
  final String code;
  final String name;
  final String tenantId;

  factory BranchSummary.fromJson(Map<String, dynamic> json) {
    return BranchSummary(
      id: json['id'] as String,
      code: json['code'] as String,
      name: json['name'] as String,
      tenantId: json['tenantId'] as String,
    );
  }
}

class AuthSession {
  const AuthSession({
    required this.profile,
    required this.branch,
  });

  final UserProfile profile;
  final BranchSummary branch;
}
