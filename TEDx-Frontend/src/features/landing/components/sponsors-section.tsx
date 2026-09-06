"use client";

import { useTranslations } from "next-intl";
import Link from "next/link";
import { Key, Download, Mail } from "lucide-react";

const SPONSORS_DATA = {
  tier1: [
    {
      name: "NILEWORKS",
      className: "font-display font-extrabold tracking-tight",
    },
    {
      name: "Kawmia Bank",
      className: "font-mono font-medium uppercase tracking-[0.2em]",
    },
  ],
  tier2: [
    { name: "Orbit Media", className: "font-bold tracking-tight" },
    { name: "Helio", className: "font-mono uppercase tracking-[0.1em]" },
    { name: "Masriya", className: "font-bold tracking-tight" },
    { name: "Atlas Press", className: "font-mono uppercase tracking-[0.1em]" },
  ],
  tier3: [
    { name: "Delta Roast", className: "font-mono uppercase" },
    { name: "Sahl", className: "font-semibold" },
    { name: "Lumen Labs", className: "font-mono uppercase" },
    { name: "Riwaya", className: "font-semibold" },
    { name: "Qafla", className: "font-mono uppercase" },
    { name: "Noor Studios", className: "font-semibold" },
  ],
};

export function SponsorsSection() {
  const t = useTranslations("landing.sponsors");

  return (
    <section
      id="sponsors"
      className="py-24 md:py-28 relative overflow-hidden border-t border-dark-border/50 bg-dark-obsidian"
    >
      <div className="absolute top-0 left-1/3 w-[45vw] h-[45vw] bg-brand-500/4 rounded-full blur-[160px] -z-10 pointer-events-none" />

      <div className="max-w-7xl mx-auto px-6">
        <div className="flex flex-col md:flex-row justify-between items-start md:items-end mb-14 animate-in fade-in slide-in-from-bottom-8 duration-1000">
          <div>
            <div className="flex items-center gap-3 mb-5">
              <span className="w-2 h-2 rounded-full bg-brand-500 animate-pulse shrink-0" />
              <span className="text-[10px] font-bold tracking-[0.28em] text-brand-500 uppercase">
                {t("eyebrow")}
              </span>
            </div>
            <h2 className="font-bold text-4xl md:text-[3.25rem] leading-[1.05] mb-4 text-white">
              {t("titleLine1")}
              <br />
              <span className="text-neutral-400">{t("titleLine2")}</span>
            </h2>
            <p className="text-neutral-400 text-base max-w-xl font-light leading-relaxed">
              {t("description")}
            </p>
          </div>
          <Link
            href="#become-partner"
            className="mt-8 md:mt-0 shrink-0 inline-flex items-center gap-2 px-5 py-2.5 rounded-full border border-dark-border bg-dark-carbon hover:bg-brand-500/10 hover:border-brand-500 text-xs font-bold text-neutral-300 hover:text-white transition-all tracking-wide"
          >
            <Key className="text-brand-500 size-4" />
            {t("becomeKeyholder")}
          </Link>
        </div>

        <div className="flex flex-col mb-20 animate-in fade-in duration-1000 delay-150">
          <div className="grid grid-cols-1 lg:grid-cols-[200px_1fr] gap-4 lg:gap-12 py-8 border-t border-dark-border">
            <div className="flex items-baseline gap-2 font-mono text-[10px] font-medium tracking-[0.2em] uppercase text-neutral-500 pt-1">
              <span className="text-brand-500">01</span> {t("tier1")}
            </div>
            <div className="flex flex-wrap items-baseline gap-x-8 md:gap-x-16 gap-y-6">
              {SPONSORS_DATA.tier1.map((sponsor, idx) => (
                <Link
                  key={idx}
                  href="#"
                  className="group relative inline-flex items-center pb-2 text-[clamp(1.25rem,2.1vw,1.75rem)] text-white/85 hover:text-white transition-opacity after:absolute after:bottom-0 after:left-0 after:h-px after:w-0 after:bg-brand-500 hover:after:w-full after:transition-all after:duration-300"
                >
                  <span className={sponsor.className}>{sponsor.name}</span>
                </Link>
              ))}
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-[200px_1fr] gap-4 lg:gap-12 py-8 border-t border-dark-border">
            <div className="flex items-baseline gap-2 font-mono text-[10px] font-medium tracking-[0.2em] uppercase text-neutral-500 pt-1">
              <span className="text-brand-500">02</span> {t("tier2")}
            </div>
            <div className="flex flex-wrap items-baseline gap-x-8 md:gap-x-16 gap-y-6">
              {SPONSORS_DATA.tier2.map((sponsor, idx) => (
                <Link
                  key={idx}
                  href="#"
                  className="group relative inline-flex items-center pb-2 text-[clamp(1rem,1.35vw,1.1875rem)] text-white/65 hover:text-white transition-opacity after:absolute after:bottom-0 after:left-0 after:h-px after:w-0 after:bg-brand-500 hover:after:w-full after:transition-all after:duration-300"
                >
                  <span className={sponsor.className}>{sponsor.name}</span>
                </Link>
              ))}
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-[200px_1fr] gap-4 lg:gap-12 py-8 border-y border-dark-border">
            <div className="flex items-baseline gap-2 font-mono text-[10px] font-medium tracking-[0.2em] uppercase text-neutral-500 pt-1">
              <span className="text-brand-500">03</span> {t("tier3")}
            </div>
            <div className="flex flex-wrap items-baseline gap-x-6 md:gap-x-10 gap-y-4">
              {SPONSORS_DATA.tier3.map((sponsor, idx) => (
                <Link
                  key={idx}
                  href="#"
                  className="group relative inline-flex items-center pb-1.5 text-[0.875rem] text-white/50 hover:text-white transition-opacity after:absolute after:bottom-0 after:left-0 after:h-px after:w-0 after:bg-brand-500 hover:after:w-full after:transition-all after:duration-300"
                >
                  <span className={sponsor.className}>{sponsor.name}</span>
                </Link>
              ))}
            </div>
          </div>
        </div>

        <div
          id="become-partner"
          className="glass-panel rounded-2xl p-8 md:p-11 relative overflow-hidden animate-in fade-in slide-in-from-bottom-8 duration-1000 delay-300"
        >
          <div className="absolute -top-24 -right-16 w-95 h-95 bg-brand-500/10 rounded-full blur-[110px] pointer-events-none" />
          <div className="absolute top-0 left-0 w-5 h-5 border-t-2 border-l-2 border-white/20" />
          <div className="absolute bottom-0 right-0 w-5 h-5 border-b-2 border-r-2 border-white/20" />

          <div className="relative z-10 flex flex-col lg:flex-row gap-10 items-start lg:items-center justify-between">
            <div className="max-w-xl">
              <h3 className="text-2xl md:text-3xl font-bold mb-3 leading-tight text-white">
                {t("ctaTitle1")}
                <br className="hidden md:block" /> {t("ctaTitle2")}
              </h3>
              <p className="text-neutral-400 font-light leading-relaxed">
                {t("ctaDesc")}
              </p>
            </div>

            <div className="flex flex-col sm:flex-row lg:flex-col gap-3 w-full lg:w-auto shrink-0">
              <Link
                href="#"
                className="inline-flex items-center justify-center gap-2 px-7 py-3.5 rounded-full bg-brand-500 text-white text-sm font-bold hover:bg-brand-glow shadow-[0_0_24px_rgba(235,0,40,0.4)] hover:-translate-y-0.5 transition-all whitespace-nowrap"
              >
                <Download className="size-4" />
                {t("deckBtn")}
              </Link>
              <Link
                href="mailto:partners@tedxalkawmia.com"
                className="inline-flex items-center justify-center gap-2 px-7 py-3.5 rounded-full border border-dark-border text-white text-sm font-bold hover:border-brand-500 hover:bg-brand-500/10 transition-all whitespace-nowrap"
              >
                <Mail className="size-4" />
                {t("contactBtn")}
              </Link>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
