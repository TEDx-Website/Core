export interface BaseResponse<T> {
  success: boolean;
  data: T;
  error: {
    code: string;
    message: string;
    fieldErrors: Record<string, string[]>;
    traceId: string;
  } | null;
  meta: {
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  } | null;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  globalRole: string;
}

export interface RegisterData extends User {
  emailConfirmationRequired: boolean;
}

export interface AuthTokens {
  accessToken: string;
  accessTokenExpiresIn: number;
  refreshToken: string;
  refreshTokenExpiresIn: number;
  user: User;
}