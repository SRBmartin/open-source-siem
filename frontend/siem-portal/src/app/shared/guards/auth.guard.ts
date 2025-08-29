import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../modules/user/services/auth.service';
import { RouteNames } from '../../shared/consts/routes';

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) return true;

  return router.createUrlTree(
    ['/user', RouteNames.LoginRoute],
    { queryParams: { returnUrl: state.url } }
  );
};
