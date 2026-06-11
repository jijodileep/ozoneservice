import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../core/auth/auth.service';

@Component({
  selector: 'app-change-password-dialog',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <div class="modal-header border-bottom">
      <h5 class="modal-title fw-semibold">Change password</h5>
      <button type="button" class="btn-close" aria-label="Close" (click)="modal.dismiss()"></button>
    </div>
    <form [formGroup]="form" (ngSubmit)="submit()">
      <div class="modal-body">
        @if (error()) {
          <div class="alert alert-danger py-2">{{ error() }}</div>
        }
        <div class="mb-3">
          <label class="form-label" for="currentPassword">Current password</label>
          <input
            id="currentPassword"
            type="password"
            class="form-control"
            formControlName="currentPassword"
            autocomplete="current-password"
          />
        </div>
        <div class="mb-3">
          <label class="form-label" for="newPassword">New password</label>
          <input
            id="newPassword"
            type="password"
            class="form-control"
            formControlName="newPassword"
            autocomplete="new-password"
          />
        </div>
        <div class="mb-0">
          <label class="form-label" for="confirmPassword">Confirm new password</label>
          <input
            id="confirmPassword"
            type="password"
            class="form-control"
            formControlName="confirmPassword"
            autocomplete="new-password"
          />
          @if (form.hasError('mismatch') && form.get('confirmPassword')?.touched) {
            <div class="invalid-feedback d-block">Passwords do not match.</div>
          }
        </div>
      </div>
      <div class="modal-footer border-top">
        <button type="button" class="btn btn-light" (click)="modal.dismiss()">Cancel</button>
        <button type="submit" class="btn btn-primary" [disabled]="saving()">
          @if (saving()) {
            <span class="spinner-border spinner-border-sm me-2"></span>
          }
          Update password
        </button>
      </div>
    </form>
  `,
})
export class ChangePasswordDialogComponent {
  readonly modal = inject(NgbActiveModal);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group(
    {
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
    },
    {
      validators: (group) =>
        group.get('newPassword')?.value === group.get('confirmPassword')?.value
          ? null
          : { mismatch: true },
    },
  );

  submit(): void {
    this.error.set(null);
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { currentPassword, newPassword } = this.form.getRawValue();
    this.saving.set(true);

    this.auth.changePassword(currentPassword, newPassword).subscribe({
      next: () => {
        this.saving.set(false);
        this.modal.close(true);
      },
      error: (err: unknown) => {
        this.saving.set(false);
        if (err instanceof HttpErrorResponse) {
          const body = err.error as { message?: string } | null;
          this.error.set(body?.message ?? 'Could not change password.');
        } else {
          this.error.set('Could not change password.');
        }
      },
    });
  }
}
