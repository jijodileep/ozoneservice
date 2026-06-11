import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult, PaginationParams } from '../models/pagination.models';

export interface InvoiceSummary {
  id: string;
  invoiceNumber: string;
  customerName: string;
  customerPhone: string;
  subTotal: number;
  cgstAmount: number;
  sgstAmount: number;
  taxAmount: number;
  totalAmount: number;
  invoiceType: string;
  issuedAt: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly http = inject(HttpClient);

  getInvoicesPaged(params: PaginationParams): Observable<PagedResult<InvoiceSummary>> {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);
    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }
    return this.http.get<PagedResult<InvoiceSummary>>(`${environment.apiUrl}/api/invoices`, {
      params: httpParams,
    });
  }

  pdfUrl(id: string): string {
    return `${environment.apiUrl}/api/invoices/${id}/pdf`;
  }
}
