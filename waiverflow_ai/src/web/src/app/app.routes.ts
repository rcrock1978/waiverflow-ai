import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { SubsComponent } from './components/subs/subs.component';
import { ComplianceComponent } from './components/compliance/compliance.component';
import { PayReadinessComponent } from './components/pay-readiness/pay-readiness.component';
import { SubmitComponent } from './components/submit/submit.component';
import { CoiComponent } from './components/coi/coi.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'projects', component: ProjectsComponent, canActivate: [AuthGuard] },
  { path: 'projects/:projectId/subs', component: SubsComponent, canActivate: [AuthGuard] },
  { path: 'projects/:projectId/compliance', component: ComplianceComponent, canActivate: [AuthGuard] },
  { path: 'projects/:projectId/pay-readiness', component: PayReadinessComponent, canActivate: [AuthGuard] },
  { path: 'submit/:waiverId', component: SubmitComponent },
  { path: 'coi', component: CoiComponent },
  { path: '', redirectTo: '/projects', pathMatch: 'full' },
];
