import { Hero } from "@/features/landing/components/hero";
import { AboutSection } from "@/features/landing/components/about-section";
import { DoorsSection } from "@/features/landing/components/doors-section";
import { SpeakersSection } from "@/features/landing/components/speakers-section";
import { SponsorsSection } from "@/features/landing/components/sponsors-section";
import { TeamSection } from "@/features/landing/components/team-section";
// import { FeaturesGrid } from "@/features/landing/components/features-grid";
// import { Tracks } from "@/features/landing/components/tracks";
// import { BookingSteps } from "@/features/landing/components/booking-steps";
// import { StatsQuote } from "@/features/landing/components/stats-quote";
import { FaqSection } from "@/features/landing/components/faq-section";
// import { CtaBanner } from "@/features/landing/components/cta-banner";
// import { Newsletter } from "@/features/landing/components/newsletter";
import { getTranslations } from "next-intl/server";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "landing.metadata" });

  return {
    title: t("title"),
    description: t("description"),
    openGraph: {
      title: t("title"),
      description: t("description"),
      type: "website",
      images: ["/assets/og-home.jpg"],
    },
    twitter: {
      card: "summary_large_image",
    },
  };
}

export default function LandingPage() {
  return (
    <>
      <main className="w-full">
        <Hero />
        <AboutSection />
        <DoorsSection />
        <SpeakersSection />
        <SponsorsSection />
        <TeamSection />
        <FaqSection />

        {/*<FeaturesGrid />
        <Tracks />
        <BookingSteps />
        <StatsQuote />
        <CtaBanner />
        <Newsletter /> */}
      </main>
    </>
  );
}
