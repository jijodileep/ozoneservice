import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { NgbAlertModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { InvoiceService, InvoiceSummary } from '../../core/invoice/invoice.service';
import { AuthService } from '../../core/auth/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [DecimalPipe, DatePipe, NgbAlertModule, NgbTooltipModule],
  templateUrl: './invoices.component.html',
})
export class InvoicesComponent implements OnInit {
  private readonly invoiceService = inject(InvoiceService);
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly invoices = signal<InvoiceSummary[]>([]);

  ngOnInit(): void {
    this.invoiceService.getInvoices().subscribe({
      next: (items) => {
        this.invoices.set(items);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load invoices.');
        this.loading.set(false);
      },
    });
  }

  viewPdf(invoice: InvoiceSummary): void {
    const token = this.auth.getAccessToken();
    if (!token) {
      return;
    }

    this.http
      .get(`${environment.apiUrl}/api/invoices/${invoice.id}/pdf`, {
        headers: { Authorization: `Bearer ${token}` },
        responseType: 'blob',
      })
      .subscribe({
        next: (blob) => window.open(URL.createObjectURL(blob), '_blank'),
      });
  }
}
