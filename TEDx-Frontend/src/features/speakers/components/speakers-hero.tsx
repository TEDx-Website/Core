import { useTranslations } from "next-intl";
import Image from "next/image";
import Link from "next/link";
import { Mic2, Ticket, IdCard } from "lucide-react";

export function SpeakersHero() {
  const t = useTranslations("speakers.hero");

  return (
    <section className="relative w-full min-h-screen overflow-hidden bg-dark-obsidian">
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <Image
          src="/assets/Tarek.jpeg"
          alt={t("nameLine1")}
          fill
          priority
          className="object-cover object-[75%_15%] max-lg:object-[65%_10%] grayscale-[15%] contrast-110 scale-[1.02] transition-all duration-[1200ms] ease-in-out hover:scale-105 hover:grayscale-0 hover:contrast-[1.15]"
        />
        <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(8,8,8,0.70)_0%,rgba(8,8,8,0.88)_50%,#080808_100%)] lg:bg-[linear-gradient(to_right,#080808_0%,rgba(8,8,8,0.96)_28%,rgba(8,8,8,0.75)_52%,rgba(8,8,8,0.25)_75%,rgba(8,8,8,0.05)_100%)]" />
        <div className="absolute inset-0 bg-linear-to-t from-dark-obsidian via-transparent to-transparent" />
        <div className="absolute inset-0 bg-[radial-gradient(ellipse_60%_80%_at_75%_40%,rgba(235,0,40,0.24)_0%,transparent_70%)] mix-blend-screen" />
      </div>

      <div className="absolute inset-0 bg-brand-500/5 blur-[200px] pointer-events-none top-[20%] left-0 w-[40%] h-[60%] rounded-full bg-[radial-gradient(ellipse,rgba(235,0,40,0.12)_0%,transparent_70%)]" />

      <div className="relative z-10 w-full min-h-screen flex flex-col justify-start pt-24 animate-in fade-in slide-in-from-left-8 duration-1000">
        <div className="max-w-7xl mx-auto px-6 sm:px-12 w-full pt-10 pb-16">
          <div className="max-w-xl lg:max-w-2xl">
            <div className="inline-flex items-center gap-2 px-4 py-1.5 rounded-full border border-brand-500/45 bg-brand-500/10 font-mono text-[11px] font-bold tracking-[0.18em] uppercase text-brand-neon mb-6 backdrop-blur-md">
              <div className="w-1.5 h-1.5 bg-brand-500 rounded-full animate-ping" />
              {t("badge")}
            </div>

            <div className="font-heading text-[clamp(2.4rem,5.5vw,4.4rem)] font-black leading-none tracking-tighter uppercase text-white mb-2.5">
              {t("nameLine1")}
              <br />
              {t("nameLine2")}
            </div>

            <div className="font-mono text-[13.6px] font-medium text-neutral-400 tracking-wide mb-7">
              {t("role")}
            </div>

            <div className="w-[min(360px,60%)] h-px bg-linear-to-r from-brand-500 to-brand-500/20 mb-7" />

            <div className="font-mono text-[10px] font-bold tracking-[0.2em] uppercase text-brand-500 mb-3 flex items-center">
              <Mic2 className="size-3.5 mr-1.5" />
              {t("topicLabel")}
            </div>

            <div className="font-heading text-[clamp(1.15rem,2.2vw,1.45rem)] font-bold text-white leading-snug max-w-[580px] mb-5 italic">
              {t("talkTitle")}
            </div>

            <div className="text-sm text-neutral-400 font-light leading-relaxed max-w-[560px] mb-8">
              {t("bio")}
            </div>

            <div className="flex flex-wrap items-center gap-4">
              <Link
                href="/contact"
                className="inline-flex items-center gap-2 px-7 py-3 bg-brand-500 text-white text-xs font-bold tracking-[0.08em] uppercase rounded-full shadow-[0_0_24px_rgba(235,0,40,0.45)] hover:bg-brand-glow hover:shadow-[0_0_38px_rgba(235,0,40,0.65)] hover:-translate-y-px transition-all"
              >
                <Ticket className="size-3.5" />
                <span>{t("primaryBtn")}</span>
              </Link>
              <button className="inline-flex items-center gap-2 px-6 py-3 bg-white/5 text-neutral-400 font-mono text-xs font-semibold tracking-[0.06em] uppercase rounded-full border border-white/10 hover:bg-white/10 hover:text-white hover:border-white/25 transition-all">
                <IdCard className="size-3.5" />
                <span>{t("secondaryBtn")}</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <div className="absolute top-[80%] right-12 -translate-y-1/2 font-heading text-[clamp(5rem,10vw,9rem)] font-black leading-none tracking-[0.06em] uppercase text-brand-500/[0.09] select-none pointer-events-none z-1 hover:text-brand-500/[0.16] transition-colors duration-500">
        {t("watermark")}
      </div>
    </section>
  );
}
