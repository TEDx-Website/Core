import Image from "next/image";
import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowRight } from "lucide-react";

export function UpcomingEvents() {
  const t = useTranslations("landing.UpcomingEvents");
  const cards = ["card1", "card2", "card3"] as const;

  return (
    <section
      id="events"
      className="w-full py-[80px] md:py-120px bg-background transition-colors duration-300"
    >
      <GridContainer>
        <div className="col-span-4 md:col-span-8 lg:col-span-12 flex flex-col md:flex-row md:items-end justify-between gap-6 mb-12">
          <div className="flex flex-col gap-3">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
              {t("tagline")}
            </span>
            <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-foreground tracking-tight">
              {t("title")}
            </h2>
            <p className="text-base md:text-lg text-neutral-500 dark:text-neutral-400 mt-1">
              {t("subtitle")}
            </p>
          </div>
          <Link 
            href="/events"
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
          {cards.map((card, index) => (
            <div
              key={card}
              className="flex flex-col bg-white dark:bg-neutral-900 rounded-[24px] shadow-[0_8px_30px_rgb(0,0,0,0.06)] dark:shadow-none dark:border dark:border-neutral-800 p-4 md:p-5 group hover:shadow-[0_8px_30px_rgb(0,0,0,0.12)] transition-all duration-300"
            >
              <div className="relative w-full aspect-video rounded-[16px] overflow-hidden bg-neutral-100 dark:bg-neutral-800">
                <Image
                  src={`/assets/event-${index + 1}.jpg`}
                  alt={t(`cards.${card}.title`)}
                  fill
                  sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
                  className="object-cover group-hover:scale-105 transition-transform duration-700 ease-out"
                />
              </div>

              <div className="flex flex-col flex-1 px-2 pt-6">
                <div className="flex flex-col gap-2 flex-1">
                  <span className="text-xs font-bold tracking-widest text-brand-600 uppercase">
                    {t(`cards.${card}.meta`)}
                  </span>
                  <h3 className="text-xl md:text-2xl font-bold text-foreground leading-tight">
                    {t(`cards.${card}.title`)}
                  </h3>
                  <p className="text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed mt-1">
                    {t(`cards.${card}.description`)}
                  </p>
                </div>

                <div className="mt-6 border-t border-neutral-100 dark:border-neutral-800 pt-5 pb-1">
                  <Link
                    href={`/events/${card}`}
                    className="inline-flex items-center gap-2 text-xs font-bold text-brand-600 hover:text-brand-700 transition-colors uppercase tracking-widest group/link"
                  >
                    {t(`cards.${card}.cta`)}
                    <ArrowRight
                      className="size-4 group-hover/link:translate-x-1 transition-transform"
                      strokeWidth={2.5}
                    />
                  </Link>
                </div>
              </div>
            </div>
          ))}
        </div>
      </GridContainer>
    </section>
  );
}
