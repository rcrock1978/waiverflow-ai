import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="toast-container">
    <div *ngFor="let m of service.messages$ | async" class="toast toast-{{ m.type }}" (click)="service.dismiss(m.id)">
      {{ m.text }}
    </div>
  </div>
  `
})
export class ToastComponent {
  constructor(public service: ToastService) {}
}
