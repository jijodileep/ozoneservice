import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  CreateMobileBrandRequest,
  CreateMobileModelRequest,
  CreateMobileVariantRequest,
  MobileBrand,
  MobileModel,
  MobileVariant,
  UpdateMobileBrandRequest,
  UpdateMobileModelRequest,
  UpdateMobileVariantRequest,
} from '../mobile-master/mobile-master.models';
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

  getMobileBrandsPaged(params: PaginationParams): Observable<PagedResult<MobileBrand>> {
    return this.http.get<PagedResult<MobileBrand>>(
      `${environment.apiUrl}/api/platform/mobile-masters/brands`,
      { params: this.toParams(params) },
    );
  }

  createMobileBrand(request: CreateMobileBrandRequest): Observable<MobileBrand> {
    return this.http.post<MobileBrand>(
      `${environment.apiUrl}/api/platform/mobile-masters/brands`,
      request,
    );
  }

  updateMobileBrand(id: string, request: UpdateMobileBrandRequest): Observable<MobileBrand> {
    return this.http.put<MobileBrand>(
      `${environment.apiUrl}/api/platform/mobile-masters/brands/${id}`,
      request,
    );
  }

  deactivateMobileBrand(id: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/api/platform/mobile-masters/brands/${id}`,
    );
  }

  getMobileModels(brandId: string): Observable<MobileModel[]> {
    return this.http.get<MobileModel[]>(
      `${environment.apiUrl}/api/platform/mobile-masters/brands/${brandId}/models`,
    );
  }

  createMobileModel(brandId: string, request: CreateMobileModelRequest): Observable<MobileModel> {
    return this.http.post<MobileModel>(
      `${environment.apiUrl}/api/platform/mobile-masters/brands/${brandId}/models`,
      request,
    );
  }

  updateMobileModel(id: string, request: UpdateMobileModelRequest): Observable<MobileModel> {
    return this.http.put<MobileModel>(
      `${environment.apiUrl}/api/platform/mobile-masters/models/${id}`,
      request,
    );
  }

  deactivateMobileModel(id: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/api/platform/mobile-masters/models/${id}`,
    );
  }

  getMobileVariants(modelId: string): Observable<MobileVariant[]> {
    return this.http.get<MobileVariant[]>(
      `${environment.apiUrl}/api/platform/mobile-masters/models/${modelId}/variants`,
    );
  }

  createMobileVariant(
    modelId: string,
    request: CreateMobileVariantRequest,
  ): Observable<MobileVariant> {
    return this.http.post<MobileVariant>(
      `${environment.apiUrl}/api/platform/mobile-masters/models/${modelId}/variants`,
      request,
    );
  }

  updateMobileVariant(id: string, request: UpdateMobileVariantRequest): Observable<MobileVariant> {
    return this.http.put<MobileVariant>(
      `${environment.apiUrl}/api/platform/mobile-masters/variants/${id}`,
      request,
    );
  }

  deactivateMobileVariant(id: string): Observable<void> {
    return this.http.delete<void>(
      `${environment.apiUrl}/api/platform/mobile-masters/variants/${id}`,
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
