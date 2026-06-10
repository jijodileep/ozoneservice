import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { LoginComponent } from './features/login/login.component';
import { PlatformDashboardComponent } from './features/platform/platform-dashboard.component';
import { ShellComponent } from './layout/shell.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: '',
    canActivate: [authGuard],
    component: ShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        canActivate: [
          roleGuard(['TenantAdmin', 'ShopAdmin', 'Accountant']),
        ],
        component: DashboardComponent,
      },
      {
        path: 'platform',
        canActivate: [roleGuard(['PlatformSuperAdmin'])],
        component: PlatformDashboardComponent,
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
