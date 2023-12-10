import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

import { CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    if (authService.isLoggedIn()) return true;
    router.navigateByUrl('accounts/login');
    return false;
};

export const loginGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    if (!authService.isLoggedIn()) return true;
    router.navigateByUrl('/');
    return false;
};

export const registerGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);
    if (!authService.isLoggedIn()) return true;
    router.navigateByUrl('/');
    return false;
};
