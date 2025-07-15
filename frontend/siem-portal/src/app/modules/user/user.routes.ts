import { Routes } from '@angular/router';
import { RouteNames } from '../../shared/consts/routes';

export const userRoutes: Routes = [
  {
    path: RouteNames.LoginRoute,
    loadComponent: () =>
      import('./features/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: RouteNames.RegisterRoute,
    loadComponent: () =>
      import('./features/register/register.component').then(m => m.RegisterComponent)
  },
  
  { path: '', redirectTo: 'login', pathMatch: 'full' } // from /user to /user/login (maybe it's good to have so we will put it here)
];