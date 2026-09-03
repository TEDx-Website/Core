import { getTranslations } from "next-intl/server";
import { ProfileView } from "@/features/profile/components/profile-view";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "profile.layout" });

  return {
    title: `${t("title")} | TEDx Alkawmia`,
  };
}

export default function ProfilePage() {
  return (
    <main className="flex-1 max-w-4xl w-full mx-auto px-4 sm:px-6 py-10 sm:py-14">
      <ProfileView />
    </main>
  );
}
