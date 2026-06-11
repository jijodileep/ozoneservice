import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { PlatformService } from '../../core/platform/platform.service';

@Component({
  selector: 'app-platform-tax',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe, DatePipe, NgbAlertModule],
  templateUrl: './platform-tax.component.html',
})
export class PlatformTaxComponent implements OnInit {
  private readonly platform = inject(PlatformService);
  private readonly fb = inject(FormBuilder);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);
  readonly updatedAt = signal<string | null>(null);
  readonly totalGst = signal(0);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    cgstRate: [9, [Validators.required, Validators.min(0), Validators.max(100)]],
    sgstRate: [9, [Validators.required, Validators.min(0), Validators.max(100)]],
  });

  ngOnInit(): void {
    this.platform.getTaxConfiguration().subscribe({
      next: (config) => {
        this.form.patchValue({
          name: config.name,
          cgstRate: config.cgstRate,
          sgstRate: config.sgstRate,
        });
        this.updatedAt.set(config.updatedAt);
        this.totalGst.set(config.totalGstRate);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load tax configuration.');
        this.loading.set(false);
      },
    });

    this.form.valueChanges.subscribe(() => {
      const { cgstRate = 0, sgstRate = 0 } = this.form.getRawValue();
      this.totalGst.set(cgstRate + sgstRate);
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }
    this.saving.set(true);
    this.message.set(null);
    this.error.set(null);
    this.platform.updateTaxConfiguration(this.form.getRawValue()).subscribe({
      next: (config) => {
        this.saving.set(false);
        this.updatedAt.set(config.updatedAt);
        this.totalGst.set(config.totalGstRate);
        this.message.set('Tax master updated. New subscription invoices will use CGST + SGST split.');
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Could not save tax configuration.');
      },
    });
  }
}
