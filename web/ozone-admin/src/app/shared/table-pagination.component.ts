import { Component, computed, input, output } from '@angular/core';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-table-pagination',
  standalone: true,
  imports: [NgbPaginationModule],
  template: `
    @if (totalCount() > 0) {
      <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 p-3 border-top">
        <span class="text-muted small">{{ rangeLabel() }}</span>
        <ngb-pagination
          [page]="page()"
          [pageSize]="pageSize()"
          [collectionSize]="totalCount()"
          [maxSize]="5"
          [rotate]="true"
          [boundaryLinks]="true"
          (pageChange)="pageChange.emit($event)"
        />
      </div>
    }
  `,
})
export class TablePaginationComponent {
  readonly page = input(1);
  readonly pageSize = input(10);
  readonly totalCount = input(0);
  readonly pageChange = output<number>();

  readonly rangeLabel = computed(() => {
    const total = this.totalCount();
    if (total === 0) {
      return 'No records';
    }
    const start = (this.page() - 1) * this.pageSize() + 1;
    const end = Math.min(this.page() * this.pageSize(), total);
    return `Showing ${start}–${end} of ${total}`;
  });
}
