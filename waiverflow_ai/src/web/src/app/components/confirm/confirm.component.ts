import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm',
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="modal-overlay" (click)="no.emit()">
    <div class="modal-content confirm-dialog" (click)="$event.stopPropagation()">
      <h2>{{ title }}</h2>
      <p>{{ message }}</p>
      <div class="modal-actions">
        <button class="btn btn-outline" (click)="no.emit()">Cancel</button>
        <button class="btn btn-danger" (click)="yes.emit()">{{ confirmLabel }}</button>
      </div>
    </div>
  </div>
  `
})
export class ConfirmComponent {
  @Input() title = 'Confirm';
  @Input() message = 'Are you sure?';
  @Input() confirmLabel = 'Confirm';
  @Output() yes = new EventEmitter<void>();
  @Output() no = new EventEmitter<void>();
}
