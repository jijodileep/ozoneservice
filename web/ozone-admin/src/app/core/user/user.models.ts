export interface UserBranchSummary {
  id: string;
  code: string;
  name: string;
}

export interface UserDetail {
  id: string;
  email: string;
  displayName: string;
  role: string;
  isActive: boolean;
  branches: UserBranchSummary[];
}

export interface CreateUserRequest {
  email: string;
  displayName: string;
  password: string;
  role: string;
  branchIds: string[];
}

export interface UpdateUserRequest {
  displayName: string;
  role: string;
  branchIds: string[];
  isActive: boolean;
}

export interface ResetUserPasswordRequest {
  newPassword: string;
}

export const ASSIGNABLE_ROLES = [
  { value: 'ShopAdmin', label: 'Shop Admin' },
  { value: 'ShopStaff', label: 'Shop Staff' },
  { value: 'Accountant', label: 'Accountant' },
] as const;
