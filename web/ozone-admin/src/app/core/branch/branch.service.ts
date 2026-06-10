import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BranchSummary } from './branch.models';

const BRANCH_ID_KEY = 'ozone_branch_id';

@Injectable({ providedIn: 'root' })
export class BranchService {
  private readonly http = inject(HttpClient);

  readonly branches = signal<BranchSummary[]>([]);
  readonly selectedBranchId = signal<string | null>(this.readStoredBranchId());

  loadBranches(): Observable<BranchSummary[]> {
    return this.http
      .get<BranchSummary[]>(`${environment.apiUrl}/api/tenancy/branches`)
      .pipe(
        tap((branches) => {
          this.branches.set(branches);
          this.ensureSelectedBranch(branches);
        }),
      );
  }

  selectBranch(branchId: string): void {
    this.selectedBranchId.set(branchId);
    sessionStorage.setItem(BRANCH_ID_KEY, branchId);
  }

  clear(): void {
    this.branches.set([]);
    this.selectedBranchId.set(null);
    sessionStorage.removeItem(BRANCH_ID_KEY);
  }

  private ensureSelectedBranch(branches: BranchSummary[]): void {
    if (!branches.length) {
      this.selectedBranchId.set(null);
      sessionStorage.removeItem(BRANCH_ID_KEY);
      return;
    }

    const stored = this.readStoredBranchId();
    const valid = stored && branches.some((b) => b.id === stored);
    const branchId = valid ? stored! : branches[0].id;
    this.selectBranch(branchId);
  }

  private readStoredBranchId(): string | null {
    return sessionStorage.getItem(BRANCH_ID_KEY);
  }
}
