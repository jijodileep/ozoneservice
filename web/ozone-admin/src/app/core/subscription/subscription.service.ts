import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PlanSummary, UpgradeRequestSummary } from '../platform/platform.models';

export interface SubscriptionOptions {
  currentPlanId: string;
  currentPlanName: string;
  currentTierOrder: number;
  subscriptionExpiresAt: string | null;
  currentPlan: PlanSummary;
  upgradeOptions: PlanSummary[];
  pendingRequest: UpgradeRequestSummary | null;
}

@Injectable({ providedIn: 'root' })
export class SubscriptionService {
  private readonly http = inject(HttpClient);

  getOptions(): Observable<SubscriptionOptions> {
    return this.http.get<SubscriptionOptions>(`${environment.apiUrl}/api/subscription/options`);
  }

  requestUpgrade(planId: string): Observable<UpgradeRequestSummary> {
    return this.http.post<UpgradeRequestSummary>(`${environment.apiUrl}/api/subscription/upgrade-request`, {
      planId,
    });
  }

  getUpgradeRequests(): Observable<UpgradeRequestSummary[]> {
    return this.http.get<UpgradeRequestSummary[]>(`${environment.apiUrl}/api/subscription/upgrade-requests`);
  }
}
