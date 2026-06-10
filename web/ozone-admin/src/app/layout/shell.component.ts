import { Component, OnInit, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../core/auth/auth.service';
import { BranchService } from '../core/branch/branch.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
  ],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
})
export class ShellComponent implements OnInit {
  readonly auth = inject(AuthService);
  readonly branch = inject(BranchService);

  ngOnInit(): void {
    if (!this.auth.profile()) {
      this.auth.loadProfile().subscribe();
    }

    if (!this.auth.isPlatformAdmin() && !this.branch.branches().length) {
      this.branch.loadBranches().subscribe();
    }
  }

  onBranchChange(branchId: string): void {
    this.branch.selectBranch(branchId);
  }

  logout(): void {
    this.branch.clear();
    this.auth.logout();
  }
}
