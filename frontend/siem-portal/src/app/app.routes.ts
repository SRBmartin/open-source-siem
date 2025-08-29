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
            },
            {
                path: 'tags',
                loadComponent: () => import('./modules/tags/features/TagsPage/tags.component').then(m => m.TagsComponent)
            }
        ]
    },
    {
    path: 'email-verify/:userId/:token',
        loadComponent: () => import('./modules/user/features/email-veriify/email-verify.component').then(m => m.EmailVerifyComponent)
    },
    {
        path: 'user',
        children: userRoutes
    },
    { path: '**', redirectTo: '/user/login' }
];
