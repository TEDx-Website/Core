"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { Loader2, ArrowRight } from "lucide-react";
import { getRegisterSchema, RegisterInput } from "../schema/auth.schema";
import { PasswordInput } from "@/shared/ui/password-input";
import { PasswordStrength } from "./password-strength";
import { useRegister } from "../api/auth.hooks";

export function RegisterForm() {
  const t = useTranslations("auth.register");
  const tErrors = useTranslations("auth.errors");
  const [isRegistered, setIsRegistered] = useState(false);
  const [targetEmail, setTargetEmail] = useState("");

  const { mutate: registerMutation, isPending } = useRegister();

  const {
    register,
    handleSubmit,
    watch,
    setError,
    formState: { errors },
  } = useForm<RegisterInput>({
    resolver: zodResolver(getRegisterSchema(tErrors)),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const passwordValue = watch("password");

  const onSubmit = (data: RegisterInput) => {
    registerMutation(data, {
      onSuccess: (response) => {
        if (!response.success) {
          if (response.error?.fieldErrors) {
            Object.entries(response.error.fieldErrors).forEach(
              ([field, messages]) => {
                setError(field as keyof RegisterInput, {
                  message: messages[0],
                });
              },
            );
          } else {
            setError("root", {
              message: response.error?.message || "Registration failed",
            });
          }
          return;
        }

        setTargetEmail(data.email);
        setIsRegistered(true);
      },
      onError: (error) => {
        setError("root", {
          message: error.message || "Failed to connect to server",
        });
      },
    });
  };

  if (isRegistered) {
    return (
      <div className="w-full text-center">
        <div className="w-14 h-14 rounded-2xl bg-neutral-100 dark:bg-[#121217] border border-neutral-200 dark:border-[#2B2B38] text-brand-500 flex items-center justify-center mx-auto mb-5 shadow-xl">
          <span className="text-2xl">✉</span>
        </div>

        <h2 className="font-black text-2xl sm:text-3xl text-foreground tracking-tight mb-2.5">
          Confirm Your Email
        </h2>

        <div className="p-4 rounded-xl bg-neutral-100 dark:bg-[#121217] border border-neutral-200 dark:border-[#2B2B38] text-xs sm:text-sm text-neutral-500 dark:text-[#D4D4D8] leading-relaxed mb-6 text-left space-y-2">
          <div className="flex items-center gap-2 text-foreground font-semibold">
            <span className="size-2 rounded-full bg-brand-500 animate-pulse"></span>
            <span>Confirmation Link Dispatched</span>
          </div>
          <p>
            We sent an activation link to{" "}
            <strong className="text-foreground font-semibold">
              {targetEmail}
            </strong>
            . Please check your inbox and verify your account.
          </p>
        </div>

        <div className="space-y-3">
          <Link
            href="/login"
            className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-lg px-5 py-3.5 flex items-center justify-center gap-2 transition-all shadow-md"
          >
            <span>Proceed to Sign In</span>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full">
      <div className="mb-5">
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
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label
              className="block text-xs font-medium text-foreground mb-1.5"
              htmlFor="firstName"
            >
              {t("firstNameLabel")}
            </label>
            <input
              {...register("firstName")}
              type="text"
              id="firstName"
              placeholder={t("firstNamePlaceholder")}
              className={`w-full bg-neutral-100 dark:bg-[#121217] border ${
                errors.firstName
                  ? "border-red-500"
                  : "border-neutral-200 dark:border-[#2B2B38]"
              } rounded-lg text-foreground px-4 py-3 text-sm transition-all focus:border-brand-500 focus:dark:bg-[#15151B] focus:ring-4 focus:ring-brand-500/20 outline-none`}
            />
            {errors.firstName && (
              <span className="text-[11px] text-red-500 mt-1 block">
                {errors.firstName.message}
              </span>
            )}
          </div>
          <div>
            <label
              className="block text-xs font-medium text-foreground mb-1.5"
              htmlFor="lastName"
            >
              {t("lastNameLabel")}
            </label>
            <input
              {...register("lastName")}
              type="text"
              id="lastName"
              placeholder={t("lastNamePlaceholder")}
              className={`w-full bg-neutral-100 dark:bg-[#121217] border ${
                errors.lastName
                  ? "border-red-500"
                  : "border-neutral-200 dark:border-[#2B2B38]"
              } rounded-lg text-foreground px-4 py-3 text-sm transition-all focus:border-brand-500 focus:dark:bg-[#15151B] focus:ring-4 focus:ring-brand-500/20 outline-none`}
            />
            {errors.lastName && (
              <span className="text-[11px] text-red-500 mt-1 block">
                {errors.lastName.message}
              </span>
            )}
          </div>
        </div>

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="regEmail"
          >
            {t("emailLabel")}
          </label>
          <input
            {...register("email")}
            type="email"
            id="regEmail"
            placeholder={t("emailPlaceholder")}
            className={`w-full bg-neutral-100 dark:bg-[#121217] border ${
              errors.email
                ? "border-red-500"
                : "border-neutral-200 dark:border-[#2B2B38]"
            } rounded-lg text-foreground px-4 py-3 text-sm transition-all focus:border-brand-500 focus:dark:bg-[#15151B] focus:ring-4 focus:ring-brand-500/20 outline-none`}
          />
          {errors.email && (
            <span className="text-[11px] text-red-500 mt-1 block">
              {errors.email.message}
            </span>
          )}
        </div>

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="regPassword"
          >
            {t("passwordLabel")}
          </label>
          <PasswordInput
            {...register("password")}
            id="regPassword"
            placeholder={t("passwordPlaceholder")}
            className={errors.password ? "border-red-500" : ""}
          />
          <PasswordStrength password={passwordValue} />
          {errors.password && (
            <span className="text-[11px] text-red-500 mt-1 block">
              {errors.password.message}
            </span>
          )}
        </div>

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="confirmPassword"
          >
            {t("confirmPasswordLabel")}
          </label>
          <PasswordInput
            {...register("confirmPassword")}
            id="confirmPassword"
            placeholder={t("confirmPasswordPlaceholder")}
            className={errors.confirmPassword ? "border-red-500" : ""}
          />
          {errors.confirmPassword && (
            <span className="text-[11px] text-red-500 mt-1 block">
              {errors.confirmPassword.message}
            </span>
          )}
        </div>

        <div className="pt-0.5">
          <label className="flex items-start gap-2.5 cursor-pointer select-none text-xs text-neutral-500 dark:text-neutral-300">
            <input
              type="checkbox"
              required
              className="rounded bg-neutral-100 dark:bg-[#121217] border-neutral-300 dark:border-[#2B2B38] text-brand-500 focus:ring-0 w-4 h-4 mt-0.5"
            />
            <span className="leading-snug">
              {t("termsText")}{" "}
              <Link
                href="#"
                className="text-foreground font-medium underline underline-offset-2 hover:text-brand-500 transition-colors"
              >
                {t("termsLink")}
              </Link>{" "}
              {t("termsAnd")}
            </span>
          </label>
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
            <>
              <span>{t("submit")}</span>
              <ArrowRight className="size-4" />
            </>
          )}
        </button>
      </form>

      <div className="mt-7 text-center">
        <p className="text-xs text-neutral-500">
          {t("alreadyRegistered")}{" "}
          <Link
            href="/login"
            className="text-foreground font-semibold underline underline-offset-4 hover:text-brand-500 transition-colors ml-1.5"
          >
            {t("signIn")}
          </Link>
        </p>
      </div>
    </div>
  );
}
