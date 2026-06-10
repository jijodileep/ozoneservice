import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { JwtPayload, TokenResponse, UserProfile } from './auth.models';

const ACCESS_TOKEN_KEY = 'ozone_access_token';
const REFRESH_TOKEN_KEY = 'ozone_refresh_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  readonly profile = signal<UserProfile | null>(null);

  login(email: string, password: string): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>(`${environment.apiUrl}/api/auth/login`, { email, password })
      .pipe(
        tap((tokens) => {
          sessionStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
          sessionStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
        }),
      );
  }

  loadProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${environment.apiUrl}/api/auth/me`).pipe(
      tap((profile) => this.profile.set(profile)),
    );
  }

  logout(): void {
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
    this.profile.set(null);
    void this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem(ACCESS_TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getAccessToken();
    if (!token) {
      return false;
    }

    const payload = this.decodeToken(token);
    if (!payload?.exp) {
      return false;
    }

    return payload.exp * 1000 > Date.now();
  }

  getRoles(): string[] {
    const fromProfile = this.profile()?.roles;
    if (fromProfile?.length) {
      return fromProfile;
    }

    const token = this.getAccessToken();
    if (!token) {
      return [];
    }

    const payload = this.decodeToken(token);
    const roleClaim =
      payload?.role ??
      payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!roleClaim) {
      return [];
    }

    return Array.isArray(roleClaim) ? roleClaim : [roleClaim];
  }

  hasAnyRole(roles: string[]): boolean {
    const userRoles = this.getRoles();
    return roles.some((role) => userRoles.includes(role));
  }

  isPlatformAdmin(): boolean {
    return this.hasAnyRole(['PlatformSuperAdmin']);
  }

  private decodeToken(token: string): JwtPayload | null {
    try {
      const payload = token.split('.')[1];
      const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(decoded) as JwtPayload;
    } catch {
      return null;
    }
  }
}
