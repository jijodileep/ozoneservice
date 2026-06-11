import { Component, OnInit, TemplateRef, computed, inject, signal, viewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { NgbAlertModule, NgbModal, NgbModalRef, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../core/auth/auth.service';
import { BranchDetail, CreateBranchRequest, UpdateBranchRequest } from '../../core/branch/branch.models';
import { BranchService } from '../../core/branch/branch.service';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog.component';
import { DEFAULT_PAGE_SIZE, clampPage, paginateSlice } from '../../shared/pagination.util';
import { TablePaginationComponent } from '../../shared/table-pagination.component';

const GSTIN_PATTERN = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$/;

function optionalPhoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const raw = (control.value ?? '').trim();
    if (!raw) {
      return null;
    }

    let digits = raw.replace(/\D/g, '');
    if (digits.length === 12 && digits.startsWith('91')) {
      digits = digits.slice(2);
    }

    return digits.length === 10 ? null : { phone: true };
  };
}

function optionalGstValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const raw = (control.value ?? '').trim().toUpperCase().replace(/\s/g, '');
    if (!raw) {
      return null;
    }

    return GSTIN_PATTERN.test(raw) ? null : { gst: true };
  };
}

@Component({
  selector: 'app-branches',
  standalone: true,
  imports: [ReactiveFormsModule, NgbAlertModule, NgbTooltipModule, TablePaginationComponent],
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
  readonly page = signal(1);
  readonly pageSize = signal(DEFAULT_PAGE_SIZE);
  readonly totalCount = computed(() => this.branches().length);
  readonly pagedBranches = computed(() =>
    paginateSlice(this.branches(), this.page(), this.pageSize()));
  readonly editingBranch = signal<BranchDetail | null>(null);

  readonly createForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.maxLength(50), Validators.pattern(/^[A-Za-z0-9_-]+$/)]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    address: ['', Validators.maxLength(500)],
    phone: ['', optionalPhoneValidator()],
    gstNumber: ['', optionalGstValidator()],
  });

  readonly editForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    address: ['', Validators.maxLength(500)],
    phone: ['', optionalPhoneValidator()],
    gstNumber: ['', optionalGstValidator()],
    isActive: [true],
  });

  ngOnInit(): void {
    this.loadBranches();
  }

  isTenantAdmin(): boolean {
    return this.auth.hasAnyRole(['TenantAdmin']);
  }

  onPageChange(page: number): void {
    this.page.set(page);
  }

  private syncPage(): void {
    this.page.set(clampPage(this.page(), this.totalCount(), this.pageSize()));
  }

  loadBranches(): void {
    this.loading.set(true);
    this.branchService.getBranches().subscribe({
      next: (branches) => {
        this.branches.set(branches);
        this.syncPage();
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

    if (control.errors['phone']) {
      return 'Enter a valid 10-digit phone number.';
    }

    if (control.errors['gst']) {
      return 'Enter a valid 15-character GSTIN.';
    }

    return 'Invalid value.';
  }

  private normalizeCreateForm(): void {
    const phone = this.normalizePhoneInput(this.createForm.controls.phone.value);
    const gstNumber = this.normalizeGstInput(this.createForm.controls.gstNumber.value);

    this.createForm.patchValue({
      code: this.createForm.controls.code.value.trim().toUpperCase(),
      name: this.createForm.controls.name.value.trim(),
      address: this.createForm.controls.address.value.trim(),
      phone,
      gstNumber,
    });
    this.createForm.updateValueAndValidity();
  }

  private normalizeEditForm(): void {
    this.editForm.patchValue({
      name: this.editForm.controls.name.value.trim(),
      address: this.editForm.controls.address.value.trim(),
      phone: this.normalizePhoneInput(this.editForm.controls.phone.value),
      gstNumber: this.normalizeGstInput(this.editForm.controls.gstNumber.value),
    });
    this.editForm.updateValueAndValidity();
  }

  private normalizePhoneInput(value: string): string {
    const raw = value.trim();
    if (!raw) {
      return '';
    }

    let digits = raw.replace(/\D/g, '');
    if (digits.length === 12 && digits.startsWith('91')) {
      digits = digits.slice(2);
    }

    return digits.length === 10 ? digits : raw;
  }

  private normalizeGstInput(value: string): string {
    const raw = value.trim();
    return raw ? raw.toUpperCase().replace(/\s/g, '') : '';
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
