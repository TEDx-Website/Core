import { apiClient } from "@/lib/api";
import {
  LoginInput,
  RegisterInput,
  ForgotPasswordInput,
  ResetPasswordInput,
} from "../schema/auth.schema";
import { AuthTokens, BaseResponse, RegisterData } from "../types/auth.types";

export const authService = {
  register: async (data: RegisterInput): Promise<BaseResponse<RegisterData>> => {
    const response = await apiClient.post<BaseResponse<RegisterData>>("/auth/register", {
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      password: data.password,
      confirmPassword: data.confirmPassword,
    });
    return response.data;
  },

  login: async (data: LoginInput): Promise<BaseResponse<AuthTokens>> => {
    const response = await apiClient.post<BaseResponse<AuthTokens>>("/auth/login", data);
    return response.data;
  },

  refreshToken: async (refreshToken: string): Promise<BaseResponse<AuthTokens>> => {
    const response = await apiClient.post<BaseResponse<AuthTokens>>("/auth/refresh", {
      refreshToken,
    });
    return response.data;
  },

  logout: async (): Promise<BaseResponse<string>> => {
    const response = await apiClient.post<BaseResponse<string>>("/auth/logout");
    return response.data;
  },

  forgotPassword: async (data: ForgotPasswordInput): Promise<BaseResponse<string>> => {
    const response = await apiClient.post<BaseResponse<string>>("/auth/forgot-password", data);
    return response.data;
  },

  resetPassword: async (data: ResetPasswordInput): Promise<BaseResponse<string>> => {
    const response = await apiClient.post<BaseResponse<string>>("/auth/reset-password", {
      email: data.email,
      token: data.token,
      newPassword: data.newPassword,
      confirmPassword: data.confirmPassword,
    });
    return response.data;
  },

  confirmEmail: async (userId: string, token: string): Promise<BaseResponse<string>> => {
    const response = await apiClient.post<BaseResponse<string>>("/auth/confirm-email", {
      userId,
      token,
    });
    return response.data;
  },

  resendConfirmation: async (email: string): Promise<BaseResponse<string>> => {
    const response = await apiClient.post<BaseResponse<string>>("/auth/resend-confirmation", {
      email,
    });
    return response.data;
  },
};