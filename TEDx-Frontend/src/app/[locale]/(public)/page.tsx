import { Navbar } from "@/shared/layout/navbar";
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
import { Footer } from "@/shared/layout/footer";

export default function LandingPage() {
  return (
    <>
      <Navbar />
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
      <Footer />
    </>
  );
}
