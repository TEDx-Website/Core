import Image from "next/image";
import { useTranslations } from "next-intl";
import { Button } from "@/shared/ui/button";
import { GridContainer } from "@/shared/ui/grid-container";
import { Sparkles } from "lucide-react";

export function Experience() {
  const t = useTranslations("landing.Experience");

  return (
    <section className="w-full py-[80px] md:py-120px bg-neutral-100 dark:bg-neutral-1000 transition-colors duration-300">
      <GridContainer className="items-center gap-y-12">
        {/* Image Column */}
        <div className="col-span-4 md:col-span-4 lg:col-span-6 relative w-full aspect-4/3 lg:aspect-square rounded-[24px] overflow-hidden shadow-2xl">
          <Image
            src="/assets/experience.webp"
            alt="TEDx Experience"
            fill
            sizes="(max-width: 768px) 100vw, 50vw"
            className="object-cover"
          />
        </div>

        {/* Content Column */}
        <div className="col-span-4 md:col-span-4 lg:col-span-5 lg:col-start-8 flex flex-col gap-8 md:py-8">
          <div className="flex flex-col gap-4">
            <div className="flex items-center gap-2">
              <Sparkles className="size-4 text-brand-600" strokeWidth={2.5} />
              <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
                {t("tagline")}
              </span>
            </div>

            <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-foreground tracking-tight whitespace-pre-line leading-[1.1]">
              {t("title")}
            </h2>

            <p className="text-base md:text-lg text-neutral-600 dark:text-neutral-400 leading-relaxed mt-2">
              {t("description")}
            </p>
          </div>

          <div className="grid grid-cols-3 gap-4 pt-6 border-t border-neutral-200 dark:border-neutral-800">
            <div className="flex flex-col gap-1">
              <span className="text-3xl md:text-4xl font-black text-foreground">
                {t("stats.speakers.value")}
              </span>
              <span className="text-[10px] md:text-xs font-bold tracking-widest text-neutral-500 uppercase">
                {t("stats.speakers.label")}
              </span>
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-3xl md:text-4xl font-black text-foreground">
                {t("stats.attendees.value")}
              </span>
              <span className="text-[10px] md:text-xs font-bold tracking-widest text-neutral-500 uppercase">
                {t("stats.attendees.label")}
              </span>
            </div>
            <div className="flex flex-col gap-1">
              <span className="text-3xl md:text-4xl font-black text-foreground">
                {t("stats.sharedDay.value")}
              </span>
              <span className="text-[10px] md:text-xs font-bold tracking-widest text-neutral-500 uppercase">
                {t("stats.sharedDay.label")}
              </span>
            </div>
          </div>

          <Button className="w-full bg-brand-500 hover:bg-brand-600 text-white rounded-full h-12 text-sm font-bold tracking-widest uppercase mt-4">
            {t("cta")}
          </Button>
        </div>
      </GridContainer>
    </section>
  );
}
