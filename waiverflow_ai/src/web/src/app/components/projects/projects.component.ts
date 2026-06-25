import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Project } from '../../models/models';
import { ToastService } from '../toast/toast.service';
import { PaginatePipe } from '../../pipes/paginate.pipe';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, PaginatePipe],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h1>Projects</h1>
        <button class="btn btn-primary" (click)="showCreateForm = !showCreateForm">
          {{ showCreateForm ? 'Cancel' : 'New Project' }}
        </button>
      </div>

      <div class="card" *ngIf="showCreateForm">
        <div class="card-body">
          <div class="form-group">
            <input class="form-input" [(ngModel)]="newProjectName" #nameRef="ngModel" placeholder="Project name" required />
            <div class="form-error" *ngIf="nameRef.invalid && nameRef.touched">Project name is required</div>
          </div>
          <div class="form-group">
            <input class="form-input" [(ngModel)]="newProjectDesc" placeholder="Description (optional)" />
          </div>
          <button class="btn btn-primary" (click)="createProject()" [disabled]="!newProjectName.trim()">Create</button>
        </div>
      </div>

      <ng-container *ngIf="projects !== null; else loading">
        <div class="grid-2" *ngIf="projects.length > 0; else empty">
          <div class="card" *ngFor="let p of projects | paginate:page:pageSize">
            <div class="card-body">
              <h3>{{ p.name }}</h3>
              <span class="status-badge">{{ p.status }}</span>
              <p *ngIf="p.description">{{ p.description }}</p>
              <div class="btn-group">
                <button class="btn btn-outline" (click)="router.navigate(['/projects', p.id, 'subs'])">Subs</button>
                <button class="btn btn-outline" (click)="router.navigate(['/projects', p.id, 'compliance'])">Compliance</button>
                <button class="btn btn-outline" (click)="router.navigate(['/projects', p.id, 'pay-readiness'])">Pay Readiness</button>
                <button class="btn btn-primary" (click)="openPayCycle(p.id)">Start Pay Cycle</button>
              </div>
            </div>
          </div>
          <div class="pagination" *ngIf="projects.length > pageSize">
            <button class="btn btn-outline" (click)="prevPage()" [disabled]="page === 1">Prev</button>
            <span>Page {{ page }} of {{ totalPages }}</span>
            <button class="btn btn-outline" (click)="nextPage()" [disabled]="page === totalPages">Next</button>
          </div>
        </div>

        <ng-template #empty>
          <div class="empty-state">
            <p>No projects yet. Create your first project to get started.</p>
          </div>
        </ng-template>
      </ng-container>

      <ng-template #loading>
        <div class="empty-state"><p>Loading...</p></div>
      </ng-template>

      <div class="modal-overlay" *ngIf="selectedProjectId" (click)="closePayCycle()">
        <div class="modal-content" (click)="$event.stopPropagation()">
          <h2>Start Pay Cycle</h2>
          <div class="form-group">
            <input class="form-input" [(ngModel)]="payCycleLabel" placeholder="Label (e.g. May 2025)" />
          </div>
          <div class="form-group">
            <input class="form-input" type="date" [(ngModel)]="payCycleDueDate" />
          </div>
          <button class="btn btn-primary" (click)="startPayCycle()" [disabled]="!payCycleLabel || !payCycleDueDate">Submit</button>
          <button class="btn btn-outline" (click)="closePayCycle()">Cancel</button>
        </div>
      </div>
    </div>
  `
})
export class ProjectsComponent implements OnInit {
  projects: Project[] | null = null;
  showCreateForm = false;
  newProjectName = '';
  newProjectDesc = '';
  selectedProjectId: string | null = null;
  payCycleLabel = '';
  payCycleDueDate = '';
  page = 1;
  pageSize = 10;

  constructor(public api: ApiService, public router: Router, private toast: ToastService) {}

  ngOnInit(): void {
    this.api.getProjects().subscribe(p => this.projects = p);
  }

  get totalPages(): number {
    return this.projects ? Math.ceil(this.projects.length / this.pageSize) : 1;
  }

  prevPage(): void {
    if (this.page > 1) this.page--;
  }

  nextPage(): void {
    if (this.page < this.totalPages) this.page++;
  }

  createProject(): void {
    if (!this.newProjectName.trim()) return;
    this.api.createProject({ name: this.newProjectName, description: this.newProjectDesc || undefined })
      .subscribe(p => {
        this.projects = [...(this.projects ?? []), p];
        this.showCreateForm = false;
        this.newProjectName = '';
        this.newProjectDesc = '';
        this.toast.show('Project created', 'success');
      });
  }

  openPayCycle(projectId: string): void {
    this.selectedProjectId = projectId;
    this.payCycleLabel = '';
    this.payCycleDueDate = '';
  }

  closePayCycle(): void {
    this.selectedProjectId = null;
  }

  startPayCycle(): void {
    if (!this.selectedProjectId || !this.payCycleLabel || !this.payCycleDueDate) return;
    this.api.startPayCycle(this.selectedProjectId, { label: this.payCycleLabel, dueDate: this.payCycleDueDate })
      .subscribe(() => {
        this.closePayCycle();
        this.toast.show('Pay cycle started', 'success');
      });
  }
}
