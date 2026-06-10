import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { PlanSummary, ShopSummary } from '../../core/platform/platform.models';
import { PlatformService } from '../../core/platform/platform.service';

@Component({
  selector: 'app-platform-dashboard',
  standalone: true,
  imports: [DatePipe, NgbAlertModule],
  templateUrl: './platform-dashboard.component.html',
})
export class PlatformDashboardComponent implements OnInit {
  private readonly platform = inject(PlatformService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly shops = signal<ShopSummary[]>([]);
  readonly plans = signal<PlanSummary[]>([]);
  readonly flash = signal<string | null>(null);

  ngOnInit(): void {
    this.platform.getPlans().subscribe({ next: (plans) => this.plans.set(plans) });
    this.platform.getShops().subscribe({
      next: (shops) => {
        this.shops.set(shops);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load shops.');
        this.loading.set(false);
      },
    });
  }

  usageLabel(shop: ShopSummary): string {
    return `${shop.userCount}/${shop.maxUsers} users · ${shop.branchCount}/${shop.maxBranches} branches`;
  }

  isOverLimit(shop: ShopSummary): boolean {
    return shop.userCount > shop.maxUsers || shop.branchCount > shop.maxBranches;
  }

  planIdForShop(shop: ShopSummary): string | undefined {
    return this.plans().find((p) => p.code === shop.planCode)?.id;
  }

  onAssignPlan(shop: ShopSummary, event: Event): void {
    const planId = (event.target as HTMLSelectElement).value;
    this.platform.assignPlan(shop.id, planId).subscribe({
      next: () => {
        this.flash.set(`Plan updated for ${shop.name}`);
        this.reloadShops();
      },
      error: (err) => this.flash.set(err.error?.message ?? 'Could not assign plan.'),
    });
  }

  private reloadShops(): void {
    this.platform.getShops().subscribe({ next: (shops) => this.shops.set(shops) });
  }
}
