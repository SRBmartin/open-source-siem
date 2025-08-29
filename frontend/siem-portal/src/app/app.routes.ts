import { Routes } from '@angular/router';
import { userRoutes } from './modules/user/user.routes';
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        canActivateChild: [authGuard],
        children: [
            {
                path: 'panel',
                loadComponent: () => import('./modules/user/features/panel/panel.component').then(m => m.UserPanelComponent)
            }
        ]
    },
    {
        path: 'user',
        children: userRoutes
    },
    { path: '**', redirectTo: '/user/login' }
];
