export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}

export interface UserProfile {
  id: string;
  email: string;
  displayName: string;
  tenantId: string | null;
  roles: string[];
}

export interface JwtPayload {
  sub?: string;
  email?: string;
  role?: string | string[];
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
  tenant_id?: string;
  exp?: number;
}
