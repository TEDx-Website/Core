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