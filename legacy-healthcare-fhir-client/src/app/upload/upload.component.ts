import { Component } from '@angular/core';
import { JobProgressComponent } from '../job-progress/job-progress.component';
import { JobBridgeService } from '../services/job-bridge.service';
import { JobDataService } from '../services/job-data.service';
import { JobStage, JobStatus } from '../enums/job-enums';
import { FhirResourceType } from '../enums/resource-type';

@Component({
  selector: 'app-upload',
  imports: [JobProgressComponent],
  templateUrl: './upload.component.html',
  styleUrl: './upload.component.css',
  providers: [JobBridgeService]
})
export class UploadComponent {
  public selectedStandard = 'FHIR';
  public standardErrorMessage?: string;
  public isConvertBtnEnabled = true;
  public selectedFile?: File;

  constructor(
    private jobDataService: JobDataService,
    public jobBridgeService: JobBridgeService
  ) {

    this.isConvertBtnEnabled = false

    this.jobBridgeService.setJobId('122');


    this.jobBridgeService.setJobStage(JobStage.ResourceTypeDetection)
    this.jobBridgeService.setJobStatus(JobStatus.AiSuggested)
    this.jobBridgeService.setResourceTypeDetection({
      resourceType: FhirResourceType.Patient,
      aiConfidence: 0.99,
      isApproved: false
    })


    // this.jobBridgeService.setJobStage(JobStage.FieldMapping)
    // this.jobBridgeService.setJobStatus(JobStatus.AiSuggested)
    // this.jobBridgeService.setFieldMapping({
    //   configurationId: 1,
    //   isApproved: false,
    //   mappings: [
    //     {
    //       mappingId: 1,
    //       sourceField: 'patient_id',
    //       normalizedField: 'PatientId',
    //       aiConfidence: 0.9812,
    //       aiExplanation: 'Matched patient identifier field'
    //     },
    //     {
    //       mappingId: 2,
    //       sourceField: 'first_name',
    //       normalizedField: 'FirstName',
    //       aiConfidence: 0.9541,
    //       aiExplanation: 'Matched patient first name'
    //     },
    //     {
    //       mappingId: 3,
    //       sourceField: 'dob',
    //       normalizedField: 'DateOfBirth',
    //       aiConfidence: 0.9012,
    //       aiExplanation: 'Matched patient date of birth'
    //     }
    //   ]
    // })



    // this.jobBridgeService.setJobStage(JobStage.Completed)
    // this.jobBridgeService.setJobStatus(JobStatus.Completed)

    // const errorMsg = [
    //   "Invalid Date of birth in Row 5",
    //   "Patient Id is missing in Row 10",
    //   "First name is missing in Row 13"
    // ]
    // this.jobBridgeService.setErrorMessages(errorMsg)

    // this.jobBridgeService.da
    // this.jobBridgeService.setResourceTypeDetection({
    //   resourceType: FhirResourceType.Patient,
    //   aiConfidence: 0.99,
    //   isApproved: false
    // })

  }


  public onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0];
  }

  onStandardChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.selectedStandard = select.value;

    if (this.selectedStandard !== 'FHIR') {
      this.standardErrorMessage = `${this.selectedStandard} conversion is in progress.`;
      this.isConvertBtnEnabled = false;
      return;
    }

    this.standardErrorMessage = undefined;
    this.isConvertBtnEnabled = true;
  }

  public onConvertBtnClick(): void {
    this.isConvertBtnEnabled = false;

    this.jobDataService.upload(this.selectedFile!).subscribe({
      next: response => {
        this.jobBridgeService.setJobId(response.jobId);
        this.jobBridgeService.setJobStage(JobStage.ResourceTypeDetection)
        this.jobBridgeService.setJobStatus(JobStatus.InProgress)
      },
      error: () => {
        this.isConvertBtnEnabled = true;
      }
    });
  }

}