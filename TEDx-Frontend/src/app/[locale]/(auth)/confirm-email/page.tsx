import { getTranslations } from "next-intl/server";
import { ConfirmEmailView } from "@/features/auth/components/confirm-email-view";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "auth.confirmEmail" });

  return {
    title: `${t("title")} | TEDx Alkawmia`,
  };
}

export default async function ConfirmEmailPage({
  searchParams,
}: {
  searchParams: Promise<{ userId?: string; token?: string }>;
}) {
  const resolvedSearchParams = await searchParams;
  const userId = resolvedSearchParams.userId || "";
  const token = resolvedSearchParams.token || "";

  return <ConfirmEmailView userId={userId} token={token} />;
}
