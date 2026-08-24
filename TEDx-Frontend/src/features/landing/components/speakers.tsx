import Image from "next/image";
import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowRight } from "lucide-react";

export function Speakers() {
  const t = useTranslations("landing.Speakers");
  const speakers = ["speaker1", "speaker2", "speaker3"] as const;

  return (
    <section
      id="speakers"
      className="w-full py-[80px] md:py-[120px] bg-background transition-colors duration-300"
    >
      <GridContainer>
        <div className="col-span-4 md:col-span-8 lg:col-span-12 flex flex-col md:flex-row md:items-end justify-between gap-6 mb-12">
          <div className="flex flex-col gap-3 max-w-2xl">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
              {t("tagline")}
            </span>
            <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-foreground tracking-tight">
              {t("title")}
            </h2>
          </div>
          <Link
            href="/speakers"
            className="group flex items-center gap-2 text-sm font-bold text-brand-600 hover:text-brand-700 transition-colors uppercase tracking-widest pb-1 md:pb-2"
          >
            {t("viewAll")}
            <ArrowRight
              className="size-4 group-hover:translate-x-1 transition-transform"
              strokeWidth={2.5}
            />
          </Link>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-12 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {speakers.map((speaker, index) => (
            <div key={speaker} className="flex flex-col group">
              <div className="relative w-full aspect-[4/3] rounded-[24px] overflow-hidden bg-neutral-100 dark:bg-neutral-800 mb-5 shadow-sm group-hover:shadow-md transition-shadow duration-300">
                <Image
                  src={`/assets/speaker-${index + 1}.jpg`}
                  alt={t(`cards.${speaker}.name`)}
                  fill
                  sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
                  className="object-cover group-hover:scale-105 transition-transform duration-700 ease-out"
                />
              </div>

              <div className="flex flex-col gap-1">
                <h3 className="text-xl font-bold text-foreground">
                  {t(`cards.${speaker}.name`)}
                </h3>
                <p className="text-sm text-neutral-500 dark:text-neutral-400">
                  {t(`cards.${speaker}.role`)}
                </p>
              </div>
            </div>
          ))}
        </div>
      </GridContainer>
    </section>
  );
}
