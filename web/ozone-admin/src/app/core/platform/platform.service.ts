import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ShopSummary } from './platform.models';

@Injectable({ providedIn: 'root' })
export class PlatformService {
  private readonly http = inject(HttpClient);

  getShops(): Observable<ShopSummary[]> {
    return this.http.get<ShopSummary[]>(`${environment.apiUrl}/api/platform/shops`);
  }
}
