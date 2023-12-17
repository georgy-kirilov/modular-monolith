export class LoginRequest {
  email: string = '';
  password: string = '';
  storeJwtInCookie: boolean = true;
}

export class LoginResponse {
  lifetimeInSeconds: number = 0;
}
