"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { Loader2, ArrowRight } from "lucide-react";
import {
  getResetPasswordSchema,
  ResetPasswordInput,
} from "../schema/auth.schema";
import { PasswordInput } from "@/shared/ui/password-input";
import { PasswordStrength } from "./password-strength";
import { useResetPassword } from "../api/auth.hooks";

interface ResetPasswordFormProps {
  token: string;
  email: string;
}

export function ResetPasswordForm({ token, email }: ResetPasswordFormProps) {
  const t = useTranslations("auth.resetPassword");
  const tErrors = useTranslations("auth.errors");
  const [isSuccess, setIsSuccess] = useState(false);

  const { mutate: resetPasswordMutation, isPending } = useResetPassword();

  const {
    register,
    handleSubmit,
    watch,
    setError,
    formState: { errors },
  } = useForm<ResetPasswordInput>({
    resolver: zodResolver(getResetPasswordSchema(tErrors)),
    defaultValues: {
      email: email || "",
      token: token || "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  const passwordValue = watch("newPassword");

  const onSubmit = (data: ResetPasswordInput) => {
    resetPasswordMutation(data, {
      onSuccess: (response) => {
        if (!response.success) {
          setError("root", {
            message: response.error?.message || "Reset failed",
          });
          return;
        }
        setIsSuccess(true);
      },
      onError: (error) => {
        setError("root", {
          message: error.message || "Failed to connect to server",
        });
      },
    });
  };

  if (isSuccess) {
    return (
      <div className="w-full text-center">
        <div className="w-14 h-14 rounded-2xl bg-emerald-950/40 border border-emerald-500/40 text-emerald-400 flex items-center justify-center mx-auto mb-5 shadow-lg">
          <span className="text-xl font-bold">✓</span>
        </div>

        <h2 className="font-black text-2xl sm:text-3xl text-foreground tracking-tight mb-2.5">
          {t("successTitle")}
        </h2>

        <div className="p-4 rounded-xl bg-neutral-100 dark:bg-[#121217] border border-neutral-200 dark:border-[#2B2B38] text-xs sm:text-sm text-neutral-500 dark:text-[#D4D4D8] leading-relaxed mb-6 text-left space-y-2">
          <div className="flex items-center gap-2 text-foreground font-semibold">
            <span className="size-2 rounded-full bg-emerald-400"></span>
            <span>Security Update Applied</span>
          </div>
          <p>{t("successMessage")}</p>
        </div>

        <Link
          href="/login"
          className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-lg px-5 py-3.5 flex items-center justify-center gap-2 transition-all shadow-md"
        >
          <span>{t("signInNew")}</span>
          <ArrowRight className="size-4" />
        </Link>
      </div>
    );
  }

  return (
    <div className="w-full">
      <div className="mb-6">
        <h2 className="font-black text-2xl sm:text-3xl text-foreground tracking-tight">
          {t("title")}
        </h2>
      </div>

      {errors.root && (
        <div className="mb-4 p-3.5 rounded-lg bg-red-950/40 border border-red-800/60 text-xs text-red-200">
          <p>{errors.root.message}</p>
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <input type="hidden" {...register("email")} />
        <input type="hidden" {...register("token")} />

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="newPassword"
          >
            {t("newPasswordLabel")}
          </label>
          <PasswordInput
            {...register("newPassword")}
            id="newPassword"
            placeholder={t("newPasswordPlaceholder")}
            className={errors.newPassword ? "border-red-500" : ""}
          />
          <PasswordStrength password={passwordValue} />
          {errors.newPassword && (
            <span className="text-[11px] text-red-500 mt-1 block">
              {errors.newPassword.message}
            </span>
          )}
        </div>

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="resetConfirm"
          >
            {t("confirmPasswordLabel")}
          </label>
          <PasswordInput
            {...register("confirmPassword")}
            id="resetConfirm"
            placeholder={t("confirmPasswordPlaceholder")}
            className={errors.confirmPassword ? "border-red-500" : ""}
          />
          {errors.confirmPassword && (
            <span className="text-[11px] text-red-500 mt-1 block">
              {errors.confirmPassword.message}
            </span>
          )}
        </div>

        <button
          type="submit"
          disabled={isPending}
          className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-lg px-5 py-3.5 flex items-center justify-center gap-2 transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-[0_2px_10px_rgba(0,0,0,0.5),0_0_24px_rgba(235,0,40,0.35)] mt-4"
        >
          {isPending ? (
            <>
              <Loader2 className="size-4 animate-spin" />
              <span>{t("submitting")}</span>
            </>
          ) : (
            <span>{t("submit")}</span>
          )}
        </button>
      </form>

      <div className="mt-7 text-center">
        <Link
          href="/login"
          className="text-xs text-neutral-500 dark:text-[#D4D4D8] hover:text-foreground transition-colors hover:underline underline-offset-4"
        >
          {t("cancel")}
        </Link>
      </div>
    </div>
  );
}
