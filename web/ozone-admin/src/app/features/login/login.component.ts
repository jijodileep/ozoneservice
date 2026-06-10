import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of, switchMap } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { BranchService } from '../../core/branch/branch.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgbAlertModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly branch = inject(BranchService);
  private readonly router = inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['admin@localhost.dev', [Validators.required, Validators.email]],
    password: ['Admin@123', [Validators.required, Validators.minLength(8)]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    const { email, password } = this.form.getRawValue();

    this.auth
      .login(email, password)
      .pipe(
        switchMap(() => this.auth.loadProfile()),
        switchMap((): Observable<unknown> =>
          this.auth.isPlatformAdmin() ? of(null) : this.branch.loadBranches()),
      )
      .subscribe({
        next: () => {
          this.loading.set(false);
          void this.router.navigate([this.auth.isPlatformAdmin() ? '/platform' : '/dashboard']);
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Invalid email or password.');
        },
      });
  }
}
