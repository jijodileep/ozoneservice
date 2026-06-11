class CustomerDevice {
  const CustomerDevice({
    required this.id,
    required this.variantId,
    required this.brandName,
    required this.modelName,
    required this.variantName,
    this.imei,
    required this.registeredAtBranchId,
    required this.registeredAtBranchName,
    required this.isActive,
    required this.createdAt,
  });

  final String id;
  final String variantId;
  final String brandName;
  final String modelName;
  final String variantName;
  final String? imei;
  final String registeredAtBranchId;
  final String registeredAtBranchName;
  final bool isActive;
  final DateTime createdAt;

  factory CustomerDevice.fromJson(Map<String, dynamic> json) {
    return CustomerDevice(
      id: json['id'] as String,
      variantId: json['variantId'] as String,
      brandName: json['brandName'] as String,
      modelName: json['modelName'] as String,
      variantName: json['variantName'] as String,
      imei: json['imei'] as String?,
      registeredAtBranchId: json['registeredAtBranchId'] as String,
      registeredAtBranchName: json['registeredAtBranchName'] as String,
      isActive: json['isActive'] as bool,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );
  }

  String get displayLabel => '$brandName $modelName $variantName';
}

class CustomerHistoryItem {
  const CustomerHistoryItem({
    required this.id,
    required this.reference,
    required this.status,
    required this.type,
    required this.occurredAt,
  });

  final String id;
  final String reference;
  final String status;
  final String type;
  final DateTime occurredAt;

  factory CustomerHistoryItem.fromJson(Map<String, dynamic> json) {
    return CustomerHistoryItem(
      id: json['id'] as String,
      reference: json['reference'] as String,
      status: json['status'] as String,
      type: json['type'] as String,
      occurredAt: DateTime.parse(json['occurredAt'] as String),
    );
  }
}

class CustomerDetail {
  const CustomerDetail({
    required this.id,
    required this.name,
    required this.mobileNumber,
    this.email,
    this.address,
    required this.devices,
    required this.history,
  });

  final String id;
  final String name;
  final String mobileNumber;
  final String? email;
  final String? address;
  final List<CustomerDevice> devices;
  final List<CustomerHistoryItem> history;

  factory CustomerDetail.fromJson(Map<String, dynamic> json) {
    return CustomerDetail(
      id: json['id'] as String,
      name: json['name'] as String,
      mobileNumber: json['mobileNumber'] as String,
      email: json['email'] as String?,
      address: json['address'] as String?,
      devices: (json['devices'] as List<dynamic>)
          .map((e) => CustomerDevice.fromJson(e as Map<String, dynamic>))
          .toList(),
      history: (json['history'] as List<dynamic>)
          .map((e) => CustomerHistoryItem.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}
