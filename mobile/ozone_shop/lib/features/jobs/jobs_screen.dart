import 'package:flutter/material.dart';

import '../../core/theme/app_theme.dart';
import '../../core/widgets/app_page_scaffold.dart';
import '../../core/widgets/empty_state.dart';

class JobsScreen extends StatelessWidget {
  const JobsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return AppPageScaffold(
      title: 'Jobs',
      actions: [
        IconButton(
          tooltip: 'Filter',
          onPressed: () {},
          icon: const Icon(Icons.filter_list_outlined),
        ),
      ],
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () {},
        backgroundColor: AppColors.primary,
        icon: const Icon(Icons.add),
        label: const Text('New job'),
      ),
      body: const EmptyState(
        icon: Icons.assignment_outlined,
        title: 'No jobs yet',
        message: 'Repair jobs will appear here once the jobs module is connected to the API.',
        actionLabel: 'Create job',
      ),
    );
  }
}
