import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../core/auth/auth.service';
import { BranchService } from '../core/branch/branch.service';
import { NotificationItem, NotificationService } from '../core/notification/notification.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgbDropdownModule],
  templateUrl: './shell.component.html',
})
export class ShellComponent implements OnInit {
  readonly auth = inject(AuthService);
  readonly branch = inject(BranchService);
  private readonly notifications = inject(NotificationService);

  readonly notificationItems = signal<NotificationItem[]>([]);

  ngOnInit(): void {
    if (!this.auth.profile()) {
      this.auth.loadProfile().subscribe();
    }
    if (!this.auth.isPlatformAdmin() && !this.branch.branches().length) {
      this.branch.loadBranches().subscribe();
    }
    this.loadNotifications();
  }

  userInitials(): string {
    const name = this.auth.profile()?.displayName ?? '?';
    return name
      .split(' ')
      .map((w) => w[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }

  loadNotifications(): void {
    this.notifications.getNotifications().subscribe({
      next: (items) => this.notificationItems.set(items),
    });
  }

  onBranchChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.branch.selectBranch(select.value);
  }

  logout(): void {
    this.branch.clear();
    this.auth.logout();
  }
}
