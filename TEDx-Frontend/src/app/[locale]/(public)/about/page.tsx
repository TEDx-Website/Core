import { TeamHero } from "@/features/about/components/team-hero";
import { LeaderSection } from "@/features/about/components/leader-section";
import { CuratorsSection } from "@/features/about/components/curators-section";
import { ProductionSection } from "@/features/about/components/production-section";
import { TeamCta } from "@/features/about/components/team-cta";
import { getTranslations } from "next-intl/server";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "about.metadata" });

  return {
    title: t("title"),
    description: t("description"),
    openGraph: {
      title: t("title"),
      description: t("description"),
      type: "website",
      images: ["/assets/og-team.jpg"],
    },
  };
}

export default function TeamPage() {
  return (
    <main className="w-full overflow-hidden">
      <TeamHero />
      <LeaderSection />
      <CuratorsSection />
      <ProductionSection />
      <TeamCta />
    </main>
  );
}
