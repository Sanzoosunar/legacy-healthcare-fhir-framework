import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { SignalRConnectionService } from './services/signal-r-connection.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'legacy-healthcare-fhir-client';

  constructor(private signalRConnectionService: SignalRConnectionService) {
  }
}
