import { Component, OnInit, TemplateRef, inject, signal, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { NgbAlertModule, NgbModal, NgbModalRef, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../core/auth/auth.service';
import { BranchDetail, CreateBranchRequest, UpdateBranchRequest } from '../../core/branch/branch.models';
import { BranchService } from '../../core/branch/branch.service';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog.component';

@Component({
  selector: 'app-branches',
  standalone: true,
  imports: [ReactiveFormsModule, NgbAlertModule, NgbTooltipModule],
  templateUrl: './branches.component.html',
})
export class BranchesComponent implements OnInit {
  private readonly branchService = inject(BranchService);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly modal = inject(NgbModal);

  readonly editModalTpl = viewChild.required<TemplateRef<unknown>>('editModal');

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly branches = signal<BranchDetail[]>([]);
  readonly editingBranch = signal<BranchDetail | null>(null);

  readonly createForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[A-Za-z0-9_-]+$/)]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    address: ['', Validators.maxLength(500)],
    phone: ['', Validators.maxLength(20)],
    gstNumber: ['', Validators.maxLength(20)],
  });

  readonly editForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    address: ['', Validators.maxLength(500)],
    phone: ['', Validators.maxLength(20)],
    gstNumber: ['', Validators.maxLength(20)],
    isActive: [true],
  });

  ngOnInit(): void {
    this.loadBranches();
  }

  isTenantAdmin(): boolean {
    return this.auth.hasAnyRole(['TenantAdmin']);
  }

  loadBranches(): void {
    this.loading.set(true);
    this.branchService.getBranches().subscribe({
      next: (branches) => {
        this.branches.set(branches);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load branches.');
        this.loading.set(false);
      },
    });
  }

  createBranch(): void {
    this.error.set(null);
    this.normalizeCreateForm();

    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      this.error.set('Please fix the highlighted fields.');
      return;
    }

    const raw = this.createForm.getRawValue();
    const request: CreateBranchRequest = {
      code: raw.code,
      name: raw.name,
      address: raw.address || null,
      phone: raw.phone || null,
      gstNumber: raw.gstNumber || null,
    };

    this.saving.set(true);
    this.branchService.createBranch(request).subscribe({
      next: () => {
        this.saving.set(false);
        this.createForm.reset({ code: '', name: '', address: '', phone: '', gstNumber: '' });
        this.loadBranches();
        this.branchService.loadBranches().subscribe();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not create branch.'));
      },
    });
  }

  openEdit(branch: BranchDetail): void {
    this.editingBranch.set(branch);
    this.editForm.patchValue({
      name: branch.name,
      address: branch.address ?? '',
      phone: branch.phone ?? '',
      gstNumber: branch.gstNumber ?? '',
      isActive: branch.isActive,
    });
    this.modal.open(this.editModalTpl(), { centered: true, backdrop: 'static' });
  }

  saveEdit(modal: NgbModalRef): void {
    const branch = this.editingBranch();
    this.error.set(null);
    this.normalizeEditForm();

    if (!branch || this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      if (this.editForm.invalid) {
        this.error.set('Please fix the highlighted fields.');
      }
      return;
    }

    const raw = this.editForm.getRawValue();
    const request: UpdateBranchRequest = {
      name: raw.name,
      address: raw.address || null,
      phone: raw.phone || null,
      gstNumber: raw.gstNumber || null,
      isActive: raw.isActive,
    };

    this.saving.set(true);
    this.branchService.updateBranch(branch.id, request).subscribe({
      next: () => {
        this.saving.set(false);
        this.editingBranch.set(null);
        modal.close();
        this.loadBranches();
        this.branchService.loadBranches().subscribe();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not update branch.'));
      },
    });
  }

  isInvalid(form: FormGroup, controlName: string): boolean {
    const control = form.get(controlName);
    return !!(control?.invalid && (control.touched || control.dirty));
  }

  fieldError(form: FormGroup, controlName: string): string | null {
    const control = form.get(controlName);
    if (!control?.errors || !(control.touched || control.dirty)) {
      return null;
    }

    if (control.errors['required']) {
      return 'This field is required.';
    }

    if (control.errors['maxlength']) {
      return `Maximum ${control.errors['maxlength'].requiredLength} characters.`;
    }

    if (control.errors['pattern']) {
      return 'Invalid value.';
    }

    return 'Invalid value.';
  }

  private normalizeCreateForm(): void {
    this.createForm.patchValue({
      code: this.createForm.controls.code.value.trim().toUpperCase(),
      name: this.createForm.controls.name.value.trim(),
      address: this.createForm.controls.address.value.trim(),
      phone: this.createForm.controls.phone.value.trim(),
      gstNumber: this.createForm.controls.gstNumber.value.trim(),
    });
    this.createForm.updateValueAndValidity();
  }

  private normalizeEditForm(): void {
    this.editForm.patchValue({
      name: this.editForm.controls.name.value.trim(),
      address: this.editForm.controls.address.value.trim(),
      phone: this.editForm.controls.phone.value.trim(),
      gstNumber: this.editForm.controls.gstNumber.value.trim(),
    });
    this.editForm.updateValueAndValidity();
  }

  private formatApiError(err: unknown, fallback: string): string {
    if (!(err instanceof HttpErrorResponse)) {
      return fallback;
    }

    const body = err.error as { message?: string; errors?: Record<string, string[]> } | null;
    if (body?.message) {
      return body.message;
    }

    if (body?.errors) {
      const messages = Object.values(body.errors).flat();
      if (messages.length) {
        return messages.join(' ');
      }
    }

    return fallback;
  }

  deactivateBranch(branch: BranchDetail): void {
    const ref = this.modal.open(ConfirmDialogComponent, { centered: true });
    ref.componentInstance.title = 'Deactivate branch';
    ref.componentInstance.message = `Deactivate "${branch.name}"? It will no longer appear in the branch selector.`;
    ref.componentInstance.confirmLabel = 'Deactivate';

    ref.result.then((confirmed) => {
      if (!confirmed) {
        return;
      }

      this.branchService.deactivateBranch(branch.id).subscribe({
        next: () => {
          this.loadBranches();
          this.branchService.loadBranches().subscribe();
        },
        error: (err) => this.error.set(err.error?.message ?? 'Could not deactivate branch.'),
      });
    }).catch(() => undefined);
  }
}
