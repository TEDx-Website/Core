"use client";

import { useEffect, useState } from "react";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { Loader2, ArrowRight, CheckCircle2, XCircle } from "lucide-react";
import { useConfirmEmail } from "../api/auth.hooks";

interface ConfirmEmailViewProps {
  userId: string;
  token: string;
}

export function ConfirmEmailView({ userId, token }: ConfirmEmailViewProps) {
  const t = useTranslations("auth.confirmEmail");
  const [status, setStatus] = useState<"loading" | "success" | "error">(
    "loading",
  );

  const { mutate: confirmEmailMutation } = useConfirmEmail();

  useEffect(() => {
    if (!userId || !token) {
      const error =() => {
      setStatus("error");
      }
      error();
      return;
    }

    confirmEmailMutation(
      { userId, token },
      {
        onSuccess: (response) => {
          if (response.success) {
            setStatus("success");
          } else {
            setStatus("error");
          }
        },
        onError: () => {
          setStatus("error");
        },
      },
    );
  }, [userId, token, confirmEmailMutation]);

  return (
    <div className="w-full text-center">
      {status === "loading" && (
        <div className="flex flex-col items-center justify-center space-y-4 py-8">
          <Loader2 className="size-12 animate-spin text-brand-500" />
          <h2 className="font-black text-2xl text-foreground tracking-tight">
            {t("verifying")}
          </h2>
          <p className="text-xs text-neutral-500 dark:text-neutral-400">
            {t("verifyingDesc")}
          </p>
        </div>
      )}

      {status === "success" && (
        <div className="flex flex-col items-center">
          <div className="w-14 h-14 rounded-2xl bg-emerald-950/40 border border-emerald-500/40 text-emerald-400 flex items-center justify-center mx-auto mb-5 shadow-lg">
            <CheckCircle2 className="size-8" />
          </div>

          <h2 className="font-black text-2xl sm:text-3xl text-foreground tracking-tight mb-2.5">
            {t("successTitle")}
          </h2>

          <p className="text-xs sm:text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed mb-6 max-w-sm mx-auto">
            {t("successDesc")}
          </p>

          <Link
            href="/login"
            className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-lg px-5 py-3.5 flex items-center justify-center gap-2 transition-all shadow-md"
          >
            <span>{t("signInBtn")}</span>
            <ArrowRight className="size-4" />
          </Link>
        </div>
      )}

      {status === "error" && (
        <div className="flex flex-col items-center">
          <div className="w-14 h-14 rounded-2xl bg-red-950/40 border border-red-500/40 text-red-400 flex items-center justify-center mx-auto mb-5 shadow-lg">
            <XCircle className="size-8" />
          </div>

          <h2 className="font-black text-2xl sm:text-3xl text-foreground tracking-tight mb-2.5">
            {t("errorTitle")}
          </h2>

          <p className="text-xs sm:text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed mb-6 max-w-sm mx-auto">
            {t("errorDesc")}
          </p>

          <div className="space-y-3 w-full">
            <Link
              href="/login"
              className="w-full bg-brand-500 hover:bg-brand-600 text-white font-semibold text-sm rounded-lg px-5 py-3.5 flex items-center justify-center gap-2 transition-all shadow-md"
            >
              <span>{t("signInBtn")}</span>
            </Link>
          </div>
        </div>
      )}
    </div>
  );
}
