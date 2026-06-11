import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/auth/auth_controller.dart';
import '../../core/auth/auth_service.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/gradient_background.dart';
import '../../core/widgets/ozone_logo.dart';

class LoginScreen extends ConsumerStatefulWidget {
  const LoginScreen({super.key});

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController(text: 'staff@localhost.dev');
  final _passwordController = TextEditingController(text: 'Staff@123');

  String? _error;
  bool _obscurePassword = true;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() => _error = null);

    await ref.read(authControllerProvider.notifier).login(
          _emailController.text.trim(),
          _passwordController.text,
        );

    if (!mounted) {
      return;
    }

    final authState = ref.read(authControllerProvider);
    authState.whenOrNull(
      error: (error, _) {
        if (!mounted) {
          return;
        }
        setState(() {
          if (error is AuthException) {
            _error = error.message;
          } else if (error is DioException) {
            final data = error.response?.data;
            if (data is Map && data['message'] is String) {
              _error = data['message'] as String;
            } else {
              _error = 'Invalid email or password.';
            }
          } else {
            _error = 'Could not sign in. Check the API is running.';
          }
        });
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final auth = ref.watch(authControllerProvider);
    final loading = auth.isLoading;
    final wide = MediaQuery.sizeOf(context).width >= 900;

    return Scaffold(
      backgroundColor: const Color(0xFF4F46E5),
      body: wide ? _buildSplitLayout(loading) : _buildMobileLayout(loading),
    );
  }

  Widget _buildSplitLayout(bool loading) {
    return Row(
      children: [
        Expanded(
          flex: 115,
          child: GradientBackground(
            gradient: AppTheme.brandGradient,
            child: SafeArea(
              child: Padding(
                padding: const EdgeInsets.all(48),
                child: Align(
                  alignment: Alignment.centerLeft,
                  child: ConstrainedBox(
                    constraints: const BoxConstraints(maxWidth: 420),
                    child: const _BrandHero(showFeatures: true),
                  ),
                ),
              ),
            ),
          ),
        ),
        Expanded(
          flex: 85,
          child: ColoredBox(
            color: Colors.white,
            child: Center(
              child: _LoginFormCard(
                formKey: _formKey,
                emailController: _emailController,
                passwordController: _passwordController,
                error: _error,
                loading: loading,
                obscurePassword: _obscurePassword,
                onTogglePassword: () => setState(() => _obscurePassword = !_obscurePassword),
                onSubmit: _submit,
              ),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildMobileLayout(bool loading) {
    final sheetHeight = MediaQuery.sizeOf(context).height * 0.58;

    return SizedBox.expand(
      child: GradientBackground(
        child: Stack(
          fit: StackFit.expand,
          children: [
            SafeArea(
              child: Align(
                alignment: Alignment.topCenter,
                child: Padding(
                  padding: const EdgeInsets.only(top: 48),
                  child: _BrandHero(showFeatures: false),
                ),
              ),
            ),
            Align(
              alignment: Alignment.bottomCenter,
              child: Container(
                height: sheetHeight,
                width: double.infinity,
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: const BorderRadius.vertical(top: Radius.circular(32)),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.12),
                      blurRadius: 24,
                      offset: const Offset(0, -4),
                    ),
                  ],
                ),
                child: SingleChildScrollView(
                  padding: const EdgeInsets.fromLTRB(28, 32, 28, 32),
                  child: _LoginFormCard(
                    formKey: _formKey,
                    emailController: _emailController,
                    passwordController: _passwordController,
                    error: _error,
                    loading: loading,
                    obscurePassword: _obscurePassword,
                    onTogglePassword: () => setState(() => _obscurePassword = !_obscurePassword),
                    onSubmit: _submit,
                    compact: true,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _BrandHero extends StatelessWidget {
  const _BrandHero({required this.showFeatures});

  final bool showFeatures;

  static const _features = [
    (Icons.build_circle_outlined, 'Track repair jobs end-to-end'),
    (Icons.people_outline, 'Manage customers at your branch'),
    (Icons.storefront_outlined, 'Multi-branch shop operations'),
  ];

  @override
  Widget build(BuildContext context) {
    final titleStyle = Theme.of(context).textTheme.headlineMedium?.copyWith(
          color: Colors.white,
          fontWeight: FontWeight.w700,
          letterSpacing: -0.5,
          height: 1.2,
        );

    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        OzoneLogo(size: showFeatures ? 72 : 64),
        SizedBox(height: showFeatures ? 28 : 20),
        Text(
          'Ozone Shop',
          style: titleStyle?.copyWith(fontSize: showFeatures ? 32 : 28),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 8),
        Text(
          showFeatures
              ? 'Run your service center from the shop floor.'
              : 'Welcome back',
          style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                color: Colors.white.withValues(alpha: 0.85),
              ),
          textAlign: TextAlign.center,
        ),
        if (showFeatures) ...[
          const SizedBox(height: 36),
          ..._features.map(
            (feature) => Padding(
              padding: const EdgeInsets.only(bottom: 14),
              child: Row(
                children: [
                  Icon(feature.$1, color: const Color(0xFF93C5FD), size: 20),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Text(
                      feature.$2,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                            color: const Color(0xFFE2E8F0),
                          ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ],
    );
  }
}

class _LoginFormCard extends StatelessWidget {
  const _LoginFormCard({
    required this.formKey,
    required this.emailController,
    required this.passwordController,
    required this.error,
    required this.loading,
    required this.obscurePassword,
    required this.onTogglePassword,
    required this.onSubmit,
    this.compact = false,
  });

  final GlobalKey<FormState> formKey;
  final TextEditingController emailController;
  final TextEditingController passwordController;
  final String? error;
  final bool loading;
  final bool obscurePassword;
  final VoidCallback onTogglePassword;
  final VoidCallback onSubmit;
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return ConstrainedBox(
      constraints: const BoxConstraints(maxWidth: 380),
      child: Form(
        key: formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              'Sign in',
              style: textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 6),
            Text(
              compact ? 'Enter your account details' : 'Enter your credentials to continue',
              style: textTheme.bodyMedium?.copyWith(color: AppColors.textMuted),
            ),
            SizedBox(height: compact ? 24 : 28),
            TextFormField(
              controller: emailController,
              keyboardType: TextInputType.emailAddress,
              autofillHints: const [AutofillHints.username],
              textInputAction: TextInputAction.next,
              decoration: const InputDecoration(
                labelText: 'Email address',
                hintText: 'you@company.dev',
                prefixIcon: Icon(Icons.email_outlined, size: 20),
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Email is required';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: passwordController,
              obscureText: obscurePassword,
              autofillHints: const [AutofillHints.password],
              textInputAction: TextInputAction.done,
              decoration: InputDecoration(
                labelText: 'Password',
                hintText: 'Enter your password',
                prefixIcon: const Icon(Icons.lock_outline, size: 20),
                suffixIcon: IconButton(
                  onPressed: onTogglePassword,
                  icon: Icon(
                    obscurePassword ? Icons.visibility_outlined : Icons.visibility_off_outlined,
                    size: 20,
                  ),
                ),
              ),
              validator: (value) {
                if (value == null || value.length < 8) {
                  return 'Password must be at least 8 characters';
                }
                return null;
              },
              onFieldSubmitted: (_) => onSubmit(),
            ),
            if (error != null) ...[
              const SizedBox(height: 16),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.red.shade50,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: Colors.red.shade100),
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Icon(Icons.error_outline, size: 18, color: Colors.red.shade700),
                    const SizedBox(width: 10),
                    Expanded(
                      child: Text(
                        error!,
                        style: textTheme.bodySmall?.copyWith(
                          color: Colors.red.shade800,
                          height: 1.4,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
            const SizedBox(height: 28),
            GradientButton(
              onPressed: onSubmit,
              loading: loading,
              child: Text(
                'Sign in',
                style: textTheme.titleSmall?.copyWith(
                  color: Colors.white,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
