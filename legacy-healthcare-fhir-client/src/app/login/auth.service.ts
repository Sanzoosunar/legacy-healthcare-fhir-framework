import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, filter, Observable, of, tap } from 'rxjs';
import { loginResponseDto } from './login-dto';
import { backendUrl } from '../environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly baseUrl = `${backendUrl}/api/v1/auth`;

  private accessTokenKeyName = 'accessToken'
  get token(): string | null {
    return localStorage.getItem(this.accessTokenKeyName);
  }

  get isLoggedIn(): boolean {
    return !!this.token;
  }

  login(username: string, password: string): Observable<loginResponseDto> {
    return this.http.post<loginResponseDto>(`${this.baseUrl}/login`, {
      username,
      password
    }).pipe(
      tap(response => {
        response.success = response.success && !!response.token.trim()

        if (!response.success) {
          response.errorMessage = this.getErrorMessage(response.errorMessage)
          return;
        }

        this.setAccessToken(response.token)
      }),
      catchError(() => of<loginResponseDto>({
        success: false,
        token: '',
        errorMessage: this.getErrorMessage()
      }))
    );
  }

  private getErrorMessage(errorMsg?: string): string {
    return errorMsg?.trim() ?? "Unable to login. Please try again."
  }

  private setAccessToken(token: string) {
    localStorage.setItem(this.accessTokenKeyName, token);
  }

  logout(): void {
    localStorage.removeItem(this.accessTokenKeyName);
    void this.router.navigateByUrl('/login');
  }
}