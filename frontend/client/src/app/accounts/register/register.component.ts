import { Component, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ErrorsModel } from '../../validation/errors-model';
import { ValidationComponent } from '../../validation/validation/validation.component';
import { Subscription, finalize, interval, takeWhile } from 'rxjs';
import { RegisterRequest, RegisterResponse } from './register.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ValidationComponent],
  templateUrl: './register.component.html'
})
export class RegisterComponent implements OnDestroy {

  private httpClient = inject(HttpClient);
  private subscription = new Subscription;
  private emailConfirmationThresholdInSeconds = 0;

  hidePassword = true;
  hideConfirmPassword = true;

  resendEmailConfirmationRemainingSeconds = 0;
  registerSucceeded = false;

  errors = new ErrorsModel;
  input = new RegisterRequest;

  register(): void {
    this.httpClient.post<RegisterResponse>('api/accounts/register', this.input).subscribe({
      next: res => {
        this.registerSucceeded = true;
        this.emailConfirmationThresholdInSeconds = res.emailConfirmationThresholdInSeconds;
        this.startCountdownTimer();
      },
      error: err => this.errors.set(err)
    });
  }

  resendConfirmationEmail(): void {
    this.httpClient.post<any>('api/accounts/send-email-confirmation', {
      email: this.input.email
    }).subscribe({
      next: _ => this.startCountdownTimer(),
      error: err => console.error(err)
    });
  }
 
  private startCountdownTimer(): void {
    this.resendEmailConfirmationRemainingSeconds = this.emailConfirmationThresholdInSeconds;
    this.subscription = interval(1000)
      .pipe(
        takeWhile(() => this.resendEmailConfirmationRemainingSeconds > 0),
        finalize(() => this.resendEmailConfirmationRemainingSeconds = 0))
      .subscribe(() => {
        this.resendEmailConfirmationRemainingSeconds--;
      });
  }

  formatCountdownTimer(): string {
    const durationInSeconds = this.resendEmailConfirmationRemainingSeconds;
    const minutes = Math.floor(durationInSeconds / 60);
    const seconds = durationInSeconds % 60;
    const paddedMinutes = minutes.toString().padStart(2, '0');
    const paddedSeconds = seconds.toString().padStart(2, '0');
    return `${paddedMinutes}:${paddedSeconds}`;
  }

  togglePassword(): void {
    this.hidePassword = !this.hidePassword;
  }

  toggleConfirmPassword(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
