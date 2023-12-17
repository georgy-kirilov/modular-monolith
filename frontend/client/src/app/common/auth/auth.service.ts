import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly EXPIRATION_TIMESTAMP = 'expiration_timestamp';
  private router = inject(Router);
  isLoggedIn = signal(this.checkIfLoggedIn());

  login(sessionLifetimeInSeconds: number): void {
    const currentTimestamp = Date.now();
    const expirationTimestamp = sessionLifetimeInSeconds * 1000 + currentTimestamp;
    localStorage.setItem(this.EXPIRATION_TIMESTAMP, expirationTimestamp.toString());
    this.isLoggedIn.set(true);
    this.router.navigateByUrl('/');
  }

  logout(): void {
    localStorage.removeItem(this.EXPIRATION_TIMESTAMP);
    this.isLoggedIn.set(false);
    this.router.navigateByUrl('/');
  }

  private checkIfLoggedIn(): boolean {
    const currentTimestamp = Date.now();
    const expirationTimestampAsText = localStorage.getItem(this.EXPIRATION_TIMESTAMP) ?? '0';
    return parseInt(expirationTimestampAsText) > currentTimestamp;
  }
}
