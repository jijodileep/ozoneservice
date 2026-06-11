import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { UpgradeRequestSummary } from '../../core/platform/platform.models';
import { PlatformService } from '../../core/platform/platform.service';
import { TablePaginationComponent } from '../../shared/table-pagination.component';

@Component({
  selector: 'app-platform-upgrade-requests',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FormsModule, NgbAlertModule, TablePaginationComponent],
  templateUrl: './platform-upgrade-requests.component.html',
})
export class PlatformUpgradeRequestsComponent implements OnInit {
  private readonly platform = inject(PlatformService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly flash = signal<string | null>(null);
  readonly requests = signal<UpgradeRequestSummary[]>([]);
  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly totalCount = signal(0);
  readonly statusFilter = signal('Pending');

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.loading.set(true);
    this.platform
      .getUpgradeRequestsPaged({
        page: this.page(),
        pageSize: this.pageSize(),
        status: this.statusFilter(),
      })
      .subscribe({
        next: (result) => {
          this.requests.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Could not load upgrade requests.');
          this.loading.set(false);
        },
      });
  }

  onFilterChange(): void {
    this.page.set(1);
    this.loadRequests();
  }

  onPageChange(page: number): void {
    this.page.set(page);
    this.loadRequests();
  }

  approve(request: UpgradeRequestSummary): void {
    this.platform.approveUpgradeRequest(request.id).subscribe({
      next: (result) => {
        this.flash.set(
          `Approved ${request.tenantName} → ${result.requestedPlanName}. Invoice ${result.invoiceNumber ?? ''} generated.`,
        );
        this.loadRequests();
      },
      error: (err) => this.flash.set(err.error?.message ?? 'Could not approve request.'),
    });
  }

  reject(request: UpgradeRequestSummary): void {
    const reason = window.prompt('Rejection reason (optional):') ?? undefined;
    this.platform.rejectUpgradeRequest(request.id, reason).subscribe({
      next: () => {
        this.flash.set(`Rejected upgrade request for ${request.tenantName}.`);
        this.loadRequests();
      },
      error: (err) => this.flash.set(err.error?.message ?? 'Could not reject request.'),
    });
  }
}
