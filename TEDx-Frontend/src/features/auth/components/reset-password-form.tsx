"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { ArrowRight, Check } from "lucide-react";
import {
  getResetPasswordSchema,
  ResetPasswordInput,
} from "../schema/auth.schema";
import { PasswordInput } from "@/shared/ui/password-input";
import { PasswordStrength } from "./password-strength";
import { useResetPassword } from "../api/auth.hooks";
import { Button } from "@/shared/ui/button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  CardFooter,
} from "@/shared/ui/card";
import { Spinner } from "@/shared/ui/spinner";

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
          message:
            error?.response?.data?.error?.message ||
            error.message ||
            "Failed to connect to server",
        });
      },
    });
  };

  if (isSuccess) {
    return (
      <Card className="w-full border-0 shadow-none bg-transparent sm:bg-card sm:border sm:shadow-sm text-center">
        <CardContent className="px-0 sm:px-6 pt-6">
          <div className="w-14 h-14 rounded-2xl bg-emerald-950/40 border border-emerald-500/40 text-emerald-400 flex items-center justify-center mx-auto mb-5 shadow-lg">
            <Check className="size-6" />
          </div>

          <h2 className="font-black text-2xl sm:text-3xl text-foreground tracking-tight mb-2.5">
            {t("successTitle")}
          </h2>

          <div className="p-4 rounded-xl bg-neutral-100 dark:bg-[#121217] border border-neutral-200 dark:border-[#2B2B38] text-xs sm:text-sm text-neutral-500 dark:text-[#D4D4D8] leading-relaxed mb-6 text-left space-y-2">
            <div className="flex items-center gap-2 text-foreground font-semibold">
              <span className="size-2 rounded-full bg-emerald-400"></span>
              <span>{t("successTitle")}</span>
            </div>
            <p>{t("successMessage")}</p>
          </div>

          <Button
            asChild
            className="w-full bg-brand-500 hover:bg-brand-600 text-white h-12 shadow-md"
          >
            <Link href="/login" prefetch={false}>
              <span>{t("signInNew")}</span>
              <ArrowRight className="size-4 ml-2" />
            </Link>
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="w-full border-0 shadow-none bg-transparent sm:bg-card sm:border sm:shadow-sm">
      <CardHeader className="px-0 sm:px-6 mb-2">
        <CardTitle className="font-black text-2xl sm:text-3xl text-foreground tracking-tight">
          {t("title")}
        </CardTitle>
      </CardHeader>

      <CardContent className="px-0 sm:px-6">
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

          <Button
            type="submit"
            disabled={isPending}
            className="w-full bg-brand-500 hover:bg-brand-600 text-white h-12 mt-4 shadow-[0_2px_10px_rgba(0,0,0,0.5),0_0_24px_rgba(235,0,40,0.35)]"
          >
            {isPending ? (
              <>
                <Spinner className="mr-2 text-white" />
                <span>{t("submitting")}</span>
              </>
            ) : (
              <span>{t("submit")}</span>
            )}
          </Button>
        </form>
      </CardContent>

      <CardFooter className="px-0 sm:px-6 justify-center pb-2 mt-3">
        <Link
          href="/login"
          prefetch={false}
          className="text-xs text-neutral-500 dark:text-[#D4D4D8] hover:text-foreground transition-colors hover:underline underline-offset-4"
        >
          {t("cancel")}
        </Link>
      </CardFooter>
    </Card>
  );
}
