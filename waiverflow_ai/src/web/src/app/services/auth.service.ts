import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class AuthService {
  private key = 'wf_user';
  get user(): any { const r = localStorage.getItem(this.key); return r ? JSON.parse(r) : null; }
  get isLoggedIn(): boolean { return this.user !== null; }
  get role(): string { return this.user?.role ?? ''; }
  login(tenantId: string, name: string, role: string) { localStorage.setItem(this.key, JSON.stringify({ tenantId, name, role })); }
  logout() { localStorage.removeItem(this.key); }
}
