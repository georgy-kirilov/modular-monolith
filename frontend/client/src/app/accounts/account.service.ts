import { HttpClient } from '@angular/common/http';
import { Injectable, Signal, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);

  getMineUserInfo(): Signal<MineUserInfoResponse | undefined> {
    return toSignal(this.http.get<MineUserInfoResponse>('api/accounts/me/info'));
  }

  confirmEmail(userId: string, token: string): Observable<any> {
    return this.http.post('api/accounts/confirm-email', {
      userId: userId,
      token: token
    });
  }
}

export interface MineUserInfoResponse {
  username: string;
  email: string;
}
