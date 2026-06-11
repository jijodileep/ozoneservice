import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateUserRequest,
  ResetUserPasswordRequest,
  UpdateUserRequest,
  UserDetail,
} from './user.models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);

  getUsers(): Observable<UserDetail[]> {
    return this.http.get<UserDetail[]>(`${environment.apiUrl}/api/users`);
  }

  createUser(request: CreateUserRequest): Observable<UserDetail> {
    return this.http.post<UserDetail>(`${environment.apiUrl}/api/users`, request);
  }

  updateUser(id: string, request: UpdateUserRequest): Observable<UserDetail> {
    return this.http.put<UserDetail>(`${environment.apiUrl}/api/users/${id}`, request);
  }

  resetPassword(id: string, request: ResetUserPasswordRequest): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/users/${id}/reset-password`, request);
  }
}
