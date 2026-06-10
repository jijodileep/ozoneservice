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
