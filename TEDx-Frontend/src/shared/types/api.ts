export interface ApiErrorResponse {
  code: string;
  message: string;
  fieldErrors?: Record<string, string[]>;
  traceId?: string;
}

export interface ApiResponse<T = unknown> {
  success: boolean;
  data: T;
  error: ApiErrorResponse | null;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}

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

export interface ApiError extends Error {
  response?: {
    data?: BaseResponse<null>;
    status?: number;
  };
}