import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../account.service';
import { ActivatedRoute, RouterModule, RouterOutlet } from '@angular/router';
import { ErrorsModel } from '../../validation/errors-model';
import { ValidationComponent } from '../../validation/validation/validation.component';

@Component({
  selector: 'app-email-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule, ValidationComponent],
  templateUrl: './email-confirmation.component.html'
})
export class EmailConfirmationComponent {

  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);

  errors = new ErrorsModel;
  emailSuccessfullyConfirmed: boolean = false;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params: any) => {
      this.accountService.confirmEmail(params.userId, params.token).subscribe({
        next: _ => this.emailSuccessfullyConfirmed = true,
        error: err => this.errors.set(err)
      });
    });
  }
}
