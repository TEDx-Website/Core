"use client";

import { useTranslations } from "next-intl";
import { useMemo } from "react";

interface PasswordStrengthProps {
  password?: string;
}

export function PasswordStrength({ password = "" }: PasswordStrengthProps) {
  const t = useTranslations("auth.errors");

  const score = useMemo(() => {
    let s = 0;
    if (password.length >= 8) s++;
    if (/[A-Z]/.test(password)) s++;
    if (/[a-z]/.test(password)) s++;
    if (/[0-9]/.test(password)) s++;
    return s;
  }, [password]);

  const getIndicatorClass = (index: number) => {
    if (index > score) return "bg-neutral-200 dark:bg-[#1E1E26]";
    if (score >= 4) return "bg-emerald-400";
    if (score === 3) return "bg-amber-400";
    return "bg-red-500";
  };

  return (
    <div className="mt-2 space-y-1.5">
      <div className="grid grid-cols-4 gap-1.5">
        {[1, 2, 3, 4].map((index) => (
          <div
            key={index}
            className={`h-1 rounded-full transition-all duration-200 ${getIndicatorClass(index)}`}
          />
        ))}
      </div>
      <p className="text-[11px] text-neutral-500 dark:text-[#A1A1AA] leading-normal">
        {t("weakPassword")}
      </p>
    </div>
  );
}
