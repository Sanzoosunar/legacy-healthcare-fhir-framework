import { inject } from '@angular/core';
import {
    HttpErrorResponse,
    HttpInterceptorFn
} from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { backendUrl } from '../environment';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
    const auth = inject(AuthService);
    const token = auth.token;

    const apiOrigin = new URL(backendUrl, window.location.origin).origin;
    const url = new URL(request.url, window.location.origin);
    const isApiRequest = url.origin === apiOrigin && url.pathname.startsWith('/api/');
    const isLoginRequest = url.pathname === '/api/auth/login';

    if (!isApiRequest || isLoginRequest || !token) {
        return next(request);
    }

    const authenticatedRequest = request.clone({
        setHeaders: {
            Authorization: `Bearer ${token}`
        }
    });

    return next(authenticatedRequest).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401 && auth.token === token) {
                auth.logout();
            }

            return throwError(() => error);
        })
    );
};