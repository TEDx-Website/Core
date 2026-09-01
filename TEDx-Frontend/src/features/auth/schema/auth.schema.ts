import { z } from "zod";

const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\w\W]{8,}$/;

export const getLoginSchema = (t: (arg: string) => string) =>
  z.object({
    email: z.string().email({ message: t("errors.invalidEmail") }),
    password: z.string().min(1, { message: t("errors.requiredPassword") }),
  });

export const getRegisterSchema = (t: (arg: string) => string) =>
  z
    .object({
      firstName: z.string().min(2, { message: t("errors.firstNameMin") }).max(100),
      lastName: z.string().min(2, { message: t("errors.lastNameMin") }).max(100),
      email: z.string().email({ message: t("errors.invalidEmail") }).max(256),
      password: z.string().regex(passwordRegex, { message: t("errors.weakPassword") }),
      confirmPassword: z.string(),
    })
    .refine((data) => data.password === data.confirmPassword, {
      message: t("errors.passwordMismatch"),
      path: ["confirmPassword"],
    });

export const getForgotPasswordSchema = (t: (arg: string) => string) =>
  z.object({
    email: z.string().email({ message: t("errors.invalidEmail") }),
  });

export const getResetPasswordSchema = (t: (arg: string) => string) =>
  z
    .object({
      email: z.string().email({ message: t("errors.invalidEmail") }),
      token: z.string().min(1, { message: t("errors.invalidToken") }),
      newPassword: z.string().regex(passwordRegex, { message: t("errors.weakPassword") }),
      confirmPassword: z.string(),
    })
    .refine((data) => data.newPassword === data.confirmPassword, {
      message: t("errors.passwordMismatch"),
      path: ["confirmPassword"],
    });

export type LoginInput = z.infer<ReturnType<typeof getLoginSchema>>;
export type RegisterInput = z.infer<ReturnType<typeof getRegisterSchema>>;
export type ForgotPasswordInput = z.infer<ReturnType<typeof getForgotPasswordSchema>>;
export type ResetPasswordInput = z.infer<ReturnType<typeof getResetPasswordSchema>>;