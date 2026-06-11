import 'package:flutter/material.dart';

import '../theme/app_theme.dart';
import '../../features/shell/shell_scaffold_key.dart';

class AppPageScaffold extends StatelessWidget {
  const AppPageScaffold({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.floatingActionButton,
  });

  final String title;
  final Widget body;
  final List<Widget>? actions;
  final Widget? floatingActionButton;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.surface,
      appBar: AppBar(
        leading: IconButton(
          tooltip: 'Menu',
          icon: const Icon(Icons.menu),
          onPressed: openAppDrawer,
        ),
        title: Text(title),
        actions: actions,
      ),
      body: body,
      floatingActionButton: floatingActionButton,
    );
  }
}

class DrawerMenuButton extends StatelessWidget {
  const DrawerMenuButton({
    super.key,
    this.color = AppColors.text,
    this.onDark = false,
  });

  final Color color;
  final bool onDark;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: onDark ? Colors.white.withValues(alpha: 0.12) : AppColors.surface,
      borderRadius: BorderRadius.circular(12),
      child: InkWell(
        onTap: openAppDrawer,
        borderRadius: BorderRadius.circular(12),
        child: SizedBox(
          width: 40,
          height: 40,
          child: Icon(
            Icons.menu,
            size: 22,
            color: onDark ? Colors.white : color,
          ),
        ),
      ),
    );
  }
}
