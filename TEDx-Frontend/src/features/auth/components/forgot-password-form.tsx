"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { Loader2, ArrowLeft, Send } from "lucide-react";
import {
  getForgotPasswordSchema,
  ForgotPasswordInput,
} from "../schema/auth.schema";
import { useForgotPassword } from "../api/auth.hooks";

export function ForgotPasswordForm() {
  const t = useTranslations("auth.forgotPassword");
  const tErrors = useTranslations("auth.errors");
  const [isDispatched, setIsDispatched] = useState(false);

  const { mutate: forgotPasswordMutation, isPending } = useForgotPassword();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm<ForgotPasswordInput>({
    resolver: zodResolver(getForgotPasswordSchema(tErrors)),
    defaultValues: {
      email: "",
    },
  });

  const onSubmit = (data: ForgotPasswordInput) => {
    forgotPasswordMutation(data, {
      onSuccess: (response) => {
        if (!response.success) {
          setError("root", {
            message: response.error?.message || "Request failed",
          });
          return;
        }
        setIsDispatched(true);
      },
      onError: (error) => {
        setError("root", {
          message: error.message || "Failed to connect to server",
        });
      },
    });
  };

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

      {isDispatched && (
        <div className="mb-5 p-4 rounded-xl bg-neutral-100 dark:bg-[#121217] border border-neutral-200 dark:border-[#2B2B38] text-xs space-y-2">
          <div className="flex items-center gap-2 text-foreground font-semibold">
            <span className="size-2 rounded-full bg-emerald-400"></span>
            <span>{t("successTitle")}</span>
          </div>
          <p className="text-neutral-500 dark:text-[#D4D4D8] leading-relaxed">
            {t("successMessage")}
          </p>
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="forgotEmail"
          >
            {t("emailLabel")}
          </label>
          <input
            {...register("email")}
            type="email"
            id="forgotEmail"
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
              <Send className="size-4" />
            </>
          )}
        </button>
      </form>

      <div className="mt-7 text-center">
        <Link
          href="/login"
          className="text-xs font-medium text-neutral-500 dark:text-[#D4D4D8] hover:text-foreground transition-colors inline-flex items-center gap-1.5 hover:underline underline-offset-4"
        >
          <ArrowLeft className="size-3.5" />
          <span>{t("backToLogin")}</span>
        </Link>
      </div>
    </div>
  );
}
