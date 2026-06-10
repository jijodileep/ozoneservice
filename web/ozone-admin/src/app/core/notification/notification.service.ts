import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface NotificationItem {
  id: string;
  tenantId: string | null;
  title: string;
  message: string;
  notificationType: string;
  isRead: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly http = inject(HttpClient);

  getNotifications(): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(`${environment.apiUrl}/api/notifications`);
  }

  markRead(id: string): Observable<void> {
    return this.http.patch<void>(`${environment.apiUrl}/api/notifications/${id}/read`, {});
  }
}
