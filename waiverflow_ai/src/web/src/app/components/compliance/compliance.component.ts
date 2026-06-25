import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { ComplianceStatus } from '../../models/models';
import { PaginatePipe } from '../../pipes/paginate.pipe';

@Component({
  selector: 'app-compliance',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginatePipe],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Compliance Status</h1>
        <button class="btn btn-outline" (click)="router.navigate(['/projects'])">Back to Projects</button>
      </div>

      <ng-container *ngIf="compliance !== null; else loading">
        <div class="card" *ngIf="compliance.length > 0; else empty">
          <div class="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Company</th>
                  <th>Status</th>
                  <th>Expiry Date</th>
                  <th>Days Until Expiry</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let c of compliance | paginate:page:pageSize">
                  <td>{{ c.companyName }}</td>
                  <td>
                    <span class="status-badge"
                      [style.background]="statusColor(c.status)"
                      [style.color]="'#fff'">
                      {{ c.status }}
                    </span>
                  </td>
                  <td>{{ c.expiryDate || 'N/A' }}</td>
                  <td>{{ c.daysUntilExpiry }}</td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="pagination" *ngIf="compliance.length > pageSize">
            <button class="btn btn-outline" (click)="prevPage()" [disabled]="page === 1">Prev</button>
            <span>Page {{ page }} of {{ totalPages }}</span>
            <button class="btn btn-outline" (click)="nextPage()" [disabled]="page === totalPages">Next</button>
          </div>
        </div>

        <ng-template #empty>
          <div class="empty-state">
            <p>No compliance data available.</p>
          </div>
        </ng-template>
      </ng-container>

      <ng-template #loading>
        <div class="empty-state"><p>Loading...</p></div>
      </ng-template>
    </div>
  `
})
export class ComplianceComponent implements OnInit {
  compliance: ComplianceStatus[] | null = null;
  page = 1;
  pageSize = 10;

  constructor(
    private api: ApiService,
    public router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    if (projectId) {
      this.api.getCompliance(projectId).subscribe(c => this.compliance = c);
    }
  }

  get totalPages(): number {
    return this.compliance ? Math.ceil(this.compliance.length / this.pageSize) : 1;
  }

  prevPage(): void {
    if (this.page > 1) this.page--;
  }

  nextPage(): void {
    if (this.page < this.totalPages) this.page++;
  }

  statusColor(status: string): string {
    switch (status) {
      case 'valid': return '#22c55e';
      case 'expiring_soon': return '#f97316';
      case 'expired': return '#ef4444';
      case 'missing': return '#9ca3af';
      default: return '#6b7280';
    }
  }
}
