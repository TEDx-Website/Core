import { getTranslations } from "next-intl/server";
import { ForgotPasswordForm } from "@/features/auth/components/forgot-password-form";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "auth.forgotPassword" });

  return {
    title: `${t("title")} | TEDx Alkawmia`,
  };
}

export default function ForgotPasswordPage() {
  return <ForgotPasswordForm />;
}
