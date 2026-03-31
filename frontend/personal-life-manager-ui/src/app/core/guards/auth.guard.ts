import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = async () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.currentUser();

  if (!auth.isLoggedIn()) {
    return router.createUrlTree(['auth/login']);
  }

  return true;
};