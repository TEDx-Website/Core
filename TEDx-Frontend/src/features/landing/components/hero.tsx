import { useTranslations } from "next-intl";
import Link from "next/link";
import { ArrowUpRight, Calendar, MapPin, Hourglass } from "lucide-react";
import { HeroSparks } from "./hero-sparks";

export function Hero() {
  const t = useTranslations("landing.Hero");

  return (
    <section className="relative min-h-screen flex items-center justify-center pt-20 pb-12 overflow-hidden bg-dark-obsidian">
      <HeroSparks />

      <div className="relative z-10 flex flex-col items-center text-center max-w-5xl mx-auto px-4 w-full animate-in fade-in slide-in-from-bottom-8 duration-1000">
        <span className="text-xs md:text-sm font-semibold tracking-[0.3em] text-neutral-400 uppercase mb-4 opacity-80">
          {t("eyebrow")}
        </span>

        <h1 className="font-bold text-6xl md:text-8xl lg:text-[9rem] leading-[0.9] tracking-tighter mb-6 text-white drop-shadow-2xl">
          {t("titleLine1")}
          <br />
          <span className="relative inline-block isolate">
            {t("titleLine2")}
            <span
              aria-hidden="true"
              className="absolute inset-x-[-2%] inset-y-[14%] bg-brand-500 blur-[46px] opacity-40 -z-10"
            />
          </span>
        </h1>

        <p className="max-w-2xl text-lg md:text-xl text-neutral-300 font-light mb-12 leading-relaxed">
          {t("description")}
        </p>

        <div className="glass-panel rounded-full p-1.5 flex flex-wrap justify-center items-center gap-2 md:gap-6 mb-12 text-sm md:text-base font-medium">
          <div className="flex items-center px-4 py-2 bg-dark-carbon/50 rounded-full border border-dark-border/50">
            <Calendar className="text-brand-500 mr-2 size-5" />
            <span>{t("date")}</span>
          </div>
          <div className="flex items-center px-4 py-2 bg-dark-carbon/50 rounded-full border border-dark-border/50">
            <MapPin className="text-brand-500 mr-2 size-5" />
            <span>{t("location")}</span>
          </div>
          <div className="flex items-center px-4 py-2 bg-dark-carbon/50 rounded-full border border-dark-border/50 text-brand-neon">
            <Hourglass className="mr-2 size-5 animate-pulse" />
            <span>{t("countdown")}</span>
          </div>
        </div>

        <div className="flex flex-col sm:flex-row gap-4 items-center">
          <Link
            href="#doors"
            className="group relative px-8 py-4 bg-brand-500 text-white font-semibold rounded-lg overflow-hidden transition-all hover:scale-105"
          >
            <div className="absolute inset-0 w-full h-full bg-linear-to-r from-brand-500 via-brand-glow to-brand-500 group-hover:animate-[gradient_2s_linear_infinite]" />
            <span className="relative flex items-center z-10">
              {t("ctaPrimary")}
              <ArrowUpRight className="ml-2 size-5 transition-transform group-hover:translate-x-1 group-hover:-translate-y-1" />
            </span>
          </Link>
          <Link
            href="#possibilities"
            className="px-8 py-4 text-white font-semibold rounded-lg border border-dark-border hover:bg-dark-glass transition-colors flex items-center"
          >
            {t("ctaSecondary")}
          </Link>
        </div>
      </div>

      <div className="absolute bottom-10 left-1/2 transform -translate-x-1/2 flex flex-col items-center opacity-50 hover:opacity-100 transition-opacity">
        <span className="text-[10px] tracking-widest uppercase mb-2 text-white">
          {t("scroll")}
        </span>
        <div className="w-px h-12 bg-linear-to-b from-white to-transparent overflow-hidden relative">
          <div className="w-full h-1/2 bg-white absolute top-0 left-0 animate-scroll-down" />
        </div>
      </div>
    </section>
  );
}
