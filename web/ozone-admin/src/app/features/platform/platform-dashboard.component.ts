import { Component, OnInit, inject, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { PlatformService } from '../../core/platform/platform.service';
import { ShopSummary } from '../../core/platform/platform.models';

@Component({
  selector: 'app-platform-dashboard',
  standalone: true,
  imports: [MatCardModule, MatTableModule, MatChipsModule, MatProgressSpinnerModule],
  templateUrl: './platform-dashboard.component.html',
  styleUrl: './platform-dashboard.component.scss',
})
export class PlatformDashboardComponent implements OnInit {
  private readonly platform = inject(PlatformService);

  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly shops = signal<ShopSummary[]>([]);

  readonly displayedColumns = [
    'name',
    'code',
    'planName',
    'branchCount',
    'userCount',
    'isActive',
  ];

  ngOnInit(): void {
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
}
