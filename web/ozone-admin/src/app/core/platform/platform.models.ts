export interface ShopSummary {
  id: string;
  name: string;
  code: string;
  isActive: boolean;
  planName: string;
  planCode: string;
  maxUsers: number;
  maxBranches: number;
  maxDevicesPerUser: number;
  branchCount: number;
  userCount: number;
  subscriptionExpiresAt: string | null;
  createdAt: string;
}

export interface PlanSummary {
  id: string;
  name: string;
  code: string;
  maxUsers: number;
  maxBranches: number;
  maxDevicesPerUser: number;
  price: number;
  billingPeriodMonths: number;
  tierOrder: number;
  allowWebLogin: boolean;
  allowMobileLogin: boolean;
  isActive: boolean;
  tenantCount: number;
}

export interface CreatePlanRequest {
  name: string;
  code: string;
  maxUsers: number;
  maxBranches: number;
  maxDevicesPerUser: number;
  price: number;
  billingPeriodMonths: number;
  tierOrder: number;
  allowWebLogin: boolean;
  allowMobileLogin: boolean;
}

export interface UpdatePlanRequest {
  name: string;
  maxUsers: number;
  maxBranches: number;
  maxDevicesPerUser: number;
  price: number;
  billingPeriodMonths: number;
  tierOrder: number;
  allowWebLogin: boolean;
  allowMobileLogin: boolean;
  isActive: boolean;
}

export interface TaxConfiguration {
  id: string;
  name: string;
  cgstRate: number;
  sgstRate: number;
  totalGstRate: number;
  isActive: boolean;
  updatedAt: string;
}

export interface UpdateTaxConfigurationRequest {
  name: string;
  cgstRate: number;
  sgstRate: number;
}

export interface UpgradeRequestSummary {
  id: string;
  tenantId: string;
  tenantName: string;
  currentPlanName: string;
  requestedPlanName: string;
  requestedPlanPrice: number;
  status: string;
  requestedAt: string;
  reviewedAt: string | null;
  rejectionReason: string | null;
  invoiceId: string | null;
  invoiceNumber: string | null;
}
