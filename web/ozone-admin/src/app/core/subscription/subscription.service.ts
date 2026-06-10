import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PlanSummary } from '../platform/platform.models';

export interface SubscriptionOptions {
  currentPlanId: string;
  currentPlanName: string;
  currentTierOrder: number;
  subscriptionExpiresAt: string | null;
  upgradeOptions: PlanSummary[];
}

@Injectable({ providedIn: 'root' })
export class SubscriptionService {
  private readonly http = inject(HttpClient);

  getOptions(): Observable<SubscriptionOptions> {
    return this.http.get<SubscriptionOptions>(`${environment.apiUrl}/api/subscription/options`);
  }

  upgrade(planId: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/subscription/upgrade`, { planId });
  }
}
