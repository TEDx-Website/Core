import { useMutation } from "@tanstack/react-query";
import { authService } from "./auth.service";
import {
  LoginInput,
  RegisterInput,
  ForgotPasswordInput,
  ResetPasswordInput,
} from "../schema/auth.schema";
import { BaseResponse, AuthTokens, RegisterData } from "../types/auth.types";

export const useRegister = () => {
  return useMutation<BaseResponse<RegisterData>, Error, RegisterInput>({
    mutationFn: authService.register,
  });
};

export const useLogin = () => {
  return useMutation<BaseResponse<AuthTokens>, Error, LoginInput>({
    mutationFn: authService.login,
  });
};

export const useForgotPassword = () => {
  return useMutation<BaseResponse<string>, Error, ForgotPasswordInput>({
    mutationFn: authService.forgotPassword,
  });
};

export const useResetPassword = () => {
  return useMutation<BaseResponse<string>, Error, ResetPasswordInput>({
    mutationFn: authService.resetPassword,
  });
};

export const useConfirmEmail = () => {
  return useMutation<BaseResponse<string>, Error, { userId: string; token: string }>({
    mutationFn: ({ userId, token }) => authService.confirmEmail(userId, token),
  });
};

export const useResendConfirmation = () => {
  return useMutation<BaseResponse<string>, Error, string>({
    mutationFn: (email: string) => authService.resendConfirmation(email),
  });
};