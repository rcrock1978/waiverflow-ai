-- RLS: Row-Level Security per tenant
-- Apply to every table that carries a TenantId column.

CREATE POLICY tenant_isolation ON "Projects"
    USING ("TenantId" = current_setting('app.tenant_id')::uuid);

CREATE POLICY tenant_isolation ON "Subcontractors"
    USING ("TenantId" = current_setting('app.tenant_id')::uuid);

CREATE POLICY tenant_isolation ON "WaiverRequests"
    USING ("TenantId" = current_setting('app.tenant_id')::uuid);

CREATE POLICY tenant_isolation ON "ComplianceDocs"
    USING ("TenantId" = current_setting('app.tenant_id')::uuid);

CREATE POLICY tenant_isolation ON "PayApplications"
    USING ("TenantId" = current_setting('app.tenant_id')::uuid);

CREATE POLICY tenant_isolation ON "AuditLogs"
    USING ("TenantId" = current_setting('app.tenant_id')::uuid);

-- Enable RLS on all tenant-isolated tables
ALTER TABLE "Projects" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Subcontractors" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "WaiverRequests" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "ComplianceDocs" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "PayApplications" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "AuditLogs" ENABLE ROW LEVEL SECURITY;
