# User Roles and Permissions

Access in Acme Translate is governed by workspace-level roles.

## Roles

- **Owner** — full control, including billing, workspace settings, and deleting the workspace. One or more per workspace.
- **Admin** — manage users, projects, and resources (glossaries, termbases, translation memories). Cannot access billing.
- **Project Manager** — create and manage projects, assign translators, approve deliverables.
- **Linguist** — access only assigned projects; edit translations in the editor; cannot download source files in bulk.
- **Viewer** — read-only access to projects they are invited to.

## Resource permissions

Glossaries and termbases are workspace-wide by default. Admins can restrict a resource to specific projects, in which case only members of those projects can view or apply it.

## SSO and provisioning

Enterprise workspaces support SAML SSO and SCIM provisioning. When SCIM is enabled, role assignment is controlled by your identity provider's group mapping and cannot be edited in-app.

## Audit log

Owners and Admins can export an audit log (CSV) covering sign-ins, permission changes, and file downloads for the past 12 months.
