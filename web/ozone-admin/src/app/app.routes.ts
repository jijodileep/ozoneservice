import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { InvoicesComponent } from './features/invoices/invoices.component';
import { LoginComponent } from './features/login/login.component';
import { PlatformDashboardComponent } from './features/platform/platform-dashboard.component';
import { PlatformPlansComponent } from './features/platform/platform-plans.component';
import { PlatformTaxComponent } from './features/platform/platform-tax.component';
import { PlatformUpgradeRequestsComponent } from './features/platform/platform-upgrade-requests.component';
import { BranchesComponent } from './features/branches/branches.component';
import { UsersComponent } from './features/users/users.component';
import { MobileMastersComponent } from './features/mobile-masters/mobile-masters.component';
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
        path: 'branches',
        canActivate: [roleGuard(['TenantAdmin', 'ShopAdmin'])],
        component: BranchesComponent,
      },
      {
        path: 'users',
        canActivate: [roleGuard(['TenantAdmin'])],
        component: UsersComponent,
      },
      {
        path: 'mobile-masters',
        canActivate: [roleGuard(['TenantAdmin', 'ShopAdmin'])],
        component: MobileMastersComponent,
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
      {
        path: 'platform/upgrade-requests',
        canActivate: [roleGuard(['PlatformSuperAdmin'])],
        component: PlatformUpgradeRequestsComponent,
      },
      {
        path: 'platform/tax',
        canActivate: [roleGuard(['PlatformSuperAdmin'])],
        component: PlatformTaxComponent,
      },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
