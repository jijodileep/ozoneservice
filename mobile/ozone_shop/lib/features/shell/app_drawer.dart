import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/auth/auth_controller.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/ozone_logo.dart';

class AppDrawer extends ConsumerWidget {
  const AppDrawer({super.key});

  static const _items = [
    _DrawerItem('/home', Icons.home_outlined, Icons.home, 'Home'),
    _DrawerItem('/jobs', Icons.assignment_outlined, Icons.assignment, 'Jobs'),
    _DrawerItem('/customers', Icons.people_outline, Icons.people, 'Customers'),
    _DrawerItem('/account', Icons.person_outline, Icons.person, 'Account'),
  ];

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final session = ref.watch(authControllerProvider).value;
    final location = GoRouterState.of(context).uri.path;
    final textTheme = Theme.of(context).textTheme;

    return Drawer(
      backgroundColor: Colors.white,
      child: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            DrawerHeader(
              margin: EdgeInsets.zero,
              padding: EdgeInsets.zero,
              decoration: const BoxDecoration(gradient: AppTheme.brandGradient),
              child: Padding(
                padding: const EdgeInsets.fromLTRB(20, 20, 20, 16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const OzoneLogo(size: 48),
                    const Spacer(),
                    Text(
                      session?.profile.displayName ?? 'Ozone Shop',
                      style: textTheme.titleMedium?.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      session?.profile.email ?? '',
                      style: textTheme.bodySmall?.copyWith(
                        color: Colors.white.withValues(alpha: 0.82),
                      ),
                    ),
                    if (session != null) ...[
                      const SizedBox(height: 10),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: Colors.white.withValues(alpha: 0.15),
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Text(
                          session.branch.name,
                          style: textTheme.labelSmall?.copyWith(color: Colors.white),
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ),
            Expanded(
              child: ListView(
                padding: const EdgeInsets.symmetric(vertical: 8),
                children: _items.map((item) {
                  final selected = location.startsWith(item.path);
                  return ListTile(
                    leading: Icon(
                      selected ? item.selectedIcon : item.icon,
                      color: selected ? AppColors.primary : AppColors.textMuted,
                    ),
                    title: Text(
                      item.label,
                      style: textTheme.bodyLarge?.copyWith(
                        fontWeight: selected ? FontWeight.w600 : FontWeight.w500,
                        color: selected ? AppColors.primary : AppColors.text,
                      ),
                    ),
                    selected: selected,
                    selectedTileColor: AppColors.primary.withValues(alpha: 0.08),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                    contentPadding: const EdgeInsets.symmetric(horizontal: 20, vertical: 2),
                    onTap: () {
                      Navigator.of(context).pop();
                      if (!selected) {
                        context.go(item.path);
                      }
                    },
                  );
                }).toList(),
              ),
            ),
            const Divider(height: 1, color: AppColors.border),
            ListTile(
              leading: const Icon(Icons.logout, color: Color(0xFFDC2626)),
              title: Text(
                'Sign out',
                style: textTheme.bodyLarge?.copyWith(
                  color: const Color(0xFFDC2626),
                  fontWeight: FontWeight.w500,
                ),
              ),
              onTap: () {
                Navigator.of(context).pop();
                ref.read(authControllerProvider.notifier).logout();
              },
            ),
            const SizedBox(height: 8),
          ],
        ),
      ),
    );
  }
}

class _DrawerItem {
  const _DrawerItem(this.path, this.icon, this.selectedIcon, this.label);

  final String path;
  final IconData icon;
  final IconData selectedIcon;
  final String label;
}
