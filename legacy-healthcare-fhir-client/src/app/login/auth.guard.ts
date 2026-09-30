import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

// Protect the dashboard.
export const authGuard: CanActivateFn = () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    return auth.isLoggedIn
        ? true
        : router.createUrlTree(['/login']);
};

// Skip login when a token is already stored.
export const loginGuard: CanActivateFn = () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    return auth.isLoggedIn
        ? router.createUrlTree(['/'])
        : true;
};