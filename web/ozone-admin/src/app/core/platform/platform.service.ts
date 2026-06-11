import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { PagedResult, PaginationParams } from '../models/pagination.models';
import {
  CreatePlanRequest,
  PlanSummary,
  ShopSummary,
  TaxConfiguration,
  UpdatePlanRequest,
  UpdateTaxConfigurationRequest,
  UpgradeRequestSummary,
} from './platform.models';

@Injectable({ providedIn: 'root' })
export class PlatformService {
  private readonly http = inject(HttpClient);

  getShopsPaged(params: PaginationParams): Observable<PagedResult<ShopSummary>> {
    return this.http.get<PagedResult<ShopSummary>>(`${environment.apiUrl}/api/platform/shops`, {
      params: this.toParams(params),
    });
  }

  getShops(): Observable<ShopSummary[]> {
    return this.getShopsPaged({ page: 1, pageSize: 500 }).pipe(map((r) => r.items));
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

  getTaxConfiguration(): Observable<TaxConfiguration> {
    return this.http.get<TaxConfiguration>(`${environment.apiUrl}/api/platform/tax`);
  }

  updateTaxConfiguration(request: UpdateTaxConfigurationRequest): Observable<TaxConfiguration> {
    return this.http.put<TaxConfiguration>(`${environment.apiUrl}/api/platform/tax`, request);
  }

  getUpgradeRequestsPaged(
    params: PaginationParams & { status?: string },
  ): Observable<PagedResult<UpgradeRequestSummary>> {
    let httpParams = this.toParams(params);
    if (params.status?.trim()) {
      httpParams = httpParams.set('status', params.status.trim());
    }
    return this.http.get<PagedResult<UpgradeRequestSummary>>(
      `${environment.apiUrl}/api/platform/upgrade-requests`,
      { params: httpParams },
    );
  }

  approveUpgradeRequest(id: string): Observable<UpgradeRequestSummary> {
    return this.http.post<UpgradeRequestSummary>(
      `${environment.apiUrl}/api/platform/upgrade-requests/${id}/approve`,
      {},
    );
  }

  rejectUpgradeRequest(id: string, reason?: string): Observable<UpgradeRequestSummary> {
    return this.http.post<UpgradeRequestSummary>(
      `${environment.apiUrl}/api/platform/upgrade-requests/${id}/reject`,
      { reason },
    );
  }

  private toParams(params: PaginationParams): HttpParams {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);
    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }
    return httpParams;
  }
}
