import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./register/register.component').then(m => m.RegisterComponent)
  },
  
  { path: '', redirectTo: 'login', pathMatch: 'full' } // from /user to /user/login (maybe it's good to have so we will put it here)
];