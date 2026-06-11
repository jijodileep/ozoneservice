import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { PlanSummary, UpgradeRequestSummary } from '../../core/platform/platform.models';
import { SubscriptionService } from '../../core/subscription/subscription.service';

@Component({
  selector: 'app-subscription',
  standalone: true,
  imports: [DecimalPipe, DatePipe, NgbAlertModule],
  templateUrl: './subscription.component.html',
})
export class SubscriptionComponent implements OnInit {
  private readonly subscription = inject(SubscriptionService);

  readonly currentPlanName = signal('');
  readonly expiresAt = signal<string | null>(null);
  readonly upgradeOptions = signal<PlanSummary[]>([]);
  readonly pendingRequest = signal<UpgradeRequestSummary | null>(null);
  readonly requestHistory = signal<UpgradeRequestSummary[]>([]);
  readonly message = signal<string | null>(null);
  readonly error = signal<string | null>(null);
  readonly requesting = signal(false);

  ngOnInit(): void {
    this.loadOptions();
    this.subscription.getUpgradeRequests().subscribe({
      next: (items) => this.requestHistory.set(items.filter((r) => r.status !== 'Pending')),
    });
  }

  loadOptions(): void {
    this.subscription.getOptions().subscribe({
      next: (options) => {
        this.currentPlanName.set(options.currentPlanName);
        this.expiresAt.set(options.subscriptionExpiresAt);
        this.upgradeOptions.set(options.upgradeOptions);
        this.pendingRequest.set(options.pendingRequest);
      },
      error: () => this.error.set('Could not load subscription options.'),
    });
  }

  requestUpgrade(plan: PlanSummary): void {
    this.message.set(null);
    this.error.set(null);
    this.requesting.set(true);
    this.subscription.requestUpgrade(plan.id).subscribe({
      next: (request) => {
        this.requesting.set(false);
        this.pendingRequest.set(request);
        this.message.set(`Upgrade to ${plan.name} requested. Awaiting super admin approval.`);
      },
      error: (err) => {
        this.requesting.set(false);
        this.error.set(err.error?.message ?? 'Could not submit upgrade request.');
      },
    });
  }
}
