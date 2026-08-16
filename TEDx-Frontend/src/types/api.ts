// Names here mirror the C# payload types exactly, so one grep crosses the whole
// stack. See docs/13-NamingConventions.md §5.

export interface ApiError {
  code: string;
  message: string;
  fieldErrors?: Record<string, string[]>;
  traceId?: string;
}

export interface PagedMeta {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface ApiResponse<T = unknown> {
  success: boolean;
  data: T;
  error: ApiError | null;
  /** Present only on paged endpoints. */
  meta?: PagedMeta;
}

export interface AuthUserDto {
  id: string;
  email: string;
  globalRole: string;
  firstName: string | null;
  lastName: string | null;
}

export interface AuthTokensResponse {
  accessToken: string;
  accessTokenExpiresIn: number;
  refreshToken: string;
  refreshTokenExpiresIn: number;
  user: AuthUserDto;
}
