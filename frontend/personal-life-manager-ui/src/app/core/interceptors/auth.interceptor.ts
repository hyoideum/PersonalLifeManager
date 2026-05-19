import { HttpInterceptorFn } from '@angular/common/http';
import { inject, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const injector = inject(Injector);
  const router = inject(Router);

  const token = localStorage.getItem('token');

  let clonedReq = req;

  if (req.url.includes('/refresh')) {
    return next(req);
  }

  if (token) {
    clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(clonedReq).pipe(
    catchError((error) => {

      if (error.status === 401 && token) {

        const authService = injector.get(AuthService);

        return authService.refreshToken().pipe(
          switchMap(() => {

            const newToken = localStorage.getItem('token');

            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newToken}`
              }
            });

            return next(retryReq);
          }),
          catchError(() => {

            authService.logout();
            router.navigate(['/auth/login']);

            return throwError(() => error);
          })
        );
      }

      return throwError(() => error);
    })
  );
};