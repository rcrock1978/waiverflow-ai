import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Subcontractor } from '../../models/models';
import { ToastService } from '../toast/toast.service';
import { PaginatePipe } from '../../pipes/paginate.pipe';

@Component({
  selector: 'app-subs',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, PaginatePipe],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Subcontractors</h1>
        <button class="btn btn-outline" (click)="router.navigate(['/projects'])">Back to Projects</button>
      </div>

      <div class="card">
        <div class="card-body">
          <h3>Add Subcontractor</h3>
          <div class="form-row">
            <div class="form-group">
              <input class="form-input" [(ngModel)]="newSub.companyName" #companyRef="ngModel" placeholder="Company name" required />
              <div class="form-error" *ngIf="companyRef.invalid && companyRef.touched">Company name is required</div>
            </div>
            <div class="form-group">
              <input class="form-input" [(ngModel)]="newSub.contactName" placeholder="Contact name" />
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <input class="form-input" [(ngModel)]="newSub.contactEmail" #emailRef="ngModel" placeholder="Email" required pattern="[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}" />
              <div class="form-error" *ngIf="emailRef.invalid && emailRef.touched">
                <ng-container *ngIf="emailRef.errors?.['required']">Email is required</ng-container>
                <ng-container *ngIf="emailRef.errors?.['pattern']">Enter a valid email address</ng-container>
              </div>
            </div>
            <div class="form-group">
              <input class="form-input" [(ngModel)]="newSub.workState" #stateRef="ngModel" placeholder="Work state" required />
              <div class="form-error" *ngIf="stateRef.invalid && stateRef.touched">Work state is required</div>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <input class="form-input" [(ngModel)]="newSub.phone" placeholder="Phone" />
            </div>
          </div>
          <button class="btn btn-primary" (click)="addSub()" [disabled]="!newSub.companyName || !newSub.contactEmail || !newSub.workState">Add</button>
        </div>
      </div>

      <div class="card">
        <div class="card-body">
          <h3>Import from CSV</h3>
          <input type="file" class="form-input" (change)="onFileSelected($event)" accept=".csv" />
          <button class="btn btn-primary" (click)="importCsv()" [disabled]="!selectedFile">Upload</button>
        </div>
      </div>

      <ng-container *ngIf="subs !== null; else loading">
        <div class="card" *ngIf="subs.length > 0">
          <div class="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Company</th>
                  <th>Contact</th>
                  <th>Email</th>
                  <th>State</th>
                  <th>COI Status</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let s of subs | paginate:page:pageSize">
                  <td>{{ s.companyName }}</td>
                  <td>{{ s.contactName }}</td>
                  <td>{{ s.contactEmail }}</td>
                  <td>{{ s.workState }}</td>
                  <td>
                    <span class="status-badge" [class.badge-green]="s.coiExpiryDate" [class.badge-gray]="!s.coiExpiryDate">
                      {{ s.coiExpiryDate ? 'Valid' : 'Missing' }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="pagination" *ngIf="subs.length > pageSize">
            <button class="btn btn-outline" (click)="prevPage()" [disabled]="page === 1">Prev</button>
            <span>Page {{ page }} of {{ totalPages }}</span>
            <button class="btn btn-outline" (click)="nextPage()" [disabled]="page === totalPages">Next</button>
          </div>
        </div>
      </ng-container>

      <ng-template #loading>
        <div class="empty-state"><p>Loading...</p></div>
      </ng-template>
    </div>
  `
})
export class SubsComponent implements OnInit {
  projectId = '';
  subs: Subcontractor[] | null = null;
  selectedFile: File | null = null;
  newSub = { companyName: '', contactName: '', contactEmail: '', workState: '', phone: '' };
  page = 1;
  pageSize = 10;

  constructor(
    private api: ApiService,
    public router: Router,
    private route: ActivatedRoute,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    if (this.projectId) {
      this.api.getSubs(this.projectId).subscribe(s => this.subs = s);
    }
  }

  get totalPages(): number {
    return this.subs ? Math.ceil(this.subs.length / this.pageSize) : 1;
  }

  prevPage(): void {
    if (this.page > 1) this.page--;
  }

  nextPage(): void {
    if (this.page < this.totalPages) this.page++;
  }

  addSub(): void {
    if (!this.projectId) return;
    this.api.addSub(this.projectId, { ...this.newSub }).subscribe(s => {
      this.subs = [...(this.subs ?? []), s];
      this.newSub = { companyName: '', contactName: '', contactEmail: '', workState: '', phone: '' };
      this.toast.show('Subcontractor added', 'success');
    });
  }

  onFileSelected(event: Event): void {
    const el = event.target as HTMLInputElement;
    if (el.files?.length) this.selectedFile = el.files[0];
  }

  importCsv(): void {
    if (!this.projectId || !this.selectedFile) return;
    this.api.importCsv(this.projectId, this.selectedFile).subscribe(() => {
      this.selectedFile = null;
      this.api.getSubs(this.projectId).subscribe(s => {
        this.subs = s;
        this.toast.show('CSV imported successfully', 'success');
      });
    });
  }
}
