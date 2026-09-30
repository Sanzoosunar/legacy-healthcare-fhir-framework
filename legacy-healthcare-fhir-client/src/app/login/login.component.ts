import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from './auth.service';
import { loginResponseDto } from './login-dto';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  username = '';
  password = '';
  loading = false;
  errorMessage = '';

  login(): void {
    if (this.loading || !this.username.trim() || !this.password) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.auth.login(this.username.trim(), this.password)
      .subscribe(response => {
        this.loading = false;
        if (response.success) {
          this.clearData();
          this.router.navigateByUrl('/');
          return;
        }

        this.errorMessage = response.errorMessage!
      });
  }

  private clearData() {
    this.username = '';
    this.password = '';
  }
}