class MobileBrandOption {
  const MobileBrandOption({required this.id, required this.name});

  final String id;
  final String name;

  factory MobileBrandOption.fromJson(Map<String, dynamic> json) {
    return MobileBrandOption(
      id: json['id'] as String,
      name: json['name'] as String,
    );
  }
}

class MobileModelOption {
  const MobileModelOption({
    required this.id,
    required this.brandId,
    required this.name,
  });

  final String id;
  final String brandId;
  final String name;

  factory MobileModelOption.fromJson(Map<String, dynamic> json) {
    return MobileModelOption(
      id: json['id'] as String,
      brandId: json['brandId'] as String,
      name: json['name'] as String,
    );
  }
}

class MobileVariantOption {
  const MobileVariantOption({
    required this.id,
    required this.modelId,
    required this.name,
  });

  final String id;
  final String modelId;
  final String name;

  factory MobileVariantOption.fromJson(Map<String, dynamic> json) {
    return MobileVariantOption(
      id: json['id'] as String,
      modelId: json['modelId'] as String,
      name: json['name'] as String,
    );
  }
}
