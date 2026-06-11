import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../auth/auth_controller.dart';
import '../mobile_master/mobile_master_service.dart';
import 'customer_service.dart';

final customerServiceProvider = Provider<CustomerService>((ref) {
  return CustomerService(ref.watch(apiClientProvider));
});

final mobileMasterServiceProvider = Provider<MobileMasterService>((ref) {
  return MobileMasterService(ref.watch(apiClientProvider));
});
