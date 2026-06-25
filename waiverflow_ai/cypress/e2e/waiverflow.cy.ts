describe('WaiverFlow UI — Core flows', () => {
  beforeEach(() => {
    cy.visit('/');
    cy.url().should('include', '/login');
  });

  it('logs in as GC accountant and sees projects page', () => {
    cy.contains('GC Accountant').click();
    cy.url().should('include', '/projects');
    cy.contains('WaiverFlow').should('be.visible');
  });

  it('creates a project', () => {
    cy.contains('GC Accountant').click();
    cy.get('input[name="projectName"]').type('Test Project');
    cy.get('button').contains('Create').click();
    cy.contains('Test Project').should('be.visible');
  });

  it('navigates to subs and adds a subcontractor', () => {
    cy.contains('GC Accountant').click();
    cy.contains('Subs').first().click();
    cy.url().should('include', '/subs');
    cy.get('input[name="companyName"]').type('ABC Electric');
    cy.get('input[name="contactEmail"]').type('admin@abcelectric.com');
    cy.get('input[name="workState"]').type('TX');
    cy.get('button').contains('Add').click();
    cy.contains('ABC Electric').should('be.visible');
  });

  it('starts a pay cycle', () => {
    cy.contains('GC Accountant').click();
    cy.contains('Start Pay Cycle').first().click();
    cy.get('input[name="cycleLabel"]').type('June 2026');
    cy.get('input[name="dueDate"]').type('2026-07-15');
    cy.get('button').contains('Create').click();
  });

  it('views compliance dashboard', () => {
    cy.contains('GC Accountant').click();
    cy.contains('Compliance').first().click();
    cy.url().should('include', '/compliance');
  });

  it('views pay readiness and exports audit package', () => {
    cy.contains('GC Accountant').click();
    cy.contains('Pay Readiness').first().click();
    cy.url().should('include', '/pay-readiness');
    cy.get('button').contains('Export Audit Package').should('be.visible');
  });

  it('logs out and returns to login', () => {
    cy.contains('GC Accountant').click();
    cy.contains('Logout').click();
    cy.url().should('include', '/login');
  });
});
