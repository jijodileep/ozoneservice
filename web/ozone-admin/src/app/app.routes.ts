import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { InvoicesComponent } from './features/invoices/invoices.component';
import { LoginComponent } from './features/login/login.component';
import { PlatformDashboardComponent } from './features/platform/platform-dashboard.component';
import { PlatformPlansComponent } from './features/platform/platform-plans.component';
import { SubscriptionComponent } from './features/subscription/subscription.component';
import { ShellComponent } from './layout/shell.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    canActivate: [authGuard],
    component: ShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        canActivate: [roleGuard(['TenantAdmin', 'ShopAdmin', 'Accountant'])],
        component: DashboardComponent,
      },
      {
        path: 'invoices',
        canActivate: [roleGuard(['TenantAdmin', 'ShopAdmin', 'Accountant'])],
        component: InvoicesComponent,
      },
      {
        path: 'subscription',
        canActivate: [roleGuard(['TenantAdmin', 'ShopAdmin'])],
        component: SubscriptionComponent,
      },
      {
        path: 'platform',
        canActivate: [roleGuard(['PlatformSuperAdmin'])],
        component: PlatformDashboardComponent,
      },
      {
        path: 'platform/plans',
        canActivate: [roleGuard(['PlatformSuperAdmin'])],
        component: PlatformPlansComponent,
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
