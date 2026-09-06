"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { useRouter } from "next/navigation";
import {
  getChangePasswordSchema,
  ChangePasswordInput,
} from "../schema/profile.schema";
import { useChangePassword } from "../api/profile.hooks";
import { clearAuthCookies } from "@/features/auth/api/auth.actions";
import { PasswordInput } from "@/shared/ui/password-input";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";

export function ChangePasswordForm() {
  const t = useTranslations("profile");
  const router = useRouter();

  const { mutate: changePassword, isPending } = useChangePassword();

  const {
    register,
    handleSubmit,
    watch,
    setError,
    formState: { errors },
  } = useForm<ChangePasswordInput>({
    resolver: zodResolver(getChangePasswordSchema(t)),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  const newPasswordValue = watch("newPassword") || "";

  const strengthScore = (() => {
    let s = 0;
    if (newPasswordValue.length >= 8) s++;
    if (/[A-Z]/.test(newPasswordValue)) s++;
    if (/[a-z]/.test(newPasswordValue)) s++;
    if (/[0-9]/.test(newPasswordValue)) s++;
    return s;
  })();

  const onSubmit = (data: ChangePasswordInput) => {
    changePassword(data, {
      onSuccess: async (res) => {
        if (!res.success) {
          setError("currentPassword", {
            message: res.error?.message || "Failed",
          });
          return;
        }
        await clearAuthCookies();
        router.push("/login");
      },
      onError: (err) => {
        setError("root", { message: err.message || "Connection error" });
      },
    });
  };

  return (
    <div className="max-w-xl space-y-6">
      <div className="p-4 rounded-xl bg-purple-950/30 border border-purple-500/30 text-xs flex items-start gap-3">
        <p className="leading-relaxed text-[#D4D4D8]">
          <strong className="text-purple-300 font-semibold">
            {t("security.warningTitle")}
          </strong>{" "}
          {t("security.warningText")}
        </p>
      </div>

      {errors.root && (
        <div className="p-3 rounded-xl bg-red-950/40 border border-red-500 text-red-200 text-xs">
          {errors.root.message}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="currentPassword"
          >
            {t("security.currentPassword")}{" "}
            <span className="text-brand-500">*</span>
          </label>
          <PasswordInput
            {...register("currentPassword")}
            id="currentPassword"
            className={errors.currentPassword ? "border-red-500" : ""}
          />
          {errors.currentPassword && (
            <span className="text-[11px] text-red-400 mt-1 block">
              {errors.currentPassword.message}
            </span>
          )}
        </div>

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="newPassword"
          >
            {t("security.newPassword")}{" "}
            <span className="text-brand-500">*</span>
          </label>
          <PasswordInput
            {...register("newPassword")}
            id="newPassword"
            className={errors.newPassword ? "border-red-500" : ""}
          />
          <div className="mt-2 space-y-1.5">
            <div className="grid grid-cols-4 gap-1.5">
              {[1, 2, 3, 4].map((idx) => (
                <div
                  key={idx}
                  className={`h-1 rounded-full transition-all ${
                    idx <= strengthScore ? "bg-brand-500" : "bg-[#1E1E26]"
                  }`}
                />
              ))}
            </div>
            <p className="text-[11px] text-[#71717A]">
              {t("security.policyHint")}
            </p>
          </div>
          {errors.newPassword && (
            <span className="text-[11px] text-red-400 mt-1 block">
              {errors.newPassword.message}
            </span>
          )}
        </div>

        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="confirmPassword"
          >
            {t("security.confirmPassword")}{" "}
            <span className="text-brand-500">*</span>
          </label>
          <PasswordInput
            {...register("confirmPassword")}
            id="confirmPassword"
            className={errors.confirmPassword ? "border-red-500" : ""}
          />
          {errors.confirmPassword && (
            <span className="text-[11px] text-red-400 mt-1 block">
              {errors.confirmPassword.message}
            </span>
          )}
        </div>

        <Button
          type="submit"
          disabled={isPending}
          className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-xl h-12 flex items-center justify-center gap-2 shadow-md"
        >
          {isPending ? (
            <>
              <Spinner className="mr-2 text-white" />
              <span>{t("security.updating")}</span>
            </>
          ) : (
            <span>{t("security.updateBtn")}</span>
          )}
        </Button>
      </form>
    </div>
  );
}
