import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

import { CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = () => {
    if (inject(AuthService).isLoggedIn()) {
        return true;
    }
    inject(Router).navigateByUrl('accounts/login');
    return false;
};

export const loginPageGuard: CanActivateFn = () => {
    if (!inject(AuthService).isLoggedIn()) {
        return true;
    }
    inject(Router).navigateByUrl('/');
    return false;
};

export const registerPageGuard: CanActivateFn = () => {
    if (!inject(AuthService).isLoggedIn()) {
        return true;
    }
    inject(Router).navigateByUrl('/');
    return false;
};
