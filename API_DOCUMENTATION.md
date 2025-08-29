# FintcsApi Documentation

## Overview

FintcsApi is a financial technology API built with ASP.NET Core that provides user authentication and management services with role-based access control. The system implements JWT-based authentication with comprehensive user profile management capabilities.

## Base URL

```
http://localhost:5000
```

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

## Available Roles

- `user` - Default role for all new registrations  
- `admin` - Administrative access to all features (must be set manually)

**Note:** All users register with `user` role by default. Admin privileges must be assigned manually through database or admin endpoints.

---

## Endpoints

### 1. User Registration

**Endpoint:** `POST /api/auth/register`

Authorization: Bearer YOUR_JWT_TOKEN_HERE (admin authorization required)

**Description:** Register a new user with optional role assignment.

**Request Body:**
```json
{
  "username": "string (required, 3-50 characters)",
  "password": "string (required, 6-100 characters)",
  "email": "string (required, valid email format)",
  "phone": "string (optional, valid phone format)",
  "EDPNo": "string (optional)",
  "Name": "string (optional)", 
  "AddressOffice": "string (optional)",
  "AddressResidential": "string (optional)",
  "Designation": "string (optional)",
  "PhoneOffice": "string (optional)",
  "PhoneResidential": "string (optional)",
  "Mobile": "string (optional)"
}
```

**Note:** Role is automatically set to "user" for all registrations.

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "id": 1,
    "username": "testuser",
    "email": "user@example.com",
    "phone": "+1234567890",
    "roles": "user",
    "details": {
      "EDPNo": "EDP001",
      "Name": "John Doe",
      "AddressOffice": "123 Business Center",
      "AddressResidential": "456 Home Street",
      "Designation": "Software Engineer",
      "PhoneOffice": "011-12345678",
      "PhoneResidential": "011-87654321",
      "Mobile": "+1234567890",
      "Email": "user@example.com"
    },
    "createdAt": "2025-08-26T17:25:26.793Z"
  },
  "errors": []
}
```

**Failure Responses:**

**400 Bad Request - User Already Exists:**
```json
{
  "success": false,
  "message": "User already exists",
  "data": null,
  "errors": ["Username 'testuser' is already taken"]
}
```

**400 Bad Request - Email Already Registered:**
```json
{
  "success": false,
  "message": "Email already registered",
  "data": null,
  "errors": ["Email 'user@example.com' is already registered"]
}
```

**400 Bad Request - Validation Error:**
```json
{
  "success": false,
  "message": "Invalid input data",
  "data": null,
  "errors": [
    "The Username field is required.",
    "The Password field is required.",
    "The Email field is required."
  ]
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "message": "Internal server error occurred during registration",
  "data": null,
  "errors": ["Detailed error message"]
}
```

---

### 2. User Login

**Endpoint:** `POST /api/auth/login`

**Description:** Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "username": "string (required)",
  "password": "string (required)"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "testuser",
    "email": "user@example.com",
    "phone": "+1234567890",
    "roles": "user",
    "details": {
      "EDPNo": "EDP001",
      "Name": "John Doe",
      "AddressOffice": "123 Business Center",
      "AddressResidential": "456 Home Street",
      "Designation": "Software Engineer",
      "PhoneOffice": "011-12345678",
      "PhoneResidential": "011-87654321",
      "Mobile": "+1234567890",
      "Email": "user@example.com"
    },
    "expiresAt": "2025-08-26T19:25:32.314Z"
  },
  "errors": []
}
```

**Failure Responses:**

**401 Unauthorized - Invalid Credentials:**
```json
{
  "success": false,
  "message": "Invalid credentials",
  "data": null,
  "errors": ["Username or password is incorrect"]
}
```

**400 Bad Request - Validation Error:**
```json
{
  "success": false,
  "message": "Invalid input data",
  "data": null,
  "errors": [
    "The Username field is required.",
    "The Password field is required."
  ]
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "message": "Internal server error occurred during login",
  "data": null,
  "errors": ["Detailed error message"]
}
```

---

### 3. Get Valid Roles

**Endpoint:** `GET /api/auth/roles`

**Description:** Get list of all valid roles in the system.

**Authentication:** Not required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Valid roles retrieved successfully",
  "data": ["user", "admin"],
  "errors": []
}
```

---

### 4. Get All Users (Admin Only)

**Endpoint:** `GET /api/users`

**Description:** Retrieve list of all users in the system.

**Authentication:** Required (Admin role only)

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Users retrieved successfully",
  "data": [
    {
      "id": 1,
      "username": "admin",
      "email": "admin@example.com",
      "phone": "+1234567890",
      "roles": ["admin", "user"],
      "createdAt": "2025-08-26T17:24:44.220Z"
    },
    {
      "id": 2,
      "username": "testuser",
      "email": "user@example.com",
      "phone": "+9876543210",
      "roles": ["user"],
      "createdAt": "2025-08-26T17:25:26.793Z"
    }
  ],
  "errors": []
}
```

**Failure Responses:**

**401 Unauthorized - No Token:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

**403 Forbidden - Insufficient Role:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "message": "Error retrieving users",
  "data": null,
  "errors": ["Detailed error message"]
}
```

---

### 5. Get Current User Profile

**Endpoint:** `GET /api/users/me`

**Description:** Get the profile information of the currently authenticated user.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User profile retrieved successfully",
  "data": {
    "id": 2,
    "username": "testuser",
    "email": "user@example.com",
    "phone": "+9876543210",
    "roles": ["user"],
    "createdAt": "2025-08-26T17:25:26.793Z"
  },
  "errors": []
}
```

**Failure Responses:**

**401 Unauthorized - Invalid Token:**
```json
{
  "success": false,
  "message": "Invalid user token",
  "data": null,
  "errors": []
}
```

**404 Not Found - User Not Found:**
```json
{
  "success": false,
  "message": "User not found",
  "data": null,
  "errors": []
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "message": "Error retrieving user profile",
  "data": null,
  "errors": ["Detailed error message"]
}
```

---

### 6. Update User Role (Admin Only)

**Endpoint:** `PUT /api/users/{id}/role`

**Description:** Update the role assigned to a specific user.

**Authentication:** Required (Admin role only)

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
Content-Type: application/json
```

**URL Parameters:**
- `id` (integer) - The ID of the user to update

**Request Body:**
```json
["admin", "manager"]
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User roles updated successfully",
  "data": {
    "id": 2,
    "username": "testuser",
    "email": "user@example.com",
    "phone": "+9876543210",
    "roles": ["admin", "manager"],
    "createdAt": "2025-08-26T17:25:26.793Z"
  },
  "errors": []
}
```

**Failure Responses:**

**404 Not Found - User Not Found:**
```json
{
  "success": false,
  "message": "User not found",
  "data": null,
  "errors": []
}
```

**401 Unauthorized - No Token:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

**403 Forbidden - Insufficient Role:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "message": "Error updating user roles",
  "data": null,
  "errors": ["Detailed error message"]
}
```

---

## Usage Examples

### Example 1: Register a New Admin User

```bash
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "adminuser",
    "password": "AdminPass123!",
    "email": "admin@company.com",
    "phone": "+1234567890",
    "roles": ["admin", "manager"]
  }'
```

### Example 2: Login and Get Token

```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "adminuser",
    "password": "AdminPass123!"
  }'
```

### Example 3: Get All Users (Using Token)

```bash
curl -X GET "http://localhost:5000/api/users" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Example 4: Update User Role

```bash
curl -X PUT "http://localhost:5000/api/users/2/role" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '"admin"'
```

---

## Society Management

### 7. Get Society Information

**Endpoint:** `GET /api/society`

**Description:** Retrieve current society configuration information.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "societyName": "ABC Credit Society",
    "address": "123 Main Street",
    "city": "Mumbai",
    "phone": "022-12345678",
    "fax": "022-87654321",
    "email": "contact@abcsociety.com",
    "website": "https://www.abcsociety.com",
    "registrationNumber": "REG001",
    "tabs": "{\"Interest\":{\"Dividend\":8.5,\"OD\":12.0,\"CD\":6.0,\"Loan\":10.0,\"EmergencyLoan\":15.0,\"LAS\":7.0},\"Limit\":{\"Share\":100000,\"Loan\":500000,\"EmergencyLoan\":50000}}",
    "isPendingApproval": false,
    "createdAt": "2025-01-01T10:00:00.000Z",
    "updatedAt": "2025-01-01T10:00:00.000Z"
  },
  "message": null
}
```

**Success Response - No Society Found (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 0,
    "societyName": "",
    "address": "",
    "city": "",
    "phone": "",
    "fax": "",
    "email": "",
    "website": "",
    "registrationNumber": "",
    "tabs": "{}",
    "isPendingApproval": false,
    "createdAt": "0001-01-01T00:00:00",
    "updatedAt": "0001-01-01T00:00:00"
  },
  "message": "No society configuration found. Using default values."
}
```

---

### 8. Update Society Information (Admin Only)

**Endpoint:** `PUT /api/society`

**Description:** Submit society information updates for approval. Changes will be pending until approved by all users.

**Authentication:** Required (Admin role only)

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
Content-Type: application/json
```

**Request Body:**
```json
{
  "societyName": "ABC Credit Society Ltd",
  "address": "456 Updated Street",
  "city": "Mumbai",
  "phone": "022-11223344",
  "fax": "022-44332211",
  "email": "info@abcsociety.com",
  "website": "https://www.abcsociety.org",
  "registrationNumber": "REG001-UPD",
  "tabs": {
    "interest": {
      "dividend": 9.0,
      "od": 12.5,
      "cd": 6.5,
      "loan": 10.5,
      "emergencyLoan": 15.5,
      "las": 7.5
    },
    "limit": {
      "share": 150000,
      "loan": 600000,
      "emergencyLoan": 75000
    }
  }
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Society update submitted for approval. All users must approve before changes become permanent."
}
```

---

### 9. Approve Society Changes

**Endpoint:** `POST /api/society/approve-changes`

**Description:** Approve and apply pending society changes.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "societyName": "ABC Credit Society Ltd",
    "address": "456 Updated Street",
    "city": "Mumbai",
    "phone": "022-11223344",
    "fax": "022-44332211",
    "email": "info@abcsociety.com",
    "website": "https://www.abcsociety.org",
    "registrationNumber": "REG001-UPD",
    "tabs": "{\"Interest\":{\"Dividend\":9.0,\"OD\":12.5,\"CD\":6.5,\"Loan\":10.5,\"EmergencyLoan\":15.5,\"LAS\":7.5},\"Limit\":{\"Share\":150000,\"Loan\":600000,\"EmergencyLoan\":75000}}",
    "isPendingApproval": false,
    "createdAt": "2025-01-01T10:00:00.000Z",
    "updatedAt": "2025-01-01T12:00:00.000Z"
  },
  "message": "Society changes approved and applied successfully"
}
```

**Failure Response - No Pending Changes (400 Bad Request):**
```json
{
  "success": false,
  "message": "No pending changes to approve"
}
```

---

### 10. Get Society Pending Changes

**Endpoint:** `GET /api/society/pending-changes`

**Description:** Check if there are pending society changes awaiting approval.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response - Has Pending Changes (200 OK):**
```json
{
  "success": true,
  "data": {
    "hasPendingChanges": true,
    "pendingChanges": "{\"SocietyName\":\"ABC Credit Society Ltd\",\"Address\":\"456 Updated Street\",\"City\":\"Mumbai\",\"Phone\":\"022-11223344\",\"Fax\":\"022-44332211\",\"Email\":\"info@abcsociety.com\",\"Website\":\"https://www.abcsociety.org\",\"RegistrationNumber\":\"REG001-UPD\",\"Tabs\":{\"Interest\":{\"Dividend\":9.0,\"OD\":12.5,\"CD\":6.5,\"Loan\":10.5,\"EmergencyLoan\":15.5,\"LAS\":7.5},\"Limit\":{\"Share\":150000,\"Loan\":600000,\"EmergencyLoan\":75000}}}"
  },
  "message": null
}
```

**Success Response - No Pending Changes (200 OK):**
```json
{
  "success": true,
  "data": null,
  "message": "No pending changes"
}
```

---

## Member Management

### 11. Get All Members

**Endpoint:** `GET /api/member`

**Description:** Retrieve list of all members in the society.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "memNo": "MEM_001",
      "name": "John Doe",
      "fhName": "Robert Doe",
      "officeAddress": "123 Business Center",
      "city": "Mumbai",
      "phoneOffice": "022-12345678",
      "branch": "IT Department",
      "phoneRes": "022-87654321",
      "mobile": "9876543210",
      "designation": "Software Engineer",
      "residenceAddress": "456 Home Street",
      "dob": "1985-06-15T00:00:00",
      "dojSociety": "2020-01-01T00:00:00",
      "email": "john.doe@company.com",
      "dojOrg": "2018-03-15T00:00:00",
      "dor": null,
      "nominee": "Jane Doe",
      "nomineeRelation": "Wife",
      "bankingDetails": {
        "bankName": "State Bank of India",
        "accountNumber": "12345678901",
        "ifscCode": "SBIN0001234",
        "branch": "Main Branch"
      },
      "isPendingApproval": false,
      "createdAt": "2025-01-01T10:00:00.000Z",
      "updatedAt": "2025-01-01T10:00:00.000Z"
    }
  ]
}
```

---

### 12. Get Member by ID

**Endpoint:** `GET /api/member/{id}`

**Description:** Retrieve detailed information of a specific member.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**URL Parameters:**
- `id` (integer) - The ID of the member

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "memNo": "MEM_001",
    "name": "John Doe",
    "fhName": "Robert Doe",
    "officeAddress": "123 Business Center",
    "city": "Mumbai",
    "phoneOffice": "022-12345678",
    "branch": "IT Department",
    "phoneRes": "022-87654321",
    "mobile": "9876543210",
    "designation": "Software Engineer",
    "residenceAddress": "456 Home Street",
    "dob": "1985-06-15T00:00:00",
    "dojSociety": "2020-01-01T00:00:00",
    "email": "john.doe@company.com",
    "dojOrg": "2018-03-15T00:00:00",
    "dor": null,
    "nominee": "Jane Doe",
    "nomineeRelation": "Wife",
    "bankingDetails": {
      "bankName": "State Bank of India",
      "accountNumber": "12345678901",
      "ifscCode": "SBIN0001234",
      "branch": "Main Branch"
    },
    "isPendingApproval": false,
    "createdAt": "2025-01-01T10:00:00.000Z",
    "updatedAt": "2025-01-01T10:00:00.000Z"
  }
}
```

**Failure Response - Member Not Found (404 Not Found):**
```json
{
  "success": false,
  "message": "Member not found"
}
```

---

### 13. Create New Member

**Endpoint:** `POST /api/member`

**Description:** Create a new member with auto-generated member number.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "John Doe",
  "fhName": "Robert Doe",
  "officeAddress": "123 Business Center",
  "city": "Mumbai",
  "phoneOffice": "022-12345678",
  "branch": "IT Department",
  "phoneRes": "022-87654321",
  "mobile": "9876543210",
  "designation": "Software Engineer",
  "residenceAddress": "456 Home Street",
  "dob": "1985-06-15T00:00:00",
  "dojSociety": "2020-01-01T00:00:00",
  "email": "john.doe@company.com",
  "dojOrg": "2018-03-15T00:00:00",
  "dor": null,
  "nominee": "Jane Doe",
  "nomineeRelation": "Wife",
  "bankingDetails": {
    "bankName": "State Bank of India",
    "accountNumber": "12345678901",
    "ifscCode": "SBIN0001234",
    "branch": "Main Branch"
  }
}
```

**Success Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "memNo": "MEM_001",
    "name": "John Doe",
    "fhName": "Robert Doe",
    "officeAddress": "123 Business Center",
    "city": "Mumbai",
    "phoneOffice": "022-12345678",
    "branch": "IT Department",
    "phoneRes": "022-87654321",
    "mobile": "9876543210",
    "designation": "Software Engineer",
    "residenceAddress": "456 Home Street",
    "dob": "1985-06-15T00:00:00",
    "dojSociety": "2020-01-01T00:00:00",
    "email": "john.doe@company.com",
    "dojOrg": "2018-03-15T00:00:00",
    "dor": null,
    "nominee": "Jane Doe",
    "nomineeRelation": "Wife",
    "bankingDetails": {
      "bankName": "State Bank of India",
      "accountNumber": "12345678901",
      "ifscCode": "SBIN0001234",
      "branch": "Main Branch"
    },
    "isPendingApproval": false,
    "createdAt": "2025-01-01T10:00:00.000Z",
    "updatedAt": "2025-01-01T10:00:00.000Z"
  },
  "message": "Member created successfully"
}
```

---

### 14. Update Member Information

**Endpoint:** `PUT /api/member/{id}`

**Description:** Submit member information updates for approval. Changes will be pending until approved.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
Content-Type: application/json
```

**URL Parameters:**
- `id` (integer) - The ID of the member to update

**Request Body:**
```json
{
  "name": "John Updated Doe",
  "fhName": "Robert Doe",
  "officeAddress": "789 New Business Center",
  "city": "Mumbai",
  "phoneOffice": "022-11223344",
  "branch": "IT Department",
  "phoneRes": "022-44332211",
  "mobile": "9876543210",
  "designation": "Senior Software Engineer",
  "residenceAddress": "456 Home Street",
  "dob": "1985-06-15T00:00:00",
  "dojSociety": "2020-01-01T00:00:00",
  "email": "john.doe@company.com",
  "dojOrg": "2018-03-15T00:00:00",
  "dor": null,
  "nominee": "Jane Doe",
  "nomineeRelation": "Wife",
  "bankingDetails": {
    "bankName": "HDFC Bank",
    "accountNumber": "98765432101",
    "ifscCode": "HDFC0001234",
    "branch": "New Branch"
  }
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Member update submitted for approval. All users must approve before changes become permanent."
}
```

**Failure Response - Member Not Found (404 Not Found):**
```json
{
  "success": false,
  "message": "Member not found"
}
```

---

### 15. Approve Member Changes

**Endpoint:** `POST /api/member/{id}/approve-changes`

**Description:** Approve and apply pending member changes.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**URL Parameters:**
- `id` (integer) - The ID of the member

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "memNo": "MEM_001",
    "name": "John Updated Doe",
    "fhName": "Robert Doe",
    "officeAddress": "789 New Business Center",
    "city": "Mumbai",
    "phoneOffice": "022-11223344",
    "branch": "IT Department",
    "phoneRes": "022-44332211",
    "mobile": "9876543210",
    "designation": "Senior Software Engineer",
    "residenceAddress": "456 Home Street",
    "dob": "1985-06-15T00:00:00",
    "dojSociety": "2020-01-01T00:00:00",
    "email": "john.doe@company.com",
    "dojOrg": "2018-03-15T00:00:00",
    "dor": null,
    "nominee": "Jane Doe",
    "nomineeRelation": "Wife",
    "bankingDetails": {
      "bankName": "HDFC Bank",
      "accountNumber": "98765432101",
      "ifscCode": "HDFC0001234",
      "branch": "New Branch"
    },
    "isPendingApproval": false,
    "createdAt": "2025-01-01T10:00:00.000Z",
    "updatedAt": "2025-01-01T12:00:00.000Z"
  },
  "message": "Member changes approved and applied successfully"
}
```

**Failure Response - No Pending Changes (400 Bad Request):**
```json
{
  "success": false,
  "message": "No pending changes to approve for this member"
}
```

---

### 16. Get Members with Pending Changes

**Endpoint:** `GET /api/member/pending-changes`

**Description:** Retrieve list of all members that have pending changes awaiting approval.

**Authentication:** Required

**Headers:**
```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "memNo": "MEM_001",
      "name": "John Doe",
      "pendingChanges": "{\"Name\":\"John Updated Doe\",\"FHName\":\"Robert Doe\",\"OfficeAddress\":\"789 New Business Center\",\"City\":\"Mumbai\",\"PhoneOffice\":\"022-11223344\",\"Branch\":\"IT Department\",\"PhoneRes\":\"022-44332211\",\"Mobile\":\"9876543210\",\"Designation\":\"Senior Software Engineer\",\"ResidenceAddress\":\"456 Home Street\",\"DOB\":\"1985-06-15T00:00:00\",\"DOJSociety\":\"2020-01-01T00:00:00\",\"Email\":\"john.doe@company.com\",\"DOJOrg\":\"2018-03-15T00:00:00\",\"DOR\":null,\"Nominee\":\"Jane Doe\",\"NomineeRelation\":\"Wife\",\"BankingDetails\":{\"BankName\":\"HDFC Bank\",\"AccountNumber\":\"98765432101\",\"IfscCode\":\"HDFC0001234\",\"Branch\":\"New Branch\"}}"
    }
  ]
}
```

---

## Error Codes Summary

| HTTP Status | Description |
|-------------|-------------|
| 200 | Success |
| 400 | Bad Request - Invalid input data or business logic error |
| 401 | Unauthorized - Missing or invalid authentication token |
| 403 | Forbidden - Insufficient permissions for the requested operation |
| 404 | Not Found - Requested resource does not exist |
| 500 | Internal Server Error - Unexpected server error |

---

## Role-Based Access Control

| Endpoint | Roles Required |
|----------|----------------|
| `POST /api/auth/register` | Admin |
| `POST /api/auth/login` | None (Public) |
| `GET /api/auth/roles` | None (Public) |
| `GET /api/users` | Admin |
| `GET /api/users/me` | Any authenticated user |
| `PUT /api/users/{id}/roles` | Admin |
| `GET /api/society` | Any authenticated user |
| `PUT /api/society` | Admin |
| `POST /api/society/approve-changes` | Any authenticated user |
| `GET /api/society/pending-changes` | Any authenticated user |
| `GET /api/member` | Any authenticated user |
| `GET /api/member/{id}` | Any authenticated user |
| `POST /api/member` | Any authenticated user |
| `PUT /api/member/{id}` | Any authenticated user |
| `POST /api/member/{id}/approve-changes` | Any authenticated user |
| `GET /api/member/pending-changes` | Any authenticated user |

---

## Token Information

- **Token Type:** JWT (JSON Web Token)
- **Expiration:** 2 hours from login
- **Claims Included:**
  - `unique_name` - Username
  - `UserId` - User ID
  - `Email` - User email
  - `role` - Array of user roles

---

## Database

The API uses SQLite database with the following table structure:

**Users Table:**
- `Id` - Primary key (auto-increment)
- `Username` - Unique username
- `PasswordHash` - BCrypt hashed password
- `Details` - JSON string containing email, phone, and roles
- `CreatedAt` - Timestamp of user creation
- `UpdatedAt` - Timestamp of last update

## Manual Admin User Creation

To create an admin user manually, execute this SQL query in your SQLite database:

```sql
-- First, create the JSON details for the admin user
-- Note: Replace the password hash below with a fresh BCrypt hash of "admin"
INSERT INTO Users (Username, PasswordHash, Details, CreatedAt, UpdatedAt)
VALUES (
  'admin', 
  '$2a$11$EXAMPLE_REPLACE_WITH_ACTUAL_BCRYPT_HASH_OF_admin_PASSWORD', 
  '{"email":"admin@fintcs.com","phone":"9876543210","role":"admin","EDPNo":"EDP001","Name":"Society Administrator","AddressOffice":"123 Finance Tower, City Center","AddressResidential":"45 Admin Colony, Main Road","Designation":"Super Admin","PhoneOffice":"011-22445566","PhoneResidential":"011-77889900","Mobile":"9876543210"}',
  datetime('now'),
  datetime('now')
);
```

**Admin User Details:**
- **Username:** admin
- **Password:** admin  
- **Email:** admin@fintcs.com
- **Phone:** 9876543210
- **Role:** admin
- **Complete Profile:** Includes EDP number, addresses, designations, etc.

**Note:** Generate a proper BCrypt hash for the password "admin" and replace the example hash in the SQL query above.