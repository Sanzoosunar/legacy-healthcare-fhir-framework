# Prototype Testing

## Overview

The prototype is manually tested using sample legacy healthcare CSV files.

Testing focuses on verifying the end-to-end transformation workflow, including:

- Multi-hospital authentication
- Resource type detection
- Field mapping
- Human review and approval
- Fingerprint and approved configuration reuse
- Data normalization and validation
- FHIR transformation
- Background job progress
- FHIR JSON download
- Source-file cleanup

Automated unit testing is not included in the current prototype scope.

---

## Test Environment

| Component | Environment |
| --- | --- |
| Backend | .NET 10 / ASP.NET Core |
| Frontend | Angular / TypeScript |
| Database | SQLite |
| Source Format | CSV |
| Target Format | FHIR R4 JSON |
| Supported Resources | Patient, Observation, Encounter |

The application automatically seeds three demonstration hospitals and three demo users for testing. See the [README](../README.md#demo-accounts) for login credentials.
---

## Test 1 - Patient Conversion

### Input

A CSV file containing 10 sample Patient records.

Example fields:

```text
patient_id
first_name
last_name
date_of_birth
gender
```

### Steps

1. Log in using a demo account.
2. Navigate to the Upload page.
3. Upload the Patient CSV file.
4. Select FHIR as the target format.
5. Start the conversion.
6. Review the detected resource type.
7. Approve the resource type.
8. Review the suggested field mappings.
9. Approve the field mappings.
10. Allow the job to complete.
11. Download the generated output.

### Expected Result

- Patient is detected as the FHIR resource type.
- Field mappings are generated.
- User can review and approve the suggestions.
- Patient data is normalized and validated.
- FHIR Patient resources are generated.
- Job reaches Completed status.
- Generated JSON is available for download.

### Result

**Passed**

---

## Test 2 - Observation Conversion

### Input

A CSV file containing 10 sample Observation records.

Example fields:

```text
observation_id
status
category_code
category_display
code
code_display
code_system
patient_id
encounter_id
effective_date_time
value
unit
unit_code
unit_system
reference_range_low
reference_range_high
```

### Expected Result

- Observation is detected as the resource type.
- Observation fields are mapped to normalized fields.
- Data passes normalization and validation.
- FHIR Observation resources are generated.
- Generated JSON is available for download.

### Result

**Passed**

---

## Test 3 - Encounter Conversion

### Input

A CSV file containing 10 sample Encounter records.

Example fields:

```text
encounter_id
patient_id
status
class
type_code
type_display
start_date_time
end_date_time
practitioner_id
location_id
reason_code
reason_display
```

### Expected Result

- Encounter is detected as the resource type.
- Encounter fields are mapped to normalized fields.
- Data passes normalization and validation.
- FHIR Encounter resources are generated.
- Generated JSON is available for download.

### Result

**Passed**

---

## Test 4 - Resource Type AI Detection

### Scenario

Upload a source schema that does not have a previously approved resource fingerprint.

### Expected Result

1. Resource Type Detection starts.
2. No approved fingerprint is found.
3. AI is called to determine the likely FHIR resource type.
4. Job status changes to AI Suggested.
5. The suggestion is displayed to the user.
6. User can approve or change the resource type.
7. Processing continues after approval.

### Result

**Passed**

---

## Test 5 - Resource Type Fingerprint Reuse

### Scenario

Upload a schema that has already gone through resource type detection and approval.

### Expected Result

1. The system generates the source fingerprint.
2. The fingerprint matches a previously approved configuration.
3. The approved resource type is reused.
4. An unnecessary AI resource detection call is avoided.
5. Processing continues using the approved configuration.

### Result

**Passed**

---

## Test 6 - AI Field Mapping

### Scenario

Process legacy fields for which approved mappings do not already exist.

Example:

```text
F_NAME
L_NAME
DOB
SEX
```

### Expected Result

AI suggests mappings such as:

```text
F_NAME → FirstName
L_NAME → LastName
DOB    → DateOfBirth
SEX    → Gender
```

The suggested mappings are displayed to the user.

The user can:

- Accept a suggested mapping.
- Change a suggested mapping.
- Approve the mappings.

### Result

**Passed**

---

## Test 7 - Field Mapping Reuse

### Scenario

Process fields that have previously approved mappings.

### Expected Result

1. Existing field fingerprints are recognized.
2. Previously approved mappings are retrieved.
3. Existing mappings are reused.
4. Unnecessary AI calls are avoided.
5. Processing continues using the approved mappings.

### Result

**Passed**

---

## Test 8 - Human Correction

### Scenario

Change an AI-suggested resource type or field mapping before approval.

### Expected Result

- User can modify the AI suggestion.
- The corrected value is approved.
- The approved value is used by the processing pipeline.
- The approved configuration can be reused in future processing.

### Result

**Passed**

---

## Test 9 - Data Validation Success

### Scenario

Upload legacy data containing valid values for the selected resource type.

### Expected Result

1. Legacy records are converted into the normalized representation.
2. Validation completes successfully.
3. The job continues to FHIR Transformation.
4. FHIR resources are generated.

### Result

**Passed**

---

## Test 10 - Data Validation Failure

### Scenario

Upload legacy data containing invalid data.

### Expected Result

1. Data is normalized.
2. Validation detects the invalid data.
3. Job status changes to Failed.
4. Validation errors are returned to the user.
5. FHIR Transformation does not continue.

### Result

**Passed**

---

## Test 11 - Background Job Progress

### Scenario

Start a new conversion job and monitor the Upload page.

### Expected Result

The user receives progress updates as the job moves through the processing stages:

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

The UI displays the corresponding job status, including:

```text
Started
InProgress
AI Suggested
Completed
Failed
```

### Result

**Passed**

---

## Test 12 - FHIR Output Download

### Scenario

Complete a successful transformation job.

### Expected Result

- FHIR JSON output is generated.
- Download becomes available after successful completion.
- Output can be downloaded from:
  - Dashboard recent jobs
  - Upload page
  - Job list page

### Result

**Passed**

---

## Test 13 - Source File Cleanup

### Scenario

Complete a successful transformation job.

### Expected Result

- Temporary uploaded legacy source file is removed.
- Generated FHIR JSON output remains available.
- Job remains available in job history.

### Result

**Passed**

---

## Test 14 - Multi-Hospital Login

### Scenario

Log in using different seeded demo users.

### Demo Accounts

```text
demo1 / demo1
demo2 / demo2
demo3 / demo3
```

### Expected Result

- Each valid user can log in.
- The authenticated username is displayed.
- The associated hospital is displayed.
- User can log out and return to the login page.

### Result

**Passed**

---

## Test 15 - Dashboard and Job History

### Scenario

Create and process transformation jobs.

### Expected Result

The dashboard displays:

- Aggregated job information
- Recent jobs
- Job stage
- Job status
- Resource type
- Creation time
- Download option for completed jobs

The Job page displays the available transformation jobs and provides download access for completed jobs.

### Result

**Passed**

---

## Test Summary

The manual prototype tests verify the primary end-to-end workflow:

```text
Legacy CSV
    ↓
Resource Detection
    ↓
Human Approval
    ↓
Field Mapping
    ↓
Human Approval
    ↓
Normalization
    ↓
Validation
    ↓
FHIR R4 Transformation
    ↓
FHIR JSON
    ↓
Download
```

The tests also verify reuse of previously approved resource and field mappings, background job progress, multi-hospital login, output download, and temporary source-file cleanup.

These tests are intended to validate the functionality of the prototype and do not represent production-level verification, clinical validation, security testing, performance testing, or FHIR conformance certification.