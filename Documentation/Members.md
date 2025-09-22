📌 Members API Documentation
🔑 Authentication

All endpoints are secured with [Authorize].
👉 You must include a valid JWT Bearer token in the request headers:
Authorization: Bearer <your_token>

1. Get All Members

Endpoint: GET /api/member
Description: Fetches a list of all members.

Response (200 OK):

{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Arun Kumar",
      "fhName": "Ramesh Kumar",
      "officeAddress": "12th Floor",
      "city": "Bengaluru",
      "phoneOffice": "080-24567",
      "branch": "Koramangala",
      "phoneRes": "080-26784",
      "mobile": "9876543210",
      "mobile2": "9123456780",
      "designation": "Manager",
      "residenceAddress": "Flat 204, Green Apartments",
      "pincode": "560034",
      "dob": "1990-05-11",
      "dojSociety": "2021-08-01",
      "dor": null,
      "email": "arun.kumar@example.com",
      "email2": "arun.alt@example.com",
      "nominee": "Priya Kumari",
      "nomineeRelation": "Wife",
      "cdAmount": "150",
      "bankName": "HDFC Bank",
      "accountNumber": "123456789012",
      "payableAt": "Andheri Branch",
      "status": "Active",
      "createdAt": "2025-09-17T10:00:00Z",
      "updatedAt": "2025-09-17T12:00:00Z"
    }
  ]
}

2. Get Member by Id

Endpoint: GET /api/member/{id}
Description: Fetches details of a single member by their ID.

Response (200 OK):

{
  "success": true,
  "data": {
    "id": 1,
    "name": "Arun Kumar",
    "fhName": "Ramesh Kumar",
    "officeAddress": "12th Floor",
    "city": "Bengaluru",
    "phoneOffice": "080-24567",
    "branch": "Koramangala",
    "phoneRes": "080-26784",
    "mobile": "9876543210",
    "mobile2": "9123456780",
    "designation": "Manager",
    "residenceAddress": "Flat 204, Green Apartments",
    "pincode": "560034",
    "dob": "1990-05-11",
    "dojSociety": "2021-08-01",
    "dor": null,
    "email": "arun.kumar@example.com",
    "email2": "arun.alt@example.com",
    "nominee": "Priya Kumari",
    "nomineeRelation": "Wife",
    "cdAmount": "150",
    "bankName": "HDFC Bank",
    "accountNumber": "123456789012",
    "payableAt": "Andheri Branch",
    "status": "Active",
    "createdAt": "2025-09-17T10:00:00Z",
    "updatedAt": "2025-09-17T12:00:00Z"
  }
}


Error Response (404 Not Found):

{
  "success": false,
  "message": "Member not found"
}

3. Create Member

Endpoint: POST /api/member
Description: Creates a new member record.

Request Body:

{
  "name": "Kesar Devi",
  "fhName": "Nakul Sharma",
  "officeAddress": "5th Floor",
  "city": "Delhi",
  "phoneOffice": "07873-832",
  "branch": "JSD",
  "phoneRes": "08332-873",
  "mobile": "9871234560",
  "mobile2": "9123459876",
  "designation": "Member",
  "residenceAddress": "House No. 34",
  "pincode": "652378",
  "dob": "1995-01-01",
  "dojSociety": "2025-09-01",
  "email": "kesar@example.com",
  "email2": "kesar.alt@example.com",
  "nominee": "Priya Sharma",
  "nomineeRelation": "Wife",
  "cdAmount": "200",
  "bankName": "ICICI Bank",
  "accountNumber": "334567890124",
  "payableAt": "Connaught Place Branch",
  "status": "Active"
}


Response (200 OK):

{
  "success": true,
  "message": "Member created successfully",
  "data": {
    "id": 2,
    "name": "Kesar Devi",
    "fhName": "Nakul Sharma",
    "officeAddress": "5th Floor",
    "city": "Delhi",
    "phoneOffice": "07873-832",
    "branch": "JSD",
    "phoneRes": "08332-873",
    "mobile": "9871234560",
    "mobile2": "9123459876",
    "designation": "Member",
    "residenceAddress": "House No. 34",
    "pincode": "652378",
    "dob": "1995-01-01",
    "dojSociety": "2025-09-01",
    "dor": null,
    "email": "kesar@example.com",
    "email2": "kesar.alt@example.com",
    "nominee": "Priya Sharma",
    "nomineeRelation": "Wife",
    "cdAmount": "200",
    "bankName": "ICICI Bank",
    "accountNumber": "334567890124",
    "payableAt": "Connaught Place Branch",
    "status": "Active",
    "createdAt": "2025-09-17T12:15:00Z",
    "updatedAt": "2025-09-17T12:15:00Z"
  }
}

4. Update Member

Endpoint: PUT /api/member/{id}
Description: Updates details of an existing member.

Request Body (only send fields you want to update):

{
  "address": "789 Green Lane",
  "mobile": "9001122334",
  "status": "Inactive",
  "bankName": "SBI",
  "payableAt": "MG Road Branch"
}


Response (200 OK):

{
  "success": true,
  "message": "Member updated successfully",
  "data": {
    "id": 1,
    "name": "Arun Kumar",
    "fhName": "Ramesh Kumar",
    "officeAddress": "12th Floor",
    "city": "Bengaluru",
    "phoneOffice": "080-24567",
    "branch": "Koramangala",
    "phoneRes": "080-26784",
    "mobile": "9001122334",
    "mobile2": "9123456780",
    "designation": "Manager",
    "residenceAddress": "Flat 204, Green Apartments",
    "pincode": "560034",
    "dob": "1990-05-11",
    "dojSociety": "2021-08-01",
    "dor": null,
    "email": "arun.kumar@example.com",
    "email2": "arun.alt@example.com",
    "nominee": "Priya Kumari",
    "nomineeRelation": "Wife",
    "cdAmount": "150",
    "bankName": "SBI",
    "accountNumber": "123456789012",
    "payableAt": "MG Road Branch",
    "status": "Inactive",
    "createdAt": "2025-09-17T10:00:00Z",
    "updatedAt": "2025-09-17T12:20:00Z"
  }
}


Error Response (404 Not Found):

{
  "success": false,
  "message": "Member not found"
}

5. Delete Member

Endpoint: DELETE /api/member/{id}
Description: Deletes a member record.

Response (200 OK):

{
  "success": true,
  "message": "Member deleted successfully"
}


Error Response (404 Not Found):

{
  "success": false,
  "message": "Member not found"
}
