
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