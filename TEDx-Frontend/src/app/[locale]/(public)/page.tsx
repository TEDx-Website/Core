import { Hero } from "@/features/landing/components/hero";
import { UpcomingEvents } from "@/features/landing/components/upcoming-events";
import { Experience } from "@/features/landing/components/experience";
import { Platform } from "@/features/landing/components/platform";
import { TeamSection } from "@/features/landing/components/team-section";
import { Speakers } from "@/features/landing/components/speakers";
import { FeaturesGrid } from "@/features/landing/components/features-grid";
import { Tracks } from "@/features/landing/components/tracks";
import { BookingSteps } from "@/features/landing/components/booking-steps";
import { StatsQuote } from "@/features/landing/components/stats-quote";
import { FaqSection } from "@/features/landing/components/faq-section";
import { CtaBanner } from "@/features/landing/components/cta-banner";
import { Newsletter } from "@/features/landing/components/newsletter";
import { getTranslations } from "next-intl/server";

export async function generateMetadata({ params }: { params: Promise<{ locale: string }> }) {
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
    }
  };
}

export default function LandingPage() {
  return (
    <>
      <main className="w-full">
        <Hero />
        <UpcomingEvents />
        <Experience />
        <Platform />
        <TeamSection />
        <Speakers />
        <FeaturesGrid />
        <Tracks />
        <BookingSteps />
        <StatsQuote />
        <FaqSection />
        <CtaBanner />
        <Newsletter />
      </main>
    </>
  );
}
