import { z } from "zod";

const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d\w\W]{8,}$/;

export const getUpdateProfileSchema = (t: (arg: string) => string) =>
  z.object({
    firstName: z.string().min(2, { message: t("errors.firstNameMin") }).max(100, { message: t("errors.firstNameMax") }),
    lastName: z.string().min(2, { message: t("errors.lastNameMin") }).max(100, { message: t("errors.lastNameMax") }),
    phone: z.string().max(32, { message: t("errors.phoneMax") }).optional().nullable(),
    bio: z.string().max(1000, { message: t("errors.bioMax") }).optional().nullable(),
  });

export const getChangePasswordSchema = (t: (arg: string) => string) =>
  z
    .object({
      currentPassword: z.string().min(1, { message: t("errors.requiredCurrentPassword") }),
      newPassword: z.string().regex(passwordRegex, { message: t("errors.weakPassword") }),
      confirmPassword: z.string(),
    })
    .refine((data) => data.newPassword === data.confirmPassword, {
      message: t("errors.passwordMismatch"),
      path: ["confirmPassword"],
    });

export type UpdateProfileInput = z.infer<ReturnType<typeof getUpdateProfileSchema>>;
export type ChangePasswordInput = z.infer<ReturnType<typeof getChangePasswordSchema>>;