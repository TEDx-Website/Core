import Link from "next/link";
import { useTranslations } from "next-intl";
import { ArrowLeft } from "lucide-react";
import { AuthBrandCanvas } from "@/features/auth/components/auth-brand-canvas";

export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const t = useTranslations("auth.layout");

  return (
    <main className="min-h-screen grid grid-cols-1 lg:grid-cols-12 bg-background">
      <AuthBrandCanvas />

      <div className="lg:col-span-6 xl:col-span-6 flex flex-col justify-between p-6 sm:p-12 xl:p-16 min-h-screen">
        <div className="flex items-center justify-between w-full min-h-11 mb-8">
          <div></div>
          <Link
            href="/"
            className="text-xs font-medium text-foreground hover:text-brand-500 transition-colors flex items-center gap-1.5 px-4 py-2 rounded-lg border border-neutral-200 dark:border-neutral-800 bg-neutral-100 dark:bg-neutral-900 hover:border-brand-500/50 shadow-sm"
          >
            <ArrowLeft className="size-3.5" />
            <span>{t("returnToEvent")}</span>
          </Link>
        </div>

        <div className="w-full max-w-110 mx-auto my-auto py-6">
          {children}
        </div>

        <div className="w-full pt-6 border-t border-neutral-200 dark:border-neutral-800 flex flex-col sm:flex-row justify-between items-center text-xs text-neutral-500 gap-3">
          <span>{t("copyright")}</span>
          <div className="flex items-center gap-3.5 text-neutral-400">
            <Link
              href="#"
              className="hover:text-foreground transition-colors hover:underline underline-offset-2"
            >
              {t("privacy")}
            </Link>
            <span>&bull;</span>
            <Link
              href="#"
              className="hover:text-foreground transition-colors hover:underline underline-offset-2"
            >
              {t("terms")}
            </Link>
            <span>&bull;</span>
            <Link
              href="/confirm-email"
              className="hover:text-brand-400 transition-colors hover:underline underline-offset-2 text-brand-500"
            >
              {t("confirmEmail")}
            </Link>
          </div>
        </div>
      </div>
    </main>
  );
}
