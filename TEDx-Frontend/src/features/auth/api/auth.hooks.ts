import { useMutation } from "@tanstack/react-query";
import { authService } from "./auth.service";
import {
  LoginInput,
  RegisterInput,
  ForgotPasswordInput,
  ResetPasswordInput,
} from "../schema/auth.schema";
import {  AuthTokens, RegisterData } from "../types/auth.types";
import { BaseResponse,ApiError } from "@/shared/types/api";

export const useRegister = () => {
  return useMutation<BaseResponse<RegisterData>, ApiError, RegisterInput>({
    mutationFn: authService.register,
  });
};

export const useLogin = () => {
  return useMutation<BaseResponse<AuthTokens>, ApiError, LoginInput>({
    mutationFn: authService.login,
  });
};

export const useForgotPassword = () => {
  return useMutation<BaseResponse<string>, ApiError, ForgotPasswordInput>({
    mutationFn: authService.forgotPassword,
  });
};

export const useResetPassword = () => {
  return useMutation<BaseResponse<string>, ApiError, ResetPasswordInput>({
    mutationFn: authService.resetPassword,
  });
};

export const useConfirmEmail = () => {
  return useMutation<BaseResponse<string>, ApiError, { userId: string; token: string }>({
    mutationFn: ({ userId, token }) => authService.confirmEmail(userId, token),
  });
};

export const useResendConfirmation = () => {
  return useMutation<BaseResponse<string>, ApiError, string>({
    mutationFn: (email: string) => authService.resendConfirmation(email),
  });
};