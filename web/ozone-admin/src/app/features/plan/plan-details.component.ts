import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { SubscriptionService } from '../../core/subscription/subscription.service';

@Component({
  selector: 'app-plan-details',
  standalone: true,
  imports: [DatePipe, DecimalPipe, NgbAlertModule],
  template: `
    <div class="content-card">
      <div class="card-header-custom">
        <h2><i class="bi bi-card-checklist me-2"></i>Plan details</h2>
        <p>Your shop subscription and plan limits.</p>
      </div>
      <div class="card-body-custom">
        @if (error()) {
          <ngb-alert type="danger" (closed)="error.set(null)">{{ error() }}</ngb-alert>
        } @else if (loading()) {
          <div class="text-center py-4">
            <div class="spinner-border text-primary"></div>
          </div>
        } @else {
          <div class="row g-4">
            <div class="col-md-6">
              <div class="border rounded p-3 h-100">
                <div class="text-muted small mb-1">Current plan</div>
                <div class="fs-5 fw-semibold">{{ currentPlanName() }}</div>
                @if (expiresAt()) {
                  <div class="text-muted small mt-2">
                    <i class="bi bi-calendar-event me-1"></i>
                    Renews / expires {{ expiresAt() | date: 'longDate' }}
                  </div>
                }
              </div>
            </div>
            @if (currentPlan(); as plan) {
              <div class="col-md-6">
                <div class="border rounded p-3 h-100">
                  <div class="text-muted small mb-2">Included in plan</div>
                  <ul class="list-unstyled mb-0 small">
                    <li class="mb-2">
                      <i class="bi bi-people me-2 text-primary"></i>
                      Up to {{ plan.maxUsers }} users
                    </li>
                    <li class="mb-2">
                      <i class="bi bi-shop me-2 text-primary"></i>
                      Up to {{ plan.maxBranches }} branches
                    </li>
                    <li>
                      <i class="bi bi-currency-rupee me-2 text-primary"></i>
                      ₹{{ plan.price | number: '1.0-0' }} per {{ plan.billingPeriodMonths }} month(s)
                    </li>
                  </ul>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
})
export class PlanDetailsComponent implements OnInit {
  private readonly subscription = inject(SubscriptionService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly currentPlanName = signal('—');
  readonly expiresAt = signal<string | null>(null);
  readonly currentPlan = signal<{
    maxUsers: number;
    maxBranches: number;
    price: number;
    billingPeriodMonths: number;
  } | null>(null);

  ngOnInit(): void {
    this.subscription.getOptions().subscribe({
      next: (options) => {
        this.currentPlanName.set(options.currentPlanName);
        this.expiresAt.set(options.subscriptionExpiresAt);
        this.currentPlan.set({
          maxUsers: options.currentPlan.maxUsers,
          maxBranches: options.currentPlan.maxBranches,
          price: options.currentPlan.price,
          billingPeriodMonths: options.currentPlan.billingPeriodMonths,
        });
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load plan details.');
        this.loading.set(false);
      },
    });
  }
}
