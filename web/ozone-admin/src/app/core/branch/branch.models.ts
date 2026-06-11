export interface BranchSummary {
  id: string;
  code: string;
  name: string;
  tenantId: string;
}

export interface BranchDetail {
  id: string;
  code: string;
  name: string;
  address: string | null;
  phone: string | null;
  gstNumber: string | null;
  isActive: boolean;
  tenantId: string;
}

export interface CreateBranchRequest {
  code: string;
  name: string;
  address?: string | null;
  phone?: string | null;
  gstNumber?: string | null;
}

export interface UpdateBranchRequest {
  name: string;
  address?: string | null;
  phone?: string | null;
  gstNumber?: string | null;
  isActive: boolean;
}
