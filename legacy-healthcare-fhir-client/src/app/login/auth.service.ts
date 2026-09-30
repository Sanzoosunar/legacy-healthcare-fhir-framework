import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, filter, Observable, of, shareReplay, tap } from 'rxjs';
import { loginResponseDto } from './login-dto';
import { backendUrl } from '../environment';
import { CurrentUserDto } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly baseUrl = `${backendUrl}/api/v1/auth`;

  private accessTokenKeyName = 'fhir-accessToken'
  private currentUserKeyName = 'fhir-currentUserInfo-key'

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
    localStorage.removeItem(this.currentUserKeyName);
    void this.router.navigateByUrl('/login');
  }

  getCurrentUser(): Observable<CurrentUserDto> {
    const cachedUser = localStorage.getItem(this.currentUserKeyName);

    if (cachedUser) {
      try {
        return of(JSON.parse(cachedUser) as CurrentUserDto);
      } catch {
        localStorage.removeItem(this.currentUserKeyName);
      }
    }
    return this.http.get<CurrentUserDto>(`${this.baseUrl}/me`).pipe(
      tap(user => {
        localStorage.setItem(this.currentUserKeyName, JSON.stringify(user));
      })
    );
  }

}