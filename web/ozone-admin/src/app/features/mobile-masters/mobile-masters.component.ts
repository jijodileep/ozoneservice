import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { MobileBrand, MobileModel, MobileVariant } from '../../core/mobile-master/mobile-master.models';
import { MobileMasterService } from '../../core/mobile-master/mobile-master.service';
import { DEFAULT_PAGE_SIZE } from '../../shared/pagination.util';
import { TablePaginationComponent } from '../../shared/table-pagination.component';

@Component({
  selector: 'app-mobile-masters',
  standalone: true,
  imports: [FormsModule, NgbAlertModule, TablePaginationComponent],
  templateUrl: './mobile-masters.component.html',
})
export class MobileMastersComponent implements OnInit {
  private readonly service = inject(MobileMasterService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly brands = signal<MobileBrand[]>([]);
  readonly models = signal<MobileModel[]>([]);
  readonly variants = signal<MobileVariant[]>([]);
  readonly selectedBrand = signal<MobileBrand | null>(null);
  readonly selectedModel = signal<MobileModel | null>(null);
  readonly loadingModels = signal(false);
  readonly loadingVariants = signal(false);
  readonly page = signal(1);
  readonly pageSize = signal(DEFAULT_PAGE_SIZE);
  readonly totalCount = signal(0);
  readonly search = signal('');

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.loading.set(true);
    this.service
      .getBrandsPaged({
        page: this.page(),
        pageSize: this.pageSize(),
        search: this.search(),
      })
      .subscribe({
        next: (result) => {
          this.brands.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
          const current = this.selectedBrand();
          if (current && !result.items.some((b) => b.id === current.id)) {
            this.selectedBrand.set(null);
            this.selectedModel.set(null);
            this.models.set([]);
            this.variants.set([]);
          }
        },
        error: () => {
          this.error.set('Could not load mobile brands.');
          this.loading.set(false);
        },
      });
  }

  onSearchChange(value: string): void {
    this.search.set(value);
    this.page.set(1);
    this.loadBrands();
  }

  onPageChange(page: number): void {
    this.page.set(page);
    this.loadBrands();
  }

  selectBrand(brand: MobileBrand): void {
    this.selectedBrand.set(brand);
    this.selectedModel.set(null);
    this.variants.set([]);
    this.loadingModels.set(true);
    this.service.getModels(brand.id).subscribe({
      next: (models) => {
        this.models.set(models);
        this.loadingModels.set(false);
      },
      error: () => {
        this.error.set('Could not load models.');
        this.loadingModels.set(false);
      },
    });
  }

  selectModel(model: MobileModel): void {
    this.selectedModel.set(model);
    this.loadingVariants.set(true);
    this.service.getVariants(model.id).subscribe({
      next: (variants) => {
        this.variants.set(variants);
        this.loadingVariants.set(false);
      },
      error: () => {
        this.error.set('Could not load variants.');
        this.loadingVariants.set(false);
      },
    });
  }
}
