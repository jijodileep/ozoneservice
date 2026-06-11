import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbAlertModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { InvoiceService, InvoiceSummary } from '../../core/invoice/invoice.service';
import { AuthService } from '../../core/auth/auth.service';
import { environment } from '../../../environments/environment';
import { TablePaginationComponent } from '../../shared/table-pagination.component';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [DecimalPipe, DatePipe, FormsModule, NgbAlertModule, NgbTooltipModule, TablePaginationComponent],
  templateUrl: './invoices.component.html',
})
export class InvoicesComponent implements OnInit {
  private readonly invoiceService = inject(InvoiceService);
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly invoices = signal<InvoiceSummary[]>([]);
  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly totalCount = signal(0);
  readonly search = signal('');

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.loading.set(true);
    this.invoiceService
      .getInvoicesPaged({
        page: this.page(),
        pageSize: this.pageSize(),
        search: this.search(),
      })
      .subscribe({
        next: (result) => {
          this.invoices.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Could not load invoices.');
          this.loading.set(false);
        },
      });
  }

  onSearch(): void {
    this.page.set(1);
    this.loadInvoices();
  }

  onPageChange(page: number): void {
    this.page.set(page);
    this.loadInvoices();
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
