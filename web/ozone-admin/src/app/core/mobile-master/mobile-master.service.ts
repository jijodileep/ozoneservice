import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
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
} from './mobile-master.models';

@Injectable({ providedIn: 'root' })
export class MobileMasterService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/mobile-masters`;

  getBrands(): Observable<MobileBrand[]> {
    return this.http.get<MobileBrand[]>(`${this.base}/brands`);
  }

  createBrand(request: CreateMobileBrandRequest): Observable<MobileBrand> {
    return this.http.post<MobileBrand>(`${this.base}/brands`, request);
  }

  updateBrand(id: string, request: UpdateMobileBrandRequest): Observable<MobileBrand> {
    return this.http.put<MobileBrand>(`${this.base}/brands/${id}`, request);
  }

  deactivateBrand(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/brands/${id}`);
  }

  getModels(brandId: string): Observable<MobileModel[]> {
    return this.http.get<MobileModel[]>(`${this.base}/brands/${brandId}/models`);
  }

  createModel(brandId: string, request: CreateMobileModelRequest): Observable<MobileModel> {
    return this.http.post<MobileModel>(`${this.base}/brands/${brandId}/models`, request);
  }

  updateModel(id: string, request: UpdateMobileModelRequest): Observable<MobileModel> {
    return this.http.put<MobileModel>(`${this.base}/models/${id}`, request);
  }

  deactivateModel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/models/${id}`);
  }

  getVariants(modelId: string): Observable<MobileVariant[]> {
    return this.http.get<MobileVariant[]>(`${this.base}/models/${modelId}/variants`);
  }

  createVariant(modelId: string, request: CreateMobileVariantRequest): Observable<MobileVariant> {
    return this.http.post<MobileVariant>(`${this.base}/models/${modelId}/variants`, request);
  }

  updateVariant(id: string, request: UpdateMobileVariantRequest): Observable<MobileVariant> {
    return this.http.put<MobileVariant>(`${this.base}/variants/${id}`, request);
  }

  deactivateVariant(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/variants/${id}`);
  }
}
