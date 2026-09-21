import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { backendUrl } from '../environment';
import { JobNotificationEvent } from '../models/job';

@Injectable({
  providedIn: 'root'
})
export class SignalRConnectionService {
  private hubConnection: signalR.HubConnection;
  private jobUpdatedSubject = new Subject<JobNotificationEvent>();

  public jobUpdated$ = this.jobUpdatedSubject.asObservable();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${backendUrl}hubs/import-job`)
      .withAutomaticReconnect()
      .build();

    this.registerListeners();
    this.startConnection();
  }

  private startConnection(): void {
    this.hubConnection.start()
      .then(() => {
        console.log('SignalR connected successfully');
      })
      .catch(error => {
        console.error('SignalR connection failed:', error);
      });
  }

  private registerListeners(): void {
    this.hubConnection.on('JobUpdated', (event: JobNotificationEvent) => {
      this.jobUpdatedSubject.next(event);
    });
  }
}