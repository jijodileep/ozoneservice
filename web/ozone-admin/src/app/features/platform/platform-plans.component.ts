import { DecimalPipe } from '@angular/common';
import { Component, OnInit, TemplateRef, computed, inject, signal, viewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbAlertModule, NgbModal, NgbModalRef, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { CreatePlanRequest, PlanSummary, UpdatePlanRequest } from '../../core/platform/platform.models';
import { PlatformService } from '../../core/platform/platform.service';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog.component';
import { DEFAULT_PAGE_SIZE, clampPage, paginateSlice } from '../../shared/pagination.util';
import { TablePaginationComponent } from '../../shared/table-pagination.component';

@Component({
  selector: 'app-platform-plans',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe, NgbAlertModule, NgbTooltipModule, TablePaginationComponent],
  templateUrl: './platform-plans.component.html',
})
export class PlatformPlansComponent implements OnInit {
  private readonly platform = inject(PlatformService);
  private readonly fb = inject(FormBuilder);
  private readonly modal = inject(NgbModal);

  readonly editModalTpl = viewChild.required<TemplateRef<unknown>>('editModal');

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly plans = signal<PlanSummary[]>([]);
  readonly page = signal(1);
  readonly pageSize = signal(DEFAULT_PAGE_SIZE);
  readonly totalCount = computed(() => this.plans().length);
  readonly pagedPlans = computed(() =>
    paginateSlice(this.plans(), this.page(), this.pageSize()));
  readonly editingPlan = signal<PlanSummary | null>(null);

  readonly createForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    code: ['', Validators.required],
    maxUsers: [3, [Validators.required, Validators.min(1)]],
    maxBranches: [1, [Validators.required, Validators.min(1)]],
    maxDevicesPerUser: [1, [Validators.required, Validators.min(1)]],
    price: [999, [Validators.required, Validators.min(0)]],
    billingPeriodMonths: [1, [Validators.required, Validators.min(1)]],
    tierOrder: [1, [Validators.required, Validators.min(1)]],
    allowWebLogin: [true],
    allowMobileLogin: [true],
  });

  readonly editForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    maxUsers: [3, [Validators.required, Validators.min(1)]],
    maxBranches: [1, [Validators.required, Validators.min(1)]],
    maxDevicesPerUser: [1, [Validators.required, Validators.min(1)]],
    price: [0, [Validators.required, Validators.min(0)]],
    billingPeriodMonths: [1, [Validators.required, Validators.min(1)]],
    tierOrder: [1, [Validators.required, Validators.min(1)]],
    allowWebLogin: [true],
    allowMobileLogin: [true],
    isActive: [true],
  });

  ngOnInit(): void {
    this.loadPlans();
  }

  onPageChange(page: number): void {
    this.page.set(page);
  }

  private syncPage(): void {
    this.page.set(clampPage(this.page(), this.totalCount(), this.pageSize()));
  }

  loadPlans(): void {
    this.loading.set(true);
    this.platform.getPlans().subscribe({
      next: (plans) => {
        this.plans.set(plans);
        this.syncPage();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load plans.');
        this.loading.set(false);
      },
    });
  }

  createPlan(): void {
    if (this.createForm.invalid) {
      return;
    }
    this.saving.set(true);
    this.platform.createPlan(this.createForm.getRawValue() as CreatePlanRequest).subscribe({
      next: () => {
        this.saving.set(false);
        this.createForm.reset({
          name: '', code: '', maxUsers: 3, maxBranches: 1, maxDevicesPerUser: 1,
          price: 999, billingPeriodMonths: 1, tierOrder: 1, allowWebLogin: true, allowMobileLogin: true,
        });
        this.loadPlans();
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Could not create plan.');
      },
    });
  }

  openEdit(plan: PlanSummary): void {
    this.editingPlan.set(plan);
    this.editForm.patchValue({
      name: plan.name,
      maxUsers: plan.maxUsers,
      maxBranches: plan.maxBranches,
      maxDevicesPerUser: plan.maxDevicesPerUser,
      price: plan.price,
      billingPeriodMonths: plan.billingPeriodMonths,
      tierOrder: plan.tierOrder,
      allowWebLogin: plan.allowWebLogin,
      allowMobileLogin: plan.allowMobileLogin,
      isActive: plan.isActive,
    });
    this.modal.open(this.editModalTpl(), { centered: true, backdrop: 'static' });
  }

  saveEdit(modal: NgbModalRef): void {
    const plan = this.editingPlan();
    if (!plan || this.editForm.invalid) {
      return;
    }
    this.saving.set(true);
    this.platform.updatePlan(plan.id, this.editForm.getRawValue() as UpdatePlanRequest).subscribe({
      next: () => {
        this.saving.set(false);
        this.editingPlan.set(null);
         modal.close();
        this.loadPlans();
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Could not update plan.');
      },
    });
  }

  deletePlan(plan: PlanSummary): void {
    const ref = this.modal.open(ConfirmDialogComponent, { centered: true });
    ref.componentInstance.title = 'Delete plan';
    ref.componentInstance.message =
      plan.tenantCount > 0
        ? `Cannot delete ${plan.name} — ${plan.tenantCount} shop(s) use this plan.`
        : `Permanently delete the "${plan.name}" plan?`;
    ref.componentInstance.confirmLabel = plan.tenantCount > 0 ? 'OK' : 'Delete';
    ref.componentInstance.confirmClass = plan.tenantCount > 0 ? 'btn-secondary' : 'btn-danger';

    ref.result.then((confirmed) => {
      if (!confirmed || plan.tenantCount > 0) {
        return;
      }
      this.platform.deletePlan(plan.id).subscribe({
        next: () => this.loadPlans(),
        error: (err) => this.error.set(err.error?.message ?? 'Could not delete plan.'),
      });
    }).catch(() => undefined);
  }
}
