import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/customer/customer_models.dart';
import '../../core/customer/customer_providers.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/app_page_scaffold.dart';
import '../../core/widgets/empty_state.dart';

class CustomerDetailScreen extends ConsumerStatefulWidget {
  const CustomerDetailScreen({super.key, required this.customerId});

  final String customerId;

  @override
  ConsumerState<CustomerDetailScreen> createState() =>
      _CustomerDetailScreenState();
}

class _CustomerDetailScreenState extends ConsumerState<CustomerDetailScreen> {
  CustomerDetail? _customer;
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });

    try {
      final customer =
          await ref.read(customerServiceProvider).getCustomer(widget.customerId);
      if (mounted) {
        setState(() {
          _customer = customer;
          _loading = false;
        });
      }
    } on DioException catch (_) {
      if (mounted) {
        setState(() {
          _error = 'Could not load customer.';
          _loading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final customer = _customer;

    return AppPageScaffold(
      title: 'Customer',
      floatingActionButton: customer == null
          ? null
          : FloatingActionButton(
              onPressed: () =>
                  context.push('/customers/${customer.id}/add-device'),
              backgroundColor: AppColors.primary,
              child: const Icon(Icons.phonelink_setup_outlined),
            ),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(child: Text(_error!))
              : customer == null
                  ? const SizedBox.shrink()
                  : RefreshIndicator(
                      onRefresh: _load,
                      child: ListView(
                        padding: const EdgeInsets.all(20),
                        children: [
                          _CustomerHeader(customer: customer),
                          const SizedBox(height: 24),
                          Text(
                            'Devices',
                            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                                  fontWeight: FontWeight.w600,
                                ),
                          ),
                          const SizedBox(height: 12),
                          if (customer.devices.isEmpty)
                            const EmptyState(
                              icon: Icons.smartphone_outlined,
                              title: 'No devices yet',
                              message:
                                  'Add a phone model for this customer.',
                            )
                          else
                            ...customer.devices.map(
                              (device) => _DeviceCard(device: device),
                            ),
                          const SizedBox(height: 24),
                          Text(
                            'History',
                            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                                  fontWeight: FontWeight.w600,
                                ),
                          ),
                          const SizedBox(height: 12),
                          if (customer.history.isEmpty)
                            const EmptyState(
                              icon: Icons.history,
                              title: 'No service history',
                              message:
                                  'Jobs and invoices will appear here later.',
                            )
                          else
                            ...customer.history.map(
                              (item) => _HistoryTile(item: item),
                            ),
                        ],
                      ),
                    ),
    );
  }
}

class _CustomerHeader extends StatelessWidget {
  const _CustomerHeader({required this.customer});

  final CustomerDetail customer;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            customer.name,
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.w700,
                ),
          ),
          const SizedBox(height: 8),
          _Row(icon: Icons.phone_outlined, text: customer.mobileNumber),
          if (customer.email != null)
            _Row(icon: Icons.email_outlined, text: customer.email!),
          if (customer.address != null)
            _Row(icon: Icons.location_on_outlined, text: customer.address!),
        ],
      ),
    );
  }
}

class _Row extends StatelessWidget {
  const _Row({required this.icon, required this.text});

  final IconData icon;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 4),
      child: Row(
        children: [
          Icon(icon, size: 18, color: AppColors.textMuted),
          const SizedBox(width: 8),
          Expanded(child: Text(text)),
        ],
      ),
    );
  }
}

class _DeviceCard extends StatelessWidget {
  const _DeviceCard({required this.device});

  final CustomerDevice device;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 10),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: const BorderSide(color: AppColors.border),
      ),
      child: ListTile(
        leading: const Icon(Icons.smartphone, color: AppColors.primary),
        title: Text(device.displayLabel),
        subtitle: Text(
          [
            if (device.imei != null) 'IMEI: ${device.imei}',
            'Branch: ${device.registeredAtBranchName}',
          ].join('\n'),
        ),
      ),
    );
  }
}

class _HistoryTile extends StatelessWidget {
  const _HistoryTile({required this.item});

  final CustomerHistoryItem item;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.zero,
      title: Text(item.reference),
      subtitle: Text('${item.type} · ${item.status}'),
      trailing: Text(
        '${item.occurredAt.day}/${item.occurredAt.month}/${item.occurredAt.year}',
        style: Theme.of(context).textTheme.bodySmall,
      ),
    );
  }
}
