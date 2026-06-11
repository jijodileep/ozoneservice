import { Component, HostListener, OnInit, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/auth/auth.service';
import { BranchService } from '../core/branch/branch.service';
import { NotificationItem, NotificationService } from '../core/notification/notification.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
})
export class ShellComponent implements OnInit {
  readonly auth = inject(AuthService);
  readonly branch = inject(BranchService);
  private readonly notifications = inject(NotificationService);

  readonly notificationItems = signal<NotificationItem[]>([]);
  readonly notificationsOpen = signal(false);

  ngOnInit(): void {
    if (!this.auth.profile()) {
      this.auth.loadProfile().subscribe();
    }
    if (!this.auth.isPlatformAdmin() && !this.branch.branches().length) {
      this.branch.loadBranches().subscribe();
    }
    this.loadNotifications();
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.notificationsOpen.set(false);
  }

  toggleNotifications(event: Event): void {
    event.stopPropagation();
    this.notificationsOpen.update((open) => !open);
  }

  closeNotifications(event?: Event): void {
    event?.stopPropagation();
    this.notificationsOpen.set(false);
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
