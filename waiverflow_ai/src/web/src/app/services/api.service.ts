import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Project, CreateProjectRequest, Subcontractor, AddSubRequest, StartPayCycleRequest, WaiverRequest, ComplianceStatus, PayReadiness, ImportResult, OverrideRequest } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = '/api/v1';
  constructor(private http: HttpClient) {}

  getProjects(): Observable<Project[]> { return this.http.get<Project[]>(`${this.base}/projects`); }
  createProject(req: CreateProjectRequest): Observable<Project> { return this.http.post<Project>(`${this.base}/projects`, req); }
  getSubs(projectId: string): Observable<Subcontractor[]> { return this.http.get<Subcontractor[]>(`${this.base}/projects/${projectId}/subcontractors`); }
  addSub(projectId: string, req: AddSubRequest): Observable<Subcontractor> { return this.http.post<Subcontractor>(`${this.base}/projects/${projectId}/subcontractors`, req); }
  importCsv(projectId: string, file: File): Observable<ImportResult> { const fd = new FormData(); fd.append('file', file); return this.http.post<ImportResult>(`${this.base}/projects/${projectId}/subcontractors/import`, fd); }
  startPayCycle(projectId: string, req: StartPayCycleRequest): Observable<WaiverRequest[]> { return this.http.post<WaiverRequest[]>(`${this.base}/projects/${projectId}/pay-cycles`, req); }
  getCompliance(projectId: string): Observable<ComplianceStatus[]> { return this.http.get<ComplianceStatus[]>(`${this.base}/projects/${projectId}/compliance`); }
  getPayReadiness(projectId: string): Observable<PayReadiness> { return this.http.get<PayReadiness>(`${this.base}/projects/${projectId}/pay-readiness`); }
  overrideWaiver(waiverId: string, req: OverrideRequest): Observable<any> { return this.http.post(`${this.base}/waiver-requests/${waiverId}/override`, req); }
}
