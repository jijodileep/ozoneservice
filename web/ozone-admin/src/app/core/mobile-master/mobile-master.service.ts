import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult, PaginationParams } from '../models/pagination.models';
import { MobileBrand, MobileModel, MobileVariant } from './mobile-master.models';

@Injectable({ providedIn: 'root' })
export class MobileMasterService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/api/mobile-masters`;

  getBrandsPaged(params: PaginationParams): Observable<PagedResult<MobileBrand>> {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);
    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }
    return this.http.get<PagedResult<MobileBrand>>(`${this.base}/brands`, { params: httpParams });
  }

  getModels(brandId: string): Observable<MobileModel[]> {
    return this.http.get<MobileModel[]>(`${this.base}/brands/${brandId}/models`);
  }

  getVariants(modelId: string): Observable<MobileVariant[]> {
    return this.http.get<MobileVariant[]>(`${this.base}/models/${modelId}/variants`);
  }
}
