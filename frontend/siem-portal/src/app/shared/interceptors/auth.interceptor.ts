import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { JwtStorageService } from '../services/jwt-storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const jwt = inject(JwtStorageService);
  const token = jwt.getAccessToken();

  if (token && !jwt.isTokenExpired()) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req);
};
