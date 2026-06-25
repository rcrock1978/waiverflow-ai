export interface Project { id: string; name: string; description?: string; status: string; }
export interface CreateProjectRequest { name: string; description?: string; }
export interface Subcontractor { id: string; companyName: string; contactName: string; contactEmail: string; workState: string; phone?: string; coiExpiryDate?: string; }
export interface AddSubRequest { companyName: string; contactName: string; contactEmail: string; workState: string; phone?: string; }
export interface StartPayCycleRequest { label: string; dueDate: string; }
export interface WaiverRequest { id: string; subcontractorId: string; waiverType: string; amount: number; status: string; dueDate: string; }
export interface ComplianceStatus { companyName: string; expiryDate?: string; status: string; daysUntilExpiry: number; }
export interface PayReadiness { status: string; totalWaiverRequests: number; completedWaiverRequests: number; outstandingWaiverRequests: number; coiComplianceStatus: string; }
export interface ImportResult { imported: number; errors: { row: number; message: string }[]; totalErrors: number; }
export interface OverrideRequest { reason: string; }
