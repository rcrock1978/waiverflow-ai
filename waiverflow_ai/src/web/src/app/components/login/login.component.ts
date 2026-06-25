import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="login-page">
      <div class="login-card">
        <div class="login-header">
          <div class="logo">WF</div>
          <h1>WaiverFlow AI</h1>
          <p>Select your role to continue</p>
        </div>
        <div class="login-body">
          <button class="role-btn" (click)="login('gc-accountant')">
            <span class="role-icon">&#128200;</span>
            <span class="role-label">GC Accountant</span>
          </button>
          <button class="role-btn" (click)="login('gc-admin')">
            <span class="role-icon">&#128736;</span>
            <span class="role-label">GC Admin</span>
          </button>
          <button class="role-btn" (click)="login('sub-admin')">
            <span class="role-icon">&#128188;</span>
            <span class="role-label">Sub Admin</span>
          </button>
          <button class="role-btn" (click)="login('controller')">
            <span class="role-icon">&#128202;</span>
            <span class="role-label">Controller</span>
          </button>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  constructor(private auth: AuthService, private router: Router) {}

  login(role: string): void {
    this.auth.login('default', role, role);
    this.router.navigate(['/projects']);
  }
}
