# Схеми колекцій PersonnelService (MongoDB)

Документ описує структуру основних колекцій мікросервісу **PersonnelService** для обліку персоналу транспортного підприємства.

---

## 1. Колекція: `personnel`

**Призначення:** зберігає основну інформацію про працівників.

### Структура документа
```json
{
  "_id": "ObjectId",
  "personnelId": "int",
  "fullName": "string",
  "birthDate": "date",
  "position": "string",
  "status": "string",
  "contacts": {
    "phone": "string",
    "email": "string",
    "address": "string"
  },
  "documents": [
    {
      "type": "string",
      "number": "string|null",
      "issuedBy": "string|null",
      "issuedOn": "date|string|null",
      "category": "string|null",
      "validUntil": "date|string|null"
    }
  ],
  "createdAt": "date",
  "updatedAt": "date"
}
Ключі:

_id — PK

personnelId — унікальний ідентифікатор працівника

JSON Schema (DDL)
{
  "bsonType": "object",
  "required": ["personnelId", "fullName", "birthDate", "position", "status", "contacts"],
  "properties": {
    "_id": { "bsonType": "objectId" },
    "personnelId": { "bsonType": "int" },
    "fullName": { "bsonType": "string" },
    "birthDate": { "bsonType": "date" },
    "position": { "bsonType": "string" },
    "status": { "bsonType": "string" },
    "contacts": {
      "bsonType": "object",
      "properties": {
        "phone": { "bsonType": "string" },
        "email": { "bsonType": "string" },
        "address": { "bsonType": "string" }
      }
    },
    "documents": {
      "bsonType": "array",
      "items": {
        "bsonType": "object",
        "properties": {
          "type": { "bsonType": "string" },
          "number": { "bsonType": ["string", "null"] },
          "issuedBy": { "bsonType": ["string", "null"] },
          "issuedOn": { "bsonType": ["date", "string", "null"] },
          "category": { "bsonType": ["string", "null"] },
          "validUntil": { "bsonType": ["date", "string", "null"] }
        }
      }
    },
    "createdAt": { "bsonType": "date" },
    "updatedAt": { "bsonType": "date" }
  }
}

     Колекція: personnel_documents
     Структура документа
{
  "_id": "ObjectId",
  "personnelId": "int",
  "docType": "string",
  "fileName": "string",
  "uploadedAt": "date|string",
  "issuedOn": "date|string",
  "validUntil": "date|string"
}

JSON Schema (DDL)
{
  "bsonType": "object",
  "required": ["personnelId", "docType", "fileName", "uploadedAt"],
  "properties": {
    "_id": { "bsonType": "objectId" },
    "personnelId": { "bsonType": "int" },
    "docType": { "bsonType": "string" },
    "fileName": { "bsonType": "string" },
    "uploadedAt": { "bsonType": ["date", "string"] },
    "issuedOn": { "bsonType": ["date", "string"] },
    "validUntil": { "bsonType": ["date", "string"] }
  }
}

Колекція: physical_examinations
Структура документа
{
  "_id": "ObjectId",
  "personnelId": "int",
  "examDate": "date|string",
  "result": "string",
  "doctorName": "string",
  "notes": "string|null",
  "metrics": {
    "height": "int",
    "weight": "int",
    "bloodPressure": "string",
    "vision": "string"
  }
}

JSON Schema (DDL)
{
  "bsonType": "object",
  "required": ["personnelId", "examDate", "result", "doctorName", "metrics"],
  "properties": {
    "_id": { "bsonType": "objectId" },
    "personnelId": { "bsonType": "int" },
    "examDate": { "bsonType": ["date", "string"] },
    "result": { "bsonType": "string" },
    "doctorName": { "bsonType": "string" },
    "notes": { "bsonType": ["string", "null"] },
    "metrics": {
      "bsonType": "object",
      "properties": {
        "height": { "bsonType": "int" },
        "weight": { "bsonType": "int" },
        "bloodPressure": { "bsonType": "string" },
        "vision": { "bsonType": "string" }
      }
    }
  }
}

    Колекція: work_shift_logs
    Структура документа
{
  "_id": "ObjectId",
  "personnelId": "int",
  "shiftDate": "date|string",
  "startTime": "string",
  "endTime": "string",
  "bus": {
    "busCountryNumber": "string",
    "brand": "string"
  },
  "route": {
    "routeNumber": "string",
    "distanceKm": "double"
  },
  "status": "string"
}

JSON Schema (DDL)
{
  "bsonType": "object",
  "required": ["personnelId", "shiftDate", "startTime", "endTime", "bus", "route"],
  "properties": {
    "_id": { "bsonType": "objectId" },
    "personnelId": { "bsonType": "int" },
    "shiftDate": { "bsonType": ["date", "string"] },
    "startTime": { "bsonType": "string" },
    "endTime": { "bsonType": "string" },
    "bus": {
      "bsonType": "object",
      "properties": {
        "busCountryNumber": { "bsonType": "string" },
        "brand": { "bsonType": "string" }
      }
    },
    "route": {
      "bsonType": "object",
      "properties": {
        "routeNumber": { "bsonType": "string" },
        "distanceKm": { "bsonType": "double" }
      }
    },
    "status": { "bsonType": "string" }
  }
}
