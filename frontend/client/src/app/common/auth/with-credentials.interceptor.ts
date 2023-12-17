import { HttpRequest, HttpInterceptorFn, HttpHandlerFn } from '@angular/common/http';

export const withCredentialsInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn) => {
  return next(request.clone({
    withCredentials: true
  }));
};
