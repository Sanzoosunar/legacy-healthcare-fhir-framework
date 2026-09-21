import { Component } from '@angular/core';
import { JobProgressComponent } from '../job-progress/job-progress.component';
import { JobBridgeService } from '../services/job-bridge.service';
import { JobDataService } from '../services/job-data.service';
import { JobStage, JobStatus } from '../enums/job-enums';

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
    this.jobBridgeService.setJobId("45455");
  }


  public onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0];
    alert('file selected')
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
    // debugger;
    // if (!this.selectedFile) {
    //   return;
    // }

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