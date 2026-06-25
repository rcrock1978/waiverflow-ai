import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../toast/toast.service';

@Component({
  selector: 'app-coi',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-container">
      <div class="card">
        <div class="card-body">
          <h2>Upload Certificate of Insurance</h2>
          <div class="form-group">
            <input type="file" class="form-input" (change)="onFileSelected($event)" accept=".pdf,.jpg,.png" #fileRef />
            <div class="form-error" *ngIf="submitted && !selectedFile">File is required</div>
          </div>
          <button class="btn btn-primary" (click)="upload()" [disabled]="!selectedFile || uploading">
            {{ uploading ? 'Uploading...' : 'Upload COI' }}
          </button>

          <ng-container *ngIf="uploading">
            <div class="empty-state"><p>Loading...</p></div>
          </ng-container>
        </div>
      </div>
    </div>
  `
})
export class CoiComponent {
  private http = inject(HttpClient);
  private toast = inject(ToastService);

  selectedFile: File | null = null;
  uploading = false;
  submitted = false;

  onFileSelected(event: Event): void {
    const el = event.target as HTMLInputElement;
    if (el.files?.length) this.selectedFile = el.files[0];
  }

  upload(): void {
    this.submitted = true;
    if (!this.selectedFile) return;
    this.uploading = true;
    const fd = new FormData();
    fd.append('file', this.selectedFile);
    this.http.post('/api/v1/sub/coi', fd).subscribe({
      next: () => {
        this.uploading = false;
        this.selectedFile = null;
        this.submitted = false;
        this.toast.show('COI uploaded successfully', 'success');
      },
      error: (err) => {
        this.uploading = false;
        this.toast.show(err.message || 'Upload failed', 'error');
      }
    });
  }
}
