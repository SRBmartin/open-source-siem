import { Routes } from '@angular/router';
import { RouteNames } from '../../shared/consts/routes';
import { authGuard } from '../../shared/guards/auth.guard';

export const userRoutes: Routes = [
  {
    path: RouteNames.LoginRoute,
    loadComponent: () =>
      import('./features/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: RouteNames.SettingsRoute,
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/settings/settings.component').then(m => m.SettingsComponent)
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' } // from /user to /user/login (maybe it's good to have so we will put it here)
];