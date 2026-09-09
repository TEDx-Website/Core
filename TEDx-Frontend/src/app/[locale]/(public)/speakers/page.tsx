import { getTranslations } from "next-intl/server";
import { SpeakersHero } from "@/features/speakers/components/speakers-hero";
import { SpeakersGrid } from "@/features/speakers/components/speakers-grid";
import { SpeakersCTA } from "@/features/speakers/components/speakers-cta";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "speakers" });

  return {
    title: `Speakers | TEDxAlkawmia`,
  };
}

export default function SpeakersPage() {
  return (
    <main className="w-full overflow-hidden">
      <SpeakersHero />
      <SpeakersGrid />
      <SpeakersCTA />
    </main>
  );
}
