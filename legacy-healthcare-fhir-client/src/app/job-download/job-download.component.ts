import { Component, Input, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, EMPTY, finalize, tap } from 'rxjs';
import { JobDataService } from '../services/job-data.service';

@Component({
  selector: 'app-job-download',
  standalone: true,
  templateUrl: './job-download.component.html',
  styleUrl: './job-download.component.css'
})
export class JobDownloadComponent {
  private dataService = inject(JobDataService);
  @Input({ required: true }) jobId!: string;
  @Input() size: 'sm' | 'md' | 'lg' = 'sm';

  isDownloading = false;

  download(): void {
    if (this.isDownloading) return;

    this.isDownloading = true;

    this.dataService.downloadJob(this.jobId).pipe(
      tap((data) => {
        console.log(data);
      }),
      catchError(() => {
        this.toastErrorMessage();
        return EMPTY;
      }),
      finalize(() => this.isDownloading = false)
    ).subscribe(response => {
      const filename = this.getFilename(response.headers.get('Content-Disposition'));
      if (!response.body || !filename) {
        this.toastErrorMessage();
        return;
      }

      const url = URL.createObjectURL(response.body);
      const link = document.createElement('a');

      link.href = url;
      link.download = filename;
      link.click();

      setTimeout(() => URL.revokeObjectURL(url), 1000);
    });
  }

  private toastErrorMessage() {
    alert('Download failed');
  }
  private getFilename(header: string | null): string | null {
    const encoded = header?.match(/filename\*=UTF-8''([^;]+)/i);
    const plain = header?.match(/filename="([^"]+)"|filename=([^;]+)/i);

    try {
      return encoded ? decodeURIComponent(encoded[1]) : plain?.[1] ?? plain?.[2]?.trim() ?? null;
    } catch {
      return null;
    }
  }
}