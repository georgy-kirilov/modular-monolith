import { Routes } from '@angular/router';
import { LoginComponent } from './accounts/login/login.component';
import { RegisterComponent } from './accounts/register/register.component';
import { MeComponent } from './accounts/me/me.component';
import { authGuard, loginGuard, registerGuard } from './auth/auth.guards';
import { DashboardComponent } from './dashboard/dashboard.component';
import { EmailConfirmationComponent } from './accounts/email-confirmation/email-confirmation.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'accounts/email-confirmation', component: EmailConfirmationComponent },
  { path: 'accounts/login', component: LoginComponent, canActivate: [loginGuard] },
  { path: 'accounts/register', component: RegisterComponent, canActivate: [registerGuard] },
  { path: 'accounts/me', component: MeComponent, canActivate: [authGuard] }, 
];
