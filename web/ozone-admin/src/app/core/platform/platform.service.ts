import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreatePlanRequest, PlanSummary, ShopSummary, UpdatePlanRequest } from './platform.models';

@Injectable({ providedIn: 'root' })
export class PlatformService {
  private readonly http = inject(HttpClient);

  getShops(): Observable<ShopSummary[]> {
    return this.http.get<ShopSummary[]>(`${environment.apiUrl}/api/platform/shops`);
  }

  getPlans(): Observable<PlanSummary[]> {
    return this.http.get<PlanSummary[]>(`${environment.apiUrl}/api/platform/plans`);
  }

  createPlan(request: CreatePlanRequest): Observable<PlanSummary> {
    return this.http.post<PlanSummary>(`${environment.apiUrl}/api/platform/plans`, request);
  }

  updatePlan(id: string, request: UpdatePlanRequest): Observable<PlanSummary> {
    return this.http.put<PlanSummary>(`${environment.apiUrl}/api/platform/plans/${id}`, request);
  }

  deletePlan(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/platform/plans/${id}`);
  }

  assignPlan(shopId: string, subscriptionPlanId: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/platform/shops/${shopId}/assign-plan`, {
      subscriptionPlanId,
    });
  }
}
