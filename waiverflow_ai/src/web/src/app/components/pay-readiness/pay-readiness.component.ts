import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { PayReadiness } from '../../models/models';
import { ToastService } from '../toast/toast.service';
import { ConfirmComponent } from '../confirm/confirm.component';

@Component({
  selector: 'app-pay-readiness',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ConfirmComponent],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Pay Readiness</h1>
        <button class="btn btn-outline" (click)="router.navigate(['/projects'])">Back to Projects</button>
      </div>

      <ng-container *ngIf="data !== null; else loading">
        <div class="card" *ngIf="data">
          <div class="card-body">
            <div class="status-badge" [style.background]="statusColor(data.status)" [style.color]="'#fff'" [style.fontSize]="'1.2rem'">
              {{ data.status }}
            </div>

            <div class="stat-grid">
              <div class="stat-card">
                <span class="stat-value">{{ data.completedWaiverRequests }}</span>
                <span class="stat-label">Completed</span>
              </div>
              <div class="stat-card">
                <span class="stat-value">{{ data.outstandingWaiverRequests }}</span>
                <span class="stat-label">Outstanding</span>
              </div>
              <div class="stat-card">
                <span class="stat-value">{{ data.totalWaiverRequests }}</span>
                <span class="stat-label">Total</span>
              </div>
            </div>

            <div class="progress-bar">
              <div class="progress-fill" [style.width.%]="percentComplete"></div>
            </div>
            <p>{{ percentComplete }}% completed</p>

            <p>
              COI Compliance:
              <span class="status-badge" [style.background]="coiColor(data.coiComplianceStatus)" [style.color]="'#fff'">
                {{ data.coiComplianceStatus }}
              </span>
            </p>

            <button class="btn btn-primary" (click)="exportAuditPackage()">Export Audit Package</button>
          </div>
        </div>

        <div class="card">
          <div class="card-body">
            <h3>Override</h3>
            <div class="form-group">
              <textarea class="form-input" [(ngModel)]="overrideReason" placeholder="Reason for override"></textarea>
            </div>
            <button class="btn btn-primary" (click)="confirmOverride()" [disabled]="!overrideReason.trim()">Submit Override</button>
          </div>
        </div>
      </ng-container>

      <ng-template #loading>
        <div class="empty-state"><p>Loading...</p></div>
      </ng-template>

      <app-confirm
        *ngIf="showConfirm"
        title="Confirm Override"
        message="Are you sure you want to override the pay readiness status? This action will mark outstanding items as complete."
        confirmLabel="Override"
        (yes)="onOverrideConfirmed()"
        (no)="onOverrideCancelled()"
      ></app-confirm>
    </div>
  `
})
export class PayReadinessComponent implements OnInit {
  data: PayReadiness | null = null;
  projectId = '';
  overrideReason = '';
  showConfirm = false;

  constructor(
    private api: ApiService,
    public router: Router,
    private route: ActivatedRoute,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    if (this.projectId) {
      this.api.getPayReadiness(this.projectId).subscribe(d => this.data = d);
    }
  }

  get percentComplete(): number {
    if (!this.data || this.data.totalWaiverRequests === 0) return 0;
    return Math.round((this.data.completedWaiverRequests / this.data.totalWaiverRequests) * 100);
  }

  statusColor(status: string): string {
    switch (status) {
      case 'ready': return '#22c55e';
      case 'blocked': return '#ef4444';
      case 'in_progress': return '#f97316';
      default: return '#6b7280';
    }
  }

  coiColor(status: string): string {
    return status === 'compliant' ? '#22c55e' : '#ef4444';
  }

  exportAuditPackage(): void {
    window.open(`/api/v1/projects/${this.projectId}/pay-readiness/export`, '_blank');
  }

  confirmOverride(): void {
    this.showConfirm = true;
  }

  onOverrideConfirmed(): void {
    this.showConfirm = false;
    if (!this.overrideReason.trim()) return;
    const projectId = this.projectId;
    this.api.getPayReadiness(projectId).subscribe(pr => {
      if (pr.outstandingWaiverRequests > 0) {
        this.api.overrideWaiver('all', { reason: this.overrideReason }).subscribe(() => {
          this.overrideReason = '';
          this.api.getPayReadiness(projectId).subscribe(d => {
            this.data = d;
            this.toast.show('Override successful', 'success');
          });
        });
      }
    });
  }

  onOverrideCancelled(): void {
    this.showConfirm = false;
  }
}
