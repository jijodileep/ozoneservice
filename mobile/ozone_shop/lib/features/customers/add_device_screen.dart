import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/customer/customer_providers.dart';
import '../../core/mobile_master/mobile_master_models.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/app_page_scaffold.dart';

class AddDeviceScreen extends ConsumerStatefulWidget {
  const AddDeviceScreen({super.key, required this.customerId});

  final String customerId;

  @override
  ConsumerState<AddDeviceScreen> createState() => _AddDeviceScreenState();
}

class _AddDeviceScreenState extends ConsumerState<AddDeviceScreen> {
  final _imeiController = TextEditingController();
  bool _saving = false;
  String? _error;

  List<MobileModelOption> _models = [];
  List<MobileVariantOption> _variants = [];

  MobileBrandOption? _selectedBrand;
  MobileModelOption? _selectedModel;
  MobileVariantOption? _selectedVariant;

  @override
  void dispose() {
    _imeiController.dispose();
    super.dispose();
  }

  Future<List<MobileBrandOption>> _searchBrands(String query) async {
    if (query.trim().length < 2) {
      return const [];
    }

    try {
      return await ref
          .read(mobileMasterServiceProvider)
          .searchActiveBrands(query);
    } catch (_) {
      return const [];
    }
  }

  Future<void> _onBrandSelected(MobileBrandOption brand) async {
    setState(() {
      _selectedBrand = brand;
      _selectedModel = null;
      _selectedVariant = null;
      _models = [];
      _variants = [];
    });

    final models =
        await ref.read(mobileMasterServiceProvider).getActiveModels(brand.id);
    if (mounted) {
      setState(() => _models = models);
    }
  }

  Future<void> _onModelChanged(MobileModelOption? model) async {
    setState(() {
      _selectedModel = model;
      _selectedVariant = null;
      _variants = [];
    });

    if (model == null) {
      return;
    }

    final variants = await ref
        .read(mobileMasterServiceProvider)
        .getActiveVariants(model.id);
    if (mounted) {
      setState(() => _variants = variants);
    }
  }

  Future<void> _submit() async {
    final variant = _selectedVariant;
    if (variant == null) {
      setState(() => _error = 'Select brand, model, and variant.');
      return;
    }

    setState(() {
      _saving = true;
      _error = null;
    });

    try {
      await ref.read(customerServiceProvider).addDevice(
            customerId: widget.customerId,
            variantId: variant.id,
            imei: _imeiController.text.trim().isEmpty
                ? null
                : _imeiController.text.trim(),
          );

      if (!mounted) {
        return;
      }

      context.go('/customers/${widget.customerId}');
    } on DioException catch (error) {
      if (!mounted) {
        return;
      }
      final data = error.response?.data;
      setState(() {
        _error = data is Map && data['message'] is String
            ? data['message'] as String
            : 'Could not add device.';
      });
    } catch (_) {
      if (mounted) {
        setState(() => _error = 'Could not add device.');
      }
    } finally {
      if (mounted) {
        setState(() => _saving = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return AppPageScaffold(
      title: 'Add device',
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Autocomplete<MobileBrandOption>(
              displayStringForOption: (option) => option.name,
              optionsBuilder: (textEditingValue) async {
                return _searchBrands(textEditingValue.text);
              },
              onSelected: _onBrandSelected,
              fieldViewBuilder: (context, controller, focusNode, onFieldSubmitted) {
                return TextField(
                  controller: controller,
                  focusNode: focusNode,
                  decoration: InputDecoration(
                    labelText: 'Brand (type to search)',
                    hintText: 'e.g. Samsung',
                    filled: true,
                    fillColor: Colors.white,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(12),
                      borderSide: const BorderSide(color: AppColors.border),
                    ),
                  ),
                  onChanged: (_) {
                    if (_selectedBrand != null) {
                      setState(() {
                        _selectedBrand = null;
                        _selectedModel = null;
                        _selectedVariant = null;
                        _models = [];
                        _variants = [];
                      });
                    }
                  },
                );
              },
            ),
            const SizedBox(height: 16),
            _Dropdown<MobileModelOption>(
              label: 'Model',
              value: _selectedModel,
              items: _models,
              itemLabel: (item) => item.name,
              onChanged: _onModelChanged,
              enabled: _selectedBrand != null,
            ),
            const SizedBox(height: 16),
            _Dropdown<MobileVariantOption>(
              label: 'Variant',
              value: _selectedVariant,
              items: _variants,
              itemLabel: (item) => item.name,
              onChanged: (value) => setState(() => _selectedVariant = value),
              enabled: _selectedModel != null,
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _imeiController,
              decoration: InputDecoration(
                labelText: 'IMEI (optional)',
                filled: true,
                fillColor: Colors.white,
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(color: AppColors.border),
                ),
              ),
            ),
            if (_error != null) ...[
              const SizedBox(height: 12),
              Text(_error!, style: TextStyle(color: Colors.red.shade700)),
            ],
            const SizedBox(height: 24),
            FilledButton(
              onPressed: _saving ? null : _submit,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                padding: const EdgeInsets.symmetric(vertical: 14),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
              child: _saving
                  ? const SizedBox(
                      width: 22,
                      height: 22,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text('Save device'),
            ),
          ],
        ),
      ),
    );
  }
}

class _Dropdown<T> extends StatelessWidget {
  const _Dropdown({
    required this.label,
    required this.value,
    required this.items,
    required this.itemLabel,
    required this.onChanged,
    this.enabled = true,
  });

  final String label;
  final T? value;
  final List<T> items;
  final String Function(T item) itemLabel;
  final ValueChanged<T?> onChanged;
  final bool enabled;

  @override
  Widget build(BuildContext context) {
    return DropdownButtonFormField<T>(
      value: value,
      decoration: InputDecoration(
        labelText: label,
        filled: true,
        fillColor: Colors.white,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: AppColors.border),
        ),
      ),
      items: items
          .map(
            (item) => DropdownMenuItem<T>(
              value: item,
              child: Text(itemLabel(item)),
            ),
          )
          .toList(),
      onChanged: enabled ? onChanged : null,
    );
  }
}
