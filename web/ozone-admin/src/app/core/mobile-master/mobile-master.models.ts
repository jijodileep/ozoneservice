export interface MobileBrand {
  id: string;
  name: string;
  isActive: boolean;
  tenantId: string;
}

export interface MobileModel {
  id: string;
  brandId: string;
  name: string;
  isActive: boolean;
  tenantId: string;
}

export interface MobileVariant {
  id: string;
  modelId: string;
  name: string;
  isActive: boolean;
  tenantId: string;
}

export interface CreateMobileBrandRequest {
  name: string;
}

export interface UpdateMobileBrandRequest {
  name: string;
  isActive: boolean;
}

export interface CreateMobileModelRequest {
  name: string;
}

export interface UpdateMobileModelRequest {
  name: string;
  isActive: boolean;
}

export interface CreateMobileVariantRequest {
  name: string;
}

export interface UpdateMobileVariantRequest {
  name: string;
  isActive: boolean;
}
