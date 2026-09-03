"use client";

import Image from "next/image";
import { usePathname } from "next/navigation";
import { useTranslations } from "next-intl";

export function AuthBrandCanvas() {
  const pathname = usePathname();
  const t = useTranslations("auth.canvas");

  const isRegisterFlow =
    pathname.includes("register") || pathname.includes("confirm-email");

  return (
    <div className="hidden lg:flex lg:col-span-6 xl:col-span-6 relative flex-col justify-between p-12 xl:p-16 overflow-hidden bg-[#050507]">
      <div className="absolute inset-0 z-0 bg-[#050507]">
        <Image
          src="/assets/login.webp" 
          alt="Login Canvas"
          fill
          priority
          sizes="50vw"
          className={`object-cover object-center grayscale contrast-125 mix-blend-luminosity transition-opacity duration-700 ease-in-out ${
            isRegisterFlow ? "opacity-0" : "opacity-30"
          }`}
        />
        <Image
          src="/assets/signup.webp" 
          alt="Register Canvas"
          fill
          priority
          sizes="50vw"
          className={`object-cover object-center grayscale contrast-125 mix-blend-luminosity transition-opacity duration-700 ease-in-out ${
            isRegisterFlow ? "opacity-30" : "opacity-0"
          }`}
        />

        <div className="absolute inset-0 bg-linear-to-r from-transparent via-background/60 to-background"></div>
        <div className="absolute inset-0 bg-linear-to-t from-background via-transparent to-background/70"></div>
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_left,var(--color-brand-500)_0%,transparent_40%)] opacity-15"></div>
      </div>

      <div className="relative z-10 min-h-11"></div>

      <div className="relative z-10 max-w-2xl my-auto py-4 -translate-y-8 xl:-translate-y-12">
        <h1 className="font-black text-4xl sm:text-5xl xl:text-6xl text-foreground leading-[1.08] tracking-tight mb-6 transition-all duration-300">
          <span className="block">
            {isRegisterFlow ? t("registerHeading1") : t("loginHeading1")}
          </span>
          <span className="block text-neutral-400 font-light">
            {isRegisterFlow ? t("registerHeading2") : t("loginHeading2")}
          </span>
        </h1>

        <p className="text-base xl:text-lg text-neutral-300 leading-relaxed font-normal max-w-lg transition-all duration-300">
          {isRegisterFlow ? t("registerSubtext") : t("loginSubtext")}
        </p>
      </div>

      <div className="relative z-10">
        <span className="text-xs text-neutral-500 font-mono tracking-widest uppercase">
          {t("trademark")}
        </span>
      </div>
    </div>
  );
}
