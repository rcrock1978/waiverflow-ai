import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../interceptors/loading.service';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="spinner-overlay" *ngIf="loading.isLoading$ | async">
    <div class="spinner"></div>
    <p>Loading...</p>
  </div>
  `
})
export class SpinnerComponent {
  constructor(public loading: LoadingService) {}
}
