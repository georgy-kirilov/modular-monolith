export class RegisterRequest {
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
}

export class RegisterResponse {
  emailConfirmationThresholdInSeconds: number = 0;
}
