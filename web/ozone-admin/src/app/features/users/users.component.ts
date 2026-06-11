import { Component, OnInit, TemplateRef, computed, inject, signal, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { NgbAlertModule, NgbModal, NgbModalRef, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../core/auth/auth.service';
import { BranchDetail } from '../../core/branch/branch.models';
import { BranchService } from '../../core/branch/branch.service';
import {
  ASSIGNABLE_ROLES,
  CreateUserRequest,
  ResetUserPasswordRequest,
  UpdateUserRequest,
  UserDetail,
} from '../../core/user/user.models';
import { UserService } from '../../core/user/user.service';
import {
  PASSWORD_REQUIREMENTS_HINT,
  passwordStrengthValidator,
} from '../../core/validators/password.validators';
import { DEFAULT_PAGE_SIZE, clampPage, paginateSlice } from '../../shared/pagination.util';
import { TablePaginationComponent } from '../../shared/table-pagination.component';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [ReactiveFormsModule, NgbAlertModule, NgbTooltipModule, TablePaginationComponent],
  templateUrl: './users.component.html',
})
export class UsersComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly branchService = inject(BranchService);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly modal = inject(NgbModal);

  readonly editModalTpl = viewChild.required<TemplateRef<unknown>>('editModal');
  readonly resetModalTpl = viewChild.required<TemplateRef<unknown>>('resetModal');

  readonly assignableRoles = ASSIGNABLE_ROLES;
  readonly passwordRequirementsHint = PASSWORD_REQUIREMENTS_HINT;
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly users = signal<UserDetail[]>([]);
  readonly page = signal(1);
  readonly pageSize = signal(DEFAULT_PAGE_SIZE);
  readonly totalCount = computed(() => this.users().length);
  readonly pagedUsers = computed(() =>
    paginateSlice(this.users(), this.page(), this.pageSize()));
  readonly branches = signal<BranchDetail[]>([]);
  readonly editingUser = signal<UserDetail | null>(null);
  readonly resettingUser = signal<UserDetail | null>(null);

  readonly createForm = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
    displayName: ['', [Validators.required, Validators.maxLength(200)]],
    password: ['', [Validators.required, passwordStrengthValidator()]],
    role: ['ShopStaff', Validators.required],
    branchIds: this.fb.nonNullable.control<string[]>([], [Validators.required, Validators.minLength(1)]),
  });

  readonly editForm = this.fb.nonNullable.group({
    displayName: ['', [Validators.required, Validators.maxLength(200)]],
    role: ['ShopStaff', Validators.required],
    branchIds: this.fb.nonNullable.control<string[]>([], [Validators.required, Validators.minLength(1)]),
    isActive: [true],
  });

  readonly resetForm = this.fb.nonNullable.group({
    newPassword: ['', [Validators.required, passwordStrengthValidator()]],
  });

  ngOnInit(): void {
    this.loadUsers();
    this.branchService.getBranches().subscribe({
      next: (branches) => this.branches.set(branches.filter((b) => b.isActive)),
    });
  }

  isTenantAdmin(): boolean {
    return this.auth.hasAnyRole(['TenantAdmin']);
  }

  isEditable(user: UserDetail): boolean {
    return user.role !== 'TenantAdmin';
  }

  roleLabel(role: string): string {
    return this.assignableRoles.find((r) => r.value === role)?.label ?? role;
  }

  onPageChange(page: number): void {
    this.page.set(page);
  }

  private syncPage(): void {
    this.page.set(clampPage(this.page(), this.totalCount(), this.pageSize()));
  }

  loadUsers(): void {
    this.loading.set(true);
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.syncPage();
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load users.');
        this.loading.set(false);
      },
    });
  }

  toggleBranch(form: FormGroup, branchId: string, checked: boolean): void {
    const control = form.get('branchIds');
    if (!control) {
      return;
    }

    const current = [...(control.value as string[])];
    const next = checked
      ? current.includes(branchId)
        ? current
        : [...current, branchId]
      : current.filter((id) => id !== branchId);

    control.setValue(next);
    control.markAsTouched();
    control.updateValueAndValidity();
  }

  isBranchSelected(form: FormGroup, branchId: string): boolean {
    const value = form.get('branchIds')?.value as string[] | undefined;
    return value?.includes(branchId) ?? false;
  }

  createUser(): void {
    this.error.set(null);
    this.normalizeCreateForm();

    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      this.error.set('Please fix the highlighted fields.');
      return;
    }

    const raw = this.createForm.getRawValue();
    const request: CreateUserRequest = {
      email: raw.email,
      displayName: raw.displayName,
      password: raw.password,
      role: raw.role,
      branchIds: raw.branchIds,
    };

    this.saving.set(true);
    this.userService.createUser(request).subscribe({
      next: () => {
        this.saving.set(false);
        this.createForm.reset({
          email: '',
          displayName: '',
          password: '',
          role: 'ShopStaff',
          branchIds: [],
        });
        this.loadUsers();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not create user.'));
      },
    });
  }

  openEdit(user: UserDetail): void {
    if (!this.isEditable(user)) {
      return;
    }

    this.editingUser.set(user);
    this.editForm.patchValue({
      displayName: user.displayName,
      role: user.role,
      branchIds: user.branches.map((b) => b.id),
      isActive: user.isActive,
    });
    this.modal.open(this.editModalTpl(), { centered: true, backdrop: 'static' });
  }

  saveEdit(modal: NgbModalRef): void {
    const user = this.editingUser();
    this.error.set(null);
    this.normalizeEditForm();

    if (!user || this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      if (this.editForm.invalid) {
        this.error.set('Please fix the highlighted fields.');
      }
      return;
    }

    const raw = this.editForm.getRawValue();
    const request: UpdateUserRequest = {
      displayName: raw.displayName,
      role: raw.role,
      branchIds: raw.branchIds,
      isActive: raw.isActive,
    };

    this.saving.set(true);
    this.userService.updateUser(user.id, request).subscribe({
      next: () => {
        this.saving.set(false);
        this.editingUser.set(null);
        modal.close();
        this.loadUsers();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not update user.'));
      },
    });
  }

  openResetPassword(user: UserDetail): void {
    if (!this.isEditable(user)) {
      return;
    }

    this.resettingUser.set(user);
    this.resetForm.reset({ newPassword: '' });
    this.modal.open(this.resetModalTpl(), { centered: true, backdrop: 'static' });
  }

  saveResetPassword(modal: NgbModalRef): void {
    const user = this.resettingUser();
    this.error.set(null);

    if (!user || this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      if (this.resetForm.invalid) {
        this.error.set('Please fix the highlighted fields.');
      }
      return;
    }

    const request: ResetUserPasswordRequest = {
      newPassword: this.resetForm.controls.newPassword.value,
    };

    this.saving.set(true);
    this.userService.resetPassword(user.id, request).subscribe({
      next: () => {
        this.saving.set(false);
        this.resettingUser.set(null);
        modal.close();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not reset password.'));
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

    if (control.errors['required'] || control.errors['minlength']) {
      return controlName === 'branchIds' ? 'Select at least one branch.' : 'This field is required.';
    }

    if (control.errors['email']) {
      return 'Enter a valid email address.';
    }

    if (control.errors['passwordStrength']) {
      return PASSWORD_REQUIREMENTS_HINT;
    }

    if (control.errors['minlength']) {
      return `Minimum ${control.errors['minlength'].requiredLength} characters.`;
    }

    if (control.errors['maxlength']) {
      return `Maximum ${control.errors['maxlength'].requiredLength} characters.`;
    }

    return 'Invalid value.';
  }

  private normalizeCreateForm(): void {
    this.createForm.patchValue({
      email: this.createForm.controls.email.value.trim().toLowerCase(),
      displayName: this.createForm.controls.displayName.value.trim(),
      password: this.createForm.controls.password.value,
    });
    this.createForm.updateValueAndValidity();
  }

  private normalizeEditForm(): void {
    this.editForm.patchValue({
      displayName: this.editForm.controls.displayName.value.trim(),
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
}
