import { Component, OnInit, TemplateRef, inject, signal, viewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { NgbAlertModule, NgbModal, NgbModalRef, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import {
  MobileBrand,
  MobileModel,
  MobileVariant,
} from '../../core/mobile-master/mobile-master.models';
import { MobileMasterService } from '../../core/mobile-master/mobile-master.service';
import { ConfirmDialogComponent } from '../../shared/confirm-dialog.component';

type EditTarget =
  | { type: 'brand'; item: MobileBrand }
  | { type: 'model'; item: MobileModel }
  | { type: 'variant'; item: MobileVariant };

@Component({
  selector: 'app-mobile-masters',
  standalone: true,
  imports: [ReactiveFormsModule, NgbAlertModule, NgbTooltipModule],
  templateUrl: './mobile-masters.component.html',
})
export class MobileMastersComponent implements OnInit {
  private readonly service = inject(MobileMasterService);
  private readonly fb = inject(FormBuilder);
  private readonly modal = inject(NgbModal);

  readonly editModalTpl = viewChild.required<TemplateRef<unknown>>('editModal');

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly brands = signal<MobileBrand[]>([]);
  readonly models = signal<MobileModel[]>([]);
  readonly variants = signal<MobileVariant[]>([]);
  readonly selectedBrand = signal<MobileBrand | null>(null);
  readonly selectedModel = signal<MobileModel | null>(null);
  readonly editing = signal<EditTarget | null>(null);

  readonly brandForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
  });

  readonly modelForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
  });

  readonly variantForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
  });

  readonly editForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    isActive: [true],
  });

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.loading.set(true);
    this.service.getBrands().subscribe({
      next: (brands) => {
        this.brands.set(brands);
        this.loading.set(false);
        const current = this.selectedBrand();
        if (current) {
          const refreshed = brands.find((b) => b.id === current.id) ?? null;
          this.selectedBrand.set(refreshed);
          if (refreshed) {
            this.loadModels(refreshed.id);
          } else {
            this.models.set([]);
            this.variants.set([]);
            this.selectedModel.set(null);
          }
        }
      },
      error: () => {
        this.error.set('Could not load brands.');
        this.loading.set(false);
      },
    });
  }

  selectBrand(brand: MobileBrand): void {
    this.selectedBrand.set(brand);
    this.selectedModel.set(null);
    this.variants.set([]);
    this.loadModels(brand.id);
  }

  loadModels(brandId: string): void {
    this.service.getModels(brandId).subscribe({
      next: (models) => {
        this.models.set(models);
        const current = this.selectedModel();
        if (current) {
          const refreshed = models.find((m) => m.id === current.id) ?? null;
          this.selectedModel.set(refreshed);
          if (refreshed) {
            this.loadVariants(refreshed.id);
          } else {
            this.variants.set([]);
          }
        }
      },
      error: () => this.error.set('Could not load models.'),
    });
  }

  selectModel(model: MobileModel): void {
    this.selectedModel.set(model);
    this.loadVariants(model.id);
  }

  loadVariants(modelId: string): void {
    this.service.getVariants(modelId).subscribe({
      next: (variants) => this.variants.set(variants),
      error: () => this.error.set('Could not load variants.'),
    });
  }

  createBrand(): void {
    this.error.set(null);
    this.brandForm.patchValue({ name: this.brandForm.controls.name.value.trim() });
    if (this.brandForm.invalid) {
      this.brandForm.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.service.createBrand({ name: this.brandForm.controls.name.value }).subscribe({
      next: () => {
        this.saving.set(false);
        this.brandForm.reset({ name: '' });
        this.loadBrands();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not create brand.'));
      },
    });
  }

  createModel(): void {
    const brand = this.selectedBrand();
    this.error.set(null);
    this.modelForm.patchValue({ name: this.modelForm.controls.name.value.trim() });
    if (!brand || this.modelForm.invalid) {
      this.modelForm.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.service.createModel(brand.id, { name: this.modelForm.controls.name.value }).subscribe({
      next: () => {
        this.saving.set(false);
        this.modelForm.reset({ name: '' });
        this.loadModels(brand.id);
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not create model.'));
      },
    });
  }

  createVariant(): void {
    const model = this.selectedModel();
    this.error.set(null);
    this.variantForm.patchValue({ name: this.variantForm.controls.name.value.trim() });
    if (!model || this.variantForm.invalid) {
      this.variantForm.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.service.createVariant(model.id, { name: this.variantForm.controls.name.value }).subscribe({
      next: () => {
        this.saving.set(false);
        this.variantForm.reset({ name: '' });
        this.loadVariants(model.id);
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not create variant.'));
      },
    });
  }

  openEdit(target: EditTarget): void {
    this.editing.set(target);
    this.editForm.patchValue({ name: target.item.name, isActive: target.item.isActive });
    this.modal.open(this.editModalTpl(), { centered: true, backdrop: 'static' });
  }

  saveEdit(modal: NgbModalRef): void {
    const target = this.editing();
    this.error.set(null);
    this.editForm.patchValue({ name: this.editForm.controls.name.value.trim() });
    if (!target || this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const raw = this.editForm.getRawValue();
    this.saving.set(true);

    const done = () => {
      this.saving.set(false);
      this.editing.set(null);
      modal.close();
    };

    if (target.type === 'brand') {
      this.service.updateBrand(target.item.id, raw).subscribe({
        next: () => {
          done();
          this.loadBrands();
        },
        error: (err) => {
          this.saving.set(false);
          this.error.set(this.formatApiError(err, 'Could not update brand.'));
        },
      });
      return;
    }

    if (target.type === 'model') {
      this.service.updateModel(target.item.id, raw).subscribe({
        next: () => {
          done();
          const brand = this.selectedBrand();
          if (brand) {
            this.loadModels(brand.id);
          }
        },
        error: (err) => {
          this.saving.set(false);
          this.error.set(this.formatApiError(err, 'Could not update model.'));
        },
      });
      return;
    }

    this.service.updateVariant(target.item.id, raw).subscribe({
      next: () => {
        done();
        const model = this.selectedModel();
        if (model) {
          this.loadVariants(model.id);
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(this.formatApiError(err, 'Could not update variant.'));
      },
    });
  }

  deactivate(target: EditTarget): void {
    const labels = { brand: 'brand', model: 'model', variant: 'variant' };
    const ref = this.modal.open(ConfirmDialogComponent, { centered: true });
    ref.componentInstance.title = `Deactivate ${labels[target.type]}`;
    ref.componentInstance.message = `Deactivate "${target.item.name}"?`;
    ref.componentInstance.confirmLabel = 'Deactivate';

    ref.result.then((confirmed) => {
      if (!confirmed) {
        return;
      }

      const request =
        target.type === 'brand'
          ? this.service.deactivateBrand(target.item.id)
          : target.type === 'model'
            ? this.service.deactivateModel(target.item.id)
            : this.service.deactivateVariant(target.item.id);

      request.subscribe({
        next: () => {
          if (target.type === 'brand') {
            this.loadBrands();
          } else if (target.type === 'model') {
            const brand = this.selectedBrand();
            if (brand) {
              this.loadModels(brand.id);
            }
          } else {
            const model = this.selectedModel();
            if (model) {
              this.loadVariants(model.id);
            }
          }
        },
        error: (err) => this.error.set(this.formatApiError(err, 'Could not deactivate.')),
      });
    }).catch(() => undefined);
  }

  editTitle(): string {
    const target = this.editing();
    if (!target) {
      return 'Edit';
    }
    return `Edit ${target.type}`;
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
    return 'Invalid value.';
  }

  private formatApiError(err: unknown, fallback: string): string {
    if (!(err instanceof HttpErrorResponse)) {
      return fallback;
    }
    const body = err.error as { message?: string } | null;
    return body?.message ?? fallback;
  }
}
