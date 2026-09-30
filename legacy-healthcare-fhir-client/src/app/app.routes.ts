import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { UploadComponent } from './upload/upload.component';
import { LoginComponent } from './login/login.component';
import { authGuard, loginGuard } from './login/auth.guard';
import { LayoutComponent } from './layout/layout.component';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent,
        canActivate: [loginGuard]
    },

    {
        path: '',
        component: LayoutComponent,
        canActivate: [authGuard],
        canActivateChild: [authGuard],
        children: [
            {
                path: '',
                pathMatch: 'full',
                component: DashboardComponent
            },
            {
                path: 'upload',
                component: UploadComponent
            },
            //   {
            //     path: 'jobs',
            //     component: JobsComponent
            //   }
        ]
    },
    {
        path: '**',
        redirectTo: ''
    }
];