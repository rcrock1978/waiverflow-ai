import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../toast/toast.service';

@Component({
  selector: 'app-submit',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Submit Waiver</h1>
        <button class="btn btn-outline" (click)="router.navigate(['/projects'])">Back to Projects</button>
      </div>

      <div class="card">
        <div class="card-body">
          <div class="form-group">
            <label>Upload signed waiver document</label>
            <input type="file" class="form-input" (change)="onFileSelected($event)" #fileRef />
            <div class="form-error" *ngIf="submitted && !selectedFile">File is required</div>
          </div>
          <button class="btn btn-primary" (click)="submit()" [disabled]="!selectedFile || submitting">
            {{ submitting ? 'Submitting...' : 'Submit' }}
          </button>

          <ng-container *ngIf="submitting">
            <div class="empty-state"><p>Loading...</p></div>
          </ng-container>
        </div>
      </div>
    </div>
  `
})
export class SubmitComponent {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  public router = inject(Router);
  private toast = inject(ToastService);

  selectedFile: File | null = null;
  submitting = false;
  submitted = false;

  onFileSelected(event: Event): void {
    const el = event.target as HTMLInputElement;
    if (el.files?.length) this.selectedFile = el.files[0];
  }

  submit(): void {
    this.submitted = true;
    if (!this.selectedFile) return;
    this.submitting = true;
    const waiverId = this.route.snapshot.paramMap.get('waiverId') ?? '';
    const fd = new FormData();
    fd.append('file', this.selectedFile);
    this.http.post(`/api/v1/sub/waivers/${waiverId}/submit`, fd).subscribe({
      next: () => {
        this.submitting = false;
        this.selectedFile = null;
        this.submitted = false;
        this.toast.show('Waiver submitted successfully', 'success');
      },
      error: (err) => {
        this.submitting = false;
        this.toast.show(err.message || 'Submission failed', 'error');
      }
    });
  }
}
