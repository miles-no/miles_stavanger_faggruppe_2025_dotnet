# Authentication Workshop: .NET 8 & Entra ID

This project demonstrates how to secure a .NET 8 Web API using Microsoft Entra ID (formerly Azure AD). It covers public endpoints, authentication (Tenant/Audience validation), App Roles (Application Permissions), and Scopes (Delegated Permissions).

## Prerequisites

- .NET 8 SDK
- A Microsoft Entra ID Tenant
- Insomnia (for API testing)
- Access to make HTTP requests to Microsoft Graph API (e.g., Graph Explorer or Postman)

## Entra ID Setup Guide

To get the application working, you need to set up two App Registrations in Entra ID: one representing the **Server** (this API) and one representing the **Client** (the consumer of the API).

### 1. Server App Registration
1. Navigate to **Entra ID** > **App registrations** > **New registration**.
2. Name: `fagkveld-demo-server`.
3. Register.
4. **Important:** Note down the `Application (client) ID` and `Directory (tenant) ID`.

#### Configure Scope (For User/Delegated Access)
1. Go to **Expose an API**.
2. Set the **Application ID URI** (usually `api://<server-client-id>`).
3. Click **Add a scope**.
   - **Scope name:** `Scope.Test`
   - **Who can consent:** Admins and users
   - **Display name:** Test Scope
   - **Description:** Scope for testing delegated access.
   - **State:** Enabled.

#### Configure App Role (For App-to-App Access)
1. Go to **App roles**.
2. Click **Create app role**.
   - **Display name:** `Role.Test`
   - **Allowed member types:** Applications
   - **Value:** `Role.Test`
   - **Description:** Role for testing application permissions.
   - **Apply**.
3. **Important:** Open the **Manifest** tab or look at the list to find the **ID (GUID)** of this new App Role. You will need it later.

### 2. Client App Registration
1. Navigate to **App registrations** > **New registration**.
2. Name: `fagkveld-demo-client`.
3. Register.
4. Note down the `Application (client) ID`.

#### Create Client Secret
1. Go to **Certificates & secrets**.
2. Create a **New client secret**.
3. Copy the **Value** immediately (you won't see it again).

#### Configure API Permissions
1. Go to **API permissions** > **Add a permission**.
2. Select **My APIs** > `fagkveld-demo-server`.
3. Select **Application permissions**.
4. Check `Role.Test` and click **Add permissions**.

### 3. Grant App Role Assignment

#### Why use Graph API?
You might notice the "Grant admin consent" button in the Azure Portal is greyed out or requires Admin privileges. This button is a "big hammer" that grants permissions on behalf of the entire organization. However, as the **Owner** of both the Client and Server applications, you have the right to assign roles between them without being a Global Admin. The Portal UI doesn't easily expose this "Owner-level" assignment, so we use the Graph API to bypass the Admin requirement.

#### Manual Graph API Request
**Request:**
`POST https://graph.microsoft.com/v1.0/servicePrincipals/<server-enterprise-app-object-id>/appRoleAssignedTo`

> **Note:** You need the **Object ID** of the **Enterprise Application** (Service Principal), NOT the App Registration. You can find this by searching for the app name in the "Enterprise applications" blade in Entra ID.

**Body:**
```json
{
  "principalId": "<client-enterprise-app-object-id>",
  "resourceId": "<server-enterprise-app-object-id>",
  "appRoleId": "<app-role-guid-from-manifest>"
}
```

## Project Configuration

1. Open `Authentication/appsettings.json`.
2. Update the `AzureAd` section with your **Server** details:

```json
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "<your-tenant-id>",
  "ClientId": "<server-client-id>",
  "Audience": "<server-client-id>" 
}
```
*Note: If your token audience is `api://<client-id>`, update the `Audience` field accordingly.*

## Code Walkthrough

The project implements the same logic in two ways: **Minimal API** and **Controller API**.

### Endpoints
| Path | Auth Requirement | Description |
|------|------------------|-------------|
| `/public` | None | Accessible by anyone. |
| `/authenticated` | Valid Token | Requires a valid JWT from the tenant. |
| `/scope-required` | Scope: `Scope.Test` | Requires a token with the specific delegated scope. |
| `/role-required` | Role: `Role.Test` | Requires a token with the specific app role. |

### Implementation Details
- **Minimal API:** Located in `Endpoints/AuthEndpoints.cs`. Uses `.RequireAuthorization()` and policy logic.
- **Controllers:** Located in `Controllers/AuthController.cs`. Uses `[Authorize]`, `[RequiredScope]`, and `[Authorize(Roles = "...")]` attributes.

## Testing with Insomnia

Use the provided Insomnia collection and configure the OAuth 2.0 settings for the requests.

**Common Settings:**
- **Auth URL:** `https://login.microsoftonline.com/<tenant-id>/oauth2/v2.0/authorize`
- **Token URL:** `https://login.microsoftonline.com/<tenant-id>/oauth2/v2.0/token`
- **Client ID:** `<client-app-client-id>`
- **Client Secret:** `<client-app-secret>`

**For Scope Testing (Delegated):**
- **Grant Type:** Authorization Code
- **Scope:** `api://<server-client-id>/Scope.Test`

**For Role Testing (App-to-App):**
- **Grant Type:** Client Credentials
- **Scope:** `api://<server-client-id>/.default`
