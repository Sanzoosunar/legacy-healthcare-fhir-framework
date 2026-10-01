# System Architecture

## Overview

The Legacy Healthcare to FHIR Integration Framework is designed as an API-based adapter between legacy healthcare data and modern interoperability standards.

The framework accepts legacy healthcare data, identifies its structure, maps the data to a common internal model, validates it, and transforms it into FHIR R4 resources.

The Angular application provides a user interface for the prototype, while the ASP.NET Core API acts as the main integration layer. This allows other systems to interact with the adapter through API endpoints without depending on the web interface.

## High-Level Architecture

```text
Legacy Healthcare Data
        │
        ▼
┌───────────────────────┐
│   Angular Web Client  │
│                       │
│ Login                 │
│ Dashboard             │
│ Upload                │
│ Human Approval        │
│ Job Monitoring        │
│ FHIR Download         │
└───────────┬───────────┘
            │
            │ HTTP / API
            ▼
┌───────────────────────┐
│ ASP.NET Core Web API  │
│                       │
│ Authentication        │
│ Job Management        │
│ Approval Endpoints    │
│ Output Access         │
│ SignalR               │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│ Background Job Queue  │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│ Background Job Worker │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────────────┐
│      Processing Pipeline      │
│                               │
│ Resource Type Detection       │
│            ↓                  │
│ Field Mapping                 │
│            ↓                  │
│ Normalization & Validation    │
│            ↓                  │
│ FHIR Transformation           │
│            ↓                  │
│ Completion & Cleanup          │
└───────────────────────────────┘
        │          │          │
        ▼          ▼          ▼
   AI Service    SQLite    File Storage
```

## Main Components

### Angular Web Client

The Angular application provides the user interface for:

- Login and logout
- Dashboard and job history
- Legacy file upload
- Resource type review and approval
- Field mapping review and approval
- Real-time job progress
- FHIR JSON download

The UI communicates with the backend through API endpoints and receives job progress updates through SignalR.

### ASP.NET Core Web API

The API is the main entry point to the framework.

It handles authentication, job creation, resource type and field mapping approvals, job information, validation results, and access to generated output.

The API also allows the adapter to be used by other systems without requiring the Angular interface.

### Background Processing

Long-running conversion work is processed asynchronously.

A background queue sends jobs to a worker, which selects the processor responsible for the current stage.

```text
ResourceTypeDetectionProcessor
        ↓
FieldMappingProcessor
        ↓
DataValidationProcessor
        ↓
FhirTransformationProcessor
        ↓
CompletedProcessor
```

This keeps the API responsive while the conversion continues in the background.

## Processing Flow

### 1. Resource Type Detection

The system creates a fingerprint from the uploaded data structure.

If an approved configuration already exists for the fingerprint, the stored resource type is reused.

Otherwise, AI suggests a FHIR resource type such as:

- Patient
- Observation
- Encounter

The user can review or change the suggestion before approval.

### 2. Field Mapping

Legacy fields are mapped to normalized fields.

For example:

```text
F_NAME  → FirstName
L_NAME  → LastName
DOB     → DateOfBirth
SEX     → Gender
```

Previously approved mappings are reused when recognized. Unknown mappings are sent to the AI service for suggestions.

The user can review and correct the mappings before approval.

### 3. Normalization and Validation

Approved mappings are used to convert legacy records into normalized internal models.

```text
Legacy Data
     ↓
Approved Mapping
     ↓
Normalized Data
     ↓
Validation
```

Validation is performed using deterministic application logic.

If validation fails, processing stops and the job is marked as failed.

### 4. FHIR Transformation

Valid normalized data is transformed into FHIR R4 resources.

```text
PatientData      → FHIR Patient
ObservationData  → FHIR Observation
EncounterData    → FHIR Encounter
```

FHIR transformation is deterministic and uses the Firely .NET SDK.

The generated resources are serialized into FHIR JSON and saved as the job output.

### 5. Completion

After successful transformation:

- The job is completed.
- The uploaded legacy source file is removed.
- The generated FHIR JSON remains available for download.

## AI and Human Review

AI is used only where legacy structures may vary.

| Process | AI |
| --- | --- |
| Resource type suggestion | Yes |
| Field mapping suggestion | Yes |
| Human approval | No |
| Data normalization | No |
| Data validation | No |
| FHIR transformation | No |

AI suggestions are reviewed by the user before they become approved configurations.

Approved configurations can then be reused when the same structure is recognized again.

## Configuration Reuse

The framework uses fingerprints to recognize previously configured structures.

```text
First Processing

Legacy Structure
      ↓
AI Suggestion
      ↓
Human Approval
      ↓
Store Approved Configuration


Later Processing

Recognized Structure
      ↓
Fingerprint Match
      ↓
Reuse Approved Configuration
      ↓
Normalization & Validation
      ↓
FHIR Transformation
```

Configuration reuse reduces repeated AI calls but does not bypass validation of new data.

## Real-Time Progress

SignalR sends job updates from the backend to the Angular application.

The main job stages are:

```text
Created
   ↓
Resource Type Detection
   ↓
Field Mapping
   ↓
Data Validation
   ↓
FHIR Transformation
   ↓
Completed
```

This allows users to monitor conversion progress without waiting for a single long-running API request.

## Multi-Hospital Design

The prototype supports multiple hospitals.

Each authenticated user is associated with a hospital, and jobs and configuration are processed within that hospital context.

The current authentication implementation is intended for prototype demonstration rather than production identity management.

## Storage

**SQLite** stores application and processing information such as jobs, hospitals, users, fingerprints, and approved mappings.

**Local file storage** is used for uploaded source files and generated FHIR JSON. Uploaded source files are removed after successful processing, while generated output remains available for download.

## Technology Stack

| Layer | Technology |
| --- | --- |
| Frontend | Angular / TypeScript |
| Backend | .NET 10 / C# |
| API | ASP.NET Core Web API |
| Database | SQLite / Entity Framework Core |
| FHIR | Firely .NET SDK / FHIR R4 |
| Real-Time Updates | SignalR |
| AI Integration | OpenAI API |
| File Storage | Local File Storage |

## Current Scope

The current prototype implements:

```text
CSV
 ↓
AI-Assisted Resource Detection
 ↓
Human Approval
 ↓
AI-Assisted Field Mapping
 ↓
Human Approval
 ↓
Normalization & Validation
 ↓
FHIR R4
 ↓
FHIR JSON
```

Currently supported FHIR resources are Patient, Observation, and Encounter.

The architecture can be extended in the future to support additional source formats, interoperability standards, and FHIR resource types.