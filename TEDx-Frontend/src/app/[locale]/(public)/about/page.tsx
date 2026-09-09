import { getTranslations } from "next-intl/server";
import { AboutHero } from "@/features/about/components/about-hero";
import { AboutTheme } from "@/features/about/components/about-theme";
import { AboutPrinciples } from "@/features/about/components/about-principles";
import { AboutTed } from "@/features/about/components/about-ted";
import { AboutTimeline } from "@/features/about/components/about-timeline";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "about" });

  return {
    title: `About Us | TEDxAlkawmia`,
  };
}

export default function AboutPage() {
  return (
    <main className="w-full overflow-hidden">
      <AboutHero />
      <AboutTheme />
      <AboutPrinciples />
      <AboutTed />
      <AboutTimeline />
    </main>
  );
}
