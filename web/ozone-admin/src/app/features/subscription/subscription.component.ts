import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { PlanSummary } from '../../core/platform/platform.models';
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
  readonly message = signal<string | null>(null);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.subscription.getOptions().subscribe({
      next: (options) => {
        this.currentPlanName.set(options.currentPlanName);
        this.expiresAt.set(options.subscriptionExpiresAt);
        this.upgradeOptions.set(options.upgradeOptions);
      },
      error: () => this.error.set('Could not load subscription options.'),
    });
  }

  upgrade(plan: PlanSummary): void {
    this.message.set(null);
    this.error.set(null);
    this.subscription.upgrade(plan.id).subscribe({
      next: () => {
        this.message.set(`Upgraded to ${plan.name}. Subscription renewed.`);
        this.currentPlanName.set(plan.name);
        this.upgradeOptions.set(this.upgradeOptions().filter((p) => p.id !== plan.id));
      },
      error: (err) => this.error.set(err.error?.message ?? 'Upgrade failed.'),
    });
  }
}
