import { useTranslations } from "next-intl";
import Link from "next/link";
import { ArrowRight } from "lucide-react";

export function SpeakersCTA() {
  const t = useTranslations("speakers.cta");

  return (
    <section className="py-20 px-6 sm:px-12 bg-dark-carbon w-full border-b border-dark-border">
      <div className="max-w-7xl mx-auto animate-in fade-in slide-in-from-bottom-8 duration-1000">
        <div className="bg-dark-glass border border-dark-border rounded-3xl p-8 sm:p-14 relative overflow-hidden shadow-2xl">
          <div className="absolute top-0 inset-x-0 h-1 bg-linear-to-r from-transparent via-brand-500 to-transparent" />
          <div className="absolute right-0 top-1/2 -translate-y-1/2 w-96 h-96 bg-brand-500/10 rounded-full blur-[140px] pointer-events-none" />

          <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-center relative z-10">
            <div className="lg:col-span-8">
              <div className="flex items-center gap-3 mb-3">
                <div className="h-px w-6 bg-brand-500" />
                <span className="text-brand-500 font-mono text-xs font-bold tracking-[0.25em] uppercase">
                  {t("eyebrow")}
                </span>
              </div>
              <h2 className="font-heading text-3xl sm:text-4xl lg:text-5xl font-black uppercase tracking-tight text-white mb-4">
                {t("title")}
              </h2>
              <p className="text-sm sm:text-base text-neutral-400 font-light leading-relaxed max-w-2xl">
                {t("desc")}
              </p>
            </div>

            <div className="lg:col-span-4 flex flex-col sm:flex-row lg:flex-col gap-4 justify-end">
              <Link
                href="/contact"
                className="inline-flex items-center justify-center gap-2 text-sm font-bold uppercase tracking-wider text-white bg-brand-500 hover:bg-brand-glow py-4 px-8 rounded-full shadow-[0_0_25px_rgba(235,0,40,0.35)] hover:-translate-y-0.5 transition-all text-center"
              >
                <span>{t("primaryBtn")}</span>
                <ArrowRight className="size-3" />
              </Link>
              <Link
                href="/about"
                className="inline-flex items-center justify-center gap-2 text-xs font-mono font-bold uppercase tracking-wider text-neutral-300 hover:text-white py-3 px-6 rounded-full border border-dark-border hover:border-neutral-500 transition-all text-center"
              >
                <span>{t("secondaryBtn")}</span>
              </Link>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
