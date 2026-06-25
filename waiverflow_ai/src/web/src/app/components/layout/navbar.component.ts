import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar">
      <div class="nav-inner">
        <a class="brand" routerLink="/projects">WaiverFlow</a>
        <div class="nav-links">
          <a routerLink="/projects" routerLinkActive="active">Projects</a>
        </div>
        <div class="nav-user" *ngIf="auth.isLoggedIn">
          <span class="status-badge">{{ auth.role }}</span>
          <button class="btn btn-outline" (click)="logout()">Logout</button>
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {
  constructor(public auth: AuthService, private router: Router) {}

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
