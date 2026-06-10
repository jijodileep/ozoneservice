import { Component, inject, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  template: `
    <div class="modal-header border-0 pb-0">
      <h5 class="modal-title fw-semibold">{{ title }}</h5>
      <button type="button" class="btn-close" aria-label="Close" (click)="activeModal.dismiss()"></button>
    </div>
    <div class="modal-body pt-2">
      <p class="mb-0 text-muted">{{ message }}</p>
    </div>
    <div class="modal-footer border-0 pt-0">
      <button type="button" class="btn btn-light" (click)="activeModal.dismiss()">Cancel</button>
      <button type="button" class="btn" [class]="confirmClass" (click)="activeModal.close(true)">
        {{ confirmLabel }}
      </button>
    </div>
  `,
})
export class ConfirmDialogComponent {
  readonly activeModal = inject(NgbActiveModal);

  @Input() title = 'Confirm';
  @Input() message = 'Are you sure?';
  @Input() confirmLabel = 'Confirm';
  @Input() confirmClass = 'btn-danger';
}
