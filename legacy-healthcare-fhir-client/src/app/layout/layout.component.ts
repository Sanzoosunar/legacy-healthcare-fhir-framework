import { Component, inject } from '@angular/core';
import {
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';
import { AuthService } from '../login/auth.service';
import { CurrentUserDto } from '../models/user';
import { takeUntil } from 'rxjs';
import { DestroyComponent } from '../destroy.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent extends DestroyComponent {
  private auth = inject(AuthService);
  public currentUser: CurrentUserDto = {
    hospitalName: '', username: ''
  }

  ngOnInit() {
    this.auth.getCurrentUser().pipe(takeUntil(this.destroy))
      .subscribe(user => this.currentUser = user)
  }

  logout(): void {
    this.auth.logout();
  }
}