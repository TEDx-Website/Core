import Image from "next/image";
import { useTranslations } from "next-intl";
import { Button } from "@/shared/ui/button";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowUpRight, Calendar, MapPin, Clock } from "lucide-react";

export function Hero() {
  const t = useTranslations("landing.Hero");

  return (
    <section className="relative w-full min-h-dvh flex items-center pt-25 md:pt-120px pb-70 md:pb-35 bg-neutral-950 overflow-hidden">
      <div className="absolute inset-0 z-0">
        <Image
          src="/assets/experience.webp"
          alt="Speaker Background"
          fill
          priority
          quality={100}
          className="object-cover object-top opacity-60 md:opacity-90"
        />
        <div className="absolute inset-0 bg-linear-to-r from-neutral-950 via-neutral-950/80 to-transparent" />
      </div>

      <GridContainer className="relative z-10 w-full">
        <div className="col-span-4 md:col-span-8 lg:col-span-7 flex flex-col gap-5 md:gap-6">
          <div className="flex flex-col gap-3">
            <div className="flex items-center gap-3 md:gap-4">
              <div className="w-8 md:w-10 h-1 bg-brand-500 shrink-0" />
              <span className="text-[10px] md:text-xs font-bold text-neutral-300 tracking-[0.2em] uppercase">
                {t("tagline")}
              </span>
            </div>

            <h1 className="text-[42px] leading-[0.95] sm:text-6xl md:text-7xl lg:text-[80px] lg:leading-[0.9] font-black text-white tracking-tighter uppercase">
              {t("title")}
            </h1>

            <h2 className="text-xl sm:text-2xl md:text-3xl lg:text-4xl font-bold text-white leading-tight whitespace-pre-line mt-1">
              {t("subtitle")}
            </h2>
          </div>

          <p className="text-sm md:text-base text-neutral-300 max-w-[480px] leading-relaxed">
            {t("description")}
          </p>

          <div className="flex flex-col sm:flex-row gap-3 md:gap-4 pt-2 md:pt-4 w-full sm:w-auto">
            <Button
              size="lg"
              className="w-full sm:w-auto bg-brand-500 hover:bg-brand-600 text-white rounded-full px-8 h-12 md:h-14 text-xs md:text-sm font-bold tracking-wide"
            >
              {t("primaryCta")}
              <ArrowUpRight className="ml-2 size-4" strokeWidth={3} />
            </Button>
            <Button
              size="lg"
              variant="outline"
              className="w-full sm:w-auto border-white text-white hover:bg-white hover:text-black rounded-full px-8 h-12 md:h-14 text-xs md:text-sm font-bold tracking-wide bg-transparent transition-colors"
            >
              {t("secondaryCta")}
            </Button>
          </div>
        </div>
      </GridContainer>

      <div className="absolute bottom-6 md:bottom-8 left-0 w-full z-20 px-4 md:px-[32px] lg:px-[80px]">
        <div className="mx-auto max-w-360">
          <div className="w-full bg-[#141414]/95 rounded-[24px] py-4 md:py-5 px-6 md:px-12 grid grid-cols-1 md:grid-cols-3 gap-6 md:gap-0 shadow-2xl">
            <div className="flex items-center gap-4">
              <Calendar
                className="text-brand-500 size-5 shrink-0"
                strokeWidth={2}
              />
              <div className="flex flex-col">
                <span className="text-[10px] font-bold text-neutral-400 uppercase tracking-widest mb-0.5">
                  {t("dateLabel")}
                </span>
                <span className="text-sm font-bold text-white">
                  {t("dateValue")}
                </span>
              </div>
            </div>

            <div className="flex items-center gap-4">
              <MapPin
                className="text-brand-500 size-5 shrink-0"
                strokeWidth={2}
              />
              <div className="flex flex-col">
                <span className="text-[10px] font-bold text-neutral-400 uppercase tracking-widest mb-0.5">
                  {t("venueLabel")}
                </span>
                <span className="text-sm font-bold text-white">
                  {t("venueValue")}
                </span>
              </div>
            </div>

            <div className="flex items-center gap-4">
              <Clock
                className="text-brand-500 size-5 shrink-0"
                strokeWidth={2}
              />
              <div className="flex flex-col">
                <span className="text-[10px] font-bold text-neutral-400 uppercase tracking-widest mb-0.5">
                  {t("doorsLabel")}
                </span>
                <span className="text-sm font-bold text-white">
                  {t("doorsValue")}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
