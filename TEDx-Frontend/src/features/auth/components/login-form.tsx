"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Loader2, ArrowRight } from "lucide-react";
import { setAuthCookies } from "../api/auth.actions";
import { useAuthStore } from "../store/auth.store";
import { getLoginSchema, LoginInput } from "../schema/auth.schema";
import { PasswordInput } from "@/shared/ui/password-input";
import { useLogin } from "../api/auth.hooks";

export function LoginForm() {
  const t = useTranslations("auth.login");
  const tErrors = useTranslations("auth.errors");
  const router = useRouter();

  const { mutate: loginMutation, isPending } = useLogin();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm<LoginInput>({
    resolver: zodResolver(getLoginSchema(tErrors)),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const setUser = useAuthStore((state) => state.setUser);

  const onSubmit = (data: LoginInput) => {
    loginMutation(data, {
      onSuccess: async (response) => {
        if (!response.success) {
          setError("root", {
            message: response.error?.message || "An error occurred",
          });
          return;
        }

        setUser(response.data.user);
        await setAuthCookies(response.data);

        router.push("/");
        router.refresh();
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
        <div className="mb-4 p-3.5 rounded-lg bg-red-950/40 border border-red-800/60 text-xs text-red-200 flex items-start gap-2.5">
          <div>
            <strong className="font-semibold text-white">
              Authentication Failed
            </strong>
            <p className="text-red-300 mt-0.5">{errors.root.message}</p>
          </div>
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label
            className="block text-xs font-medium text-foreground mb-1.5"
            htmlFor="email"
          >
            {t("emailLabel")}
          </label>
          <input
            {...register("email")}
            type="email"
            id="email"
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
          <div className="flex items-center justify-between mb-1.5">
            <label
              className="text-xs font-medium text-foreground"
              htmlFor="password"
            >
              {t("passwordLabel")}
            </label>
            <Link
              href="/forgot-password"
              className="text-xs text-foreground hover:text-brand-500 font-medium transition-colors hover:underline underline-offset-4"
            >
              {t("forgotPassword")}
            </Link>
          </div>
          <PasswordInput
            {...register("password")}
            id="password"
            placeholder={t("passwordPlaceholder")}
            className={errors.password ? "border-red-500" : ""}
          />
          {errors.password && (
            <span className="text-[11px] text-red-500 mt-1 block">
              {errors.password.message}
            </span>
          )}
        </div>

        <div className="pt-0.5">
          <label className="flex items-center gap-2.5 cursor-pointer select-none text-xs text-neutral-500 hover:text-foreground transition-colors">
            <input
              type="checkbox"
              className="rounded bg-neutral-100 dark:bg-[#121217] border-neutral-300 dark:border-[#2B2B38] text-brand-500 focus:ring-0 w-4 h-4"
            />
            <span>{t("rememberMe")}</span>
          </label>
        </div>

        <button
          type="submit"
          disabled={isPending}
          className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-lg px-5 py-3.5 flex items-center justify-center gap-2 transition-all disabled:opacity-50 disabled:cursor-not-allowed shadow-[0_2px_10px_rgba(0,0,0,0.5),0_0_24px_rgba(235,0,40,0.35)] mt-6"
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

      <div className="relative my-6 text-center">
        <div className="absolute inset-0 flex items-center">
          <div className="w-full border-t border-neutral-200 dark:border-[#1E1E26]"></div>
        </div>
        <span className="relative px-3 bg-background text-[11px] text-neutral-500 uppercase tracking-widest font-mono">
          {t("orContinueWith")}
        </span>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <button
          type="button"
          className="bg-neutral-100 dark:bg-[#16161D] border border-neutral-200 dark:border-[#2B2B38] text-foreground text-[13px] font-medium rounded-lg px-4 py-3 flex items-center justify-center gap-2.5 transition-all hover:dark:bg-[#20202A] shadow-sm"
        >
          <svg className="size-4 shrink-0" viewBox="0 0 24 24">
            <path
              fill="#EA4335"
              d="M12 5c1.6 0 3 .6 4.1 1.7l3.1-3.1C17.3 1.8 14.8 1 12 1 7.5 1 3.7 3.6 1.9 7.3l3.7 2.9C6.5 7.4 9 5 12 5z"
            />
            <path
              fill="#4285F4"
              d="M23.5 12.3c0-.8-.1-1.6-.2-2.3H12v4.5h6.5c-.3 1.5-1.1 2.8-2.4 3.7l3.7 2.9c2.2-2 3.7-5 3.7-8.8z"
            />
            <path
              fill="#FBBC05"
              d="M5.6 14.8c-.3-.8-.4-1.8-.4-2.8s.1-2 .4-2.8L1.9 6.3C.7 8.7 0 11.3 0 14s.7 5.3 1.9 7.7l3.7-2.9z"
            />
            <path
              fill="#34A853"
              d="M12 23c3.2 0 6-1.1 8-3l-3.7-2.9c-1.1.7-2.5 1.2-4.3 1.2-3 0-5.5-2.4-6.4-5.2L1.9 16c1.8 3.7 5.6 7 10.1 7z"
            />
          </svg>
          <span>Google</span>
        </button>
        <button
          type="button"
          className="bg-neutral-100 dark:bg-[#16161D] border border-neutral-200 dark:border-[#2B2B38] text-foreground text-[13px] font-medium rounded-lg px-4 py-3 flex items-center justify-center gap-2.5 transition-all hover:dark:bg-[#20202A] shadow-sm"
        >
          <svg className="size-4 fill-current shrink-0" viewBox="0 0 170 170">
            <path d="M150.37 130.25c-2.45 5.66-5.35 10.87-8.71 15.66-4.58 6.53-8.33 11.05-11.22 13.56-4.48 4.12-9.28 6.23-14.42 6.35-3.69 0-8.14-1.05-13.32-3.18-5.19-2.12-9.97-3.17-14.34-3.17-4.58 0-9.49 1.05-14.75 3.17-5.26 2.13-9.5 3.24-12.74 3.35-4.35.13-9.16-1.9-14.42-6.08-3.69-3.04-7.67-7.81-11.96-14.34-6.19-9.56-11.05-20.73-14.58-33.51-3.53-12.78-5.3-24.66-5.3-35.63 0-14.12 3.63-25.75 10.88-34.88 7.25-9.13 16.48-13.82 27.69-14.07 4.58 0 9.83 1.24 15.75 3.72 5.92 2.48 9.94 3.77 12.06 3.89 1.63 0 5.86-1.41 12.71-4.24 6.84-2.82 12.39-4.04 16.63-3.66 12.63 1.04 22.38 5.76 29.27 14.17-11.06 6.74-16.46 15.8-16.2 27.21.26 9.53 4 17.51 11.22 23.94 7.22 6.43 15.54 10.02 24.96 10.77-2.17 6.42-4.8 12.68-7.89 18.78zM119.22 31.84c0-7.39 2.65-14.28 7.94-20.67 5.3-6.39 11.77-10.45 19.42-12.17.65 1.52.98 3.12.98 4.79 0 7.39-2.77 14.36-8.31 20.91-5.54 6.55-12.21 10.49-20.03 11.83z" />
          </svg>
          <span>Apple</span>
        </button>
      </div>

      <div className="mt-7 text-center">
        <p className="text-xs text-neutral-500">
          {t("newToPlatform")}{" "}
          <Link
            href="/register"
            className="text-foreground font-semibold underline underline-offset-4 hover:text-brand-500 transition-colors ml-1.5"
          >
            {t("createAccount")}
          </Link>
        </p>
      </div>
    </div>
  );
}
