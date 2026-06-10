import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface InvoiceSummary {
  id: string;
  invoiceNumber: string;
  customerName: string;
  customerPhone: string;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  issuedAt: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly http = inject(HttpClient);

  getInvoices(): Observable<InvoiceSummary[]> {
    return this.http.get<InvoiceSummary[]>(`${environment.apiUrl}/api/invoices`);
  }

  pdfUrl(id: string): string {
    return `${environment.apiUrl}/api/invoices/${id}/pdf`;
  }
}
