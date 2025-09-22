Society API Documentation

The Society API manages society information along with its associated loan types.
Each society can only exist once in the system (singleton pattern).
LoanTypes are tightly coupled with Society — any update replaces existing loan types with new ones.

🔹 Endpoints
1. Create Society

POST /api/society
🔒 Authorization: admin role only.
⚠️ Allowed only when no society exists in DB.

Request Body (JSON)
{
  "societyName": "Test Society",
  "address": "123 Street",
  "city": "Chennai",
  "phone": "9876543210",
  "fax": "044-1234567",
  "email": "info@test.com",
  "website": "www.test.com",
  "registrationNumber": "REG123",
  "chBounceCharge": 250.00,
  "targetDropdown": "target1",
  "loanTypes": [
    {
      "loanTypeName": "General Loan",
      "compulsoryDeposit": 1000,
      "optionalDeposit": 500,
      "share": 200,
      "limitAmount": 50000,
      "interest": 12.5,
      "xTimes": 2
    },
    {
      "loanTypeName": "Emergency Loan",
      "compulsoryDeposit": 500,
      "optionalDeposit": 200,
      "share": 100,
      "limitAmount": 20000,
      "interest": 15.0,
      "xTimes": 1
    }
  ]
}

Response (200 OK)
{
  "success": true,
  "message": "Society created successfully.",
  "data": {
    "id": 1,
    "societyName": "Test Society",
    "address": "123 Street",
    "city": "Chennai",
    "phone": "9876543210",
    "fax": "044-1234567",
    "email": "info@test.com",
    "website": "www.test.com",
    "registrationNumber": "REG123",
    "chBounceCharge": 250.0,
    "targetDropdown": "target1",
    "loanTypes": [
      {
        "loanTypeName": "General Loan",
        "compulsoryDeposit": 1000,
        "optionalDeposit": 500,
        "share": 200,
        "limitAmount": 50000,
        "interest": 12.5,
        "xTimes": 2
      },
      {
        "loanTypeName": "Emergency Loan",
        "compulsoryDeposit": 500,
        "optionalDeposit": 200,
        "share": 100,
        "limitAmount": 20000,
        "interest": 15.0,
        "xTimes": 1
      }
    ]
  }
}

2. Get Society

GET /api/society

Response (200 OK - if society exists)
{
  "success": true,
  "data": {
    "id": 1,
    "societyName": "Test Society",
    "address": "123 Street",
    "city": "Chennai",
    "phone": "9876543210",
    "fax": "044-1234567",
    "email": "info@test.com",
    "website": "www.test.com",
    "registrationNumber": "REG123",
    "chBounceCharge": 250.0,
    "targetDropdown": "target1",
    "loanTypes": [
      {
        "loanTypeName": "General Loan",
        "compulsoryDeposit": 1000,
        "optionalDeposit": 500,
        "share": 200,
        "limitAmount": 50000,
        "interest": 12.5,
        "xTimes": 2
      },
      {
        "loanTypeName": "Emergency Loan",
        "compulsoryDeposit": 500,
        "optionalDeposit": 200,
        "share": 100,
        "limitAmount": 20000,
        "interest": 15.0,
        "xTimes": 1
      }
    ]
  }
}

Response (200 OK - if no society exists)
{
  "success": true,
  "data": null,
  "message": "No society configuration found. Using default values."
}

3. Update Society

PUT /api/society
🔒 Authorization: admin role only.

Request Body (JSON)
{
  "id": 1,
  "societyName": "Updated Society",
  "address": "456 New Street",
  "city": "Mumbai",
  "phone": "9876543211",
  "fax": "022-7654321",
  "email": "support@test.com",
  "website": "www.updated.com",
  "registrationNumber": "REG999",
  "chBounceCharge": 300.00,
  "targetDropdown": "target2",
  "loanTypes": [
    {
      "loanTypeName": "General Loan",
      "compulsoryDeposit": 2000,
      "optionalDeposit": 1000,
      "share": 400,
      "limitAmount": 100000,
      "interest": 10.0,
      "xTimes": 3
    },
    {
      "loanTypeName": "Special Loan",
      "compulsoryDeposit": 1500,
      "optionalDeposit": 700,
      "share": 250,
      "limitAmount": 75000,
      "interest": 11.5,
      "xTimes": 2
    }
  ]
}

Important Behavior:

All existing loan types for the society are deleted.

New loan types from request body are inserted.

If request body has fewer loan types, missing ones are removed permanently.

Response (200 OK)
{
  "success": true,
  "message": "Society updated successfully.",
  "data": {
    "id": 1,
    "societyName": "Updated Society",
    "address": "456 New Street",
    "city": "Mumbai",
    "phone": "9876543211",
    "fax": "022-7654321",
    "email": "support@test.com",
    "website": "www.updated.com",
    "registrationNumber": "REG999",
    "chBounceCharge": 300.0,
    "targetDropdown": "target2",
    "loanTypes": [
      {
        "loanTypeName": "General Loan",
        "compulsoryDeposit": 2000,
        "optionalDeposit": 1000,
        "share": 400,
        "limitAmount": 100000,
        "interest": 10.0,
        "xTimes": 3
      },
      {
        "loanTypeName": "Special Loan",
        "compulsoryDeposit": 1500,
        "optionalDeposit": 700,
        "share": 250,
        "limitAmount": 75000,
        "interest": 11.5,
        "xTimes": 2
      }
    ]
  }
}

🔹 Error Responses

400 Bad Request

{
  "success": false,
  "message": "Society already exists. Only one society is allowed in the system."
}


404 Not Found

{
  "success": false,
  "message": "Society with Id 1 not found."
}


500 Internal Server Error

{
  "success": false,
  "message": "Error updating society information",
  "errors": ["<detailed error message>"]
}

🔹 Summary

Create (POST) → Only once when no society exists.

Get (GET) → Fetches society + loan types.

Update (PUT) → Updates one society and replaces all loan types with new ones.