import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { BranchService } from '../branch/branch.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('/api/auth/login')) {
    return next(req);
  }

  const auth = inject(AuthService);
  const branch = inject(BranchService);

  let headers = req.headers;
  const token = auth.getAccessToken();

  if (token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  const branchId = branch.selectedBranchId();
  if (branchId) {
    headers = headers.set('X-Branch-Id', branchId);
  }

  return next(req.clone({ headers }));
};
