import { Routes } from '@angular/router';
import { userRoutes } from './modules/user/user.routes';

export const routes: Routes = [
    { 
        path: 'user',
        children: userRoutes
    }
];
