import { useTranslations } from "next-intl";
import Image from "next/image";

export function AboutHero() {
  const t = useTranslations("about.hero");

  return (
    <section className="relative min-h-[85vh] lg:min-h-[88vh] flex items-center py-30 lg:py-40 px-6 sm:px-12 bg-dark-obsidian w-full border-b border-dark-border/50 overflow-hidden">
      <div className="absolute right-0 top-1/2 -translate-y-1/2 w-[45vw] h-[45vw] max-w-137.5 max-h-137.5 bg-brand-500/10 blur-[150px] rounded-full pointer-events-none" />
      <div className="absolute left-0 top-1/4 w-[30vw] h-[30vw] max-w-87.5 max-h-87.5 bg-brand-500/5 blur-[120px] rounded-full pointer-events-none" />

      <div className="max-w-7xl mx-auto w-full flex flex-col lg:flex-row items-center gap-12 lg:gap-16 relative z-10 animate-in fade-in slide-in-from-bottom-8 duration-1000">
        <div className="w-full lg:w-1/2 flex flex-col justify-center">
          <div className="flex items-center gap-4 mb-5">
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
            <span className="text-brand-500 text-xs font-bold tracking-[0.25em] uppercase">
              {t("eyebrow")}
            </span>
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
          </div>
          <h1 className="text-4xl sm:text-5xl lg:text-6xl font-black uppercase tracking-tight text-white leading-[1.05] mb-6">
            {t("title")}
          </h1>

          <p className="text-neutral-300 text-base md:text-lg leading-relaxed mb-4 max-w-xl font-light">
            {t("p1")}
          </p>
          <p className="text-neutral-400 text-sm md:text-base leading-relaxed mb-10 max-w-xl font-light">
            {t("p2")}
          </p>

          <div className="flex flex-wrap items-center gap-8 sm:gap-12 pt-6 border-t border-dark-border">
            {[1, 2, 3].map((num) => (
              <div key={num}>
                <div className="flex items-center gap-2 mb-1">
                  <span className="w-1.5 h-1.5 rounded-full bg-brand-500 animate-pulse" />
                  <span className="text-white text-3xl sm:text-4xl font-black font-mono">
                    {t(`stats.s${num}_value`)}
                  </span>
                </div>
                <div className="text-neutral-400 text-xs sm:text-sm font-semibold tracking-wide">
                  {t(`stats.s${num}_label`)}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="w-full lg:w-1/2 relative z-10">
          <div className="relative rounded-2xl overflow-hidden border border-dark-border shadow-2xl group transition-all duration-500 hover:-translate-y-2 hover:shadow-[0_15px_30px_-10px_rgba(0,0,0,0.7)] bg-dark-glass aspect-16/10">
            <Image
              src="/assets/contact.webp"
              alt="TEDxAlKawmia Stage"
              fill
              className="object-cover opacity-90 group-hover:opacity-100 group-hover:scale-105 transition-all duration-700"
            />
            <div className="absolute inset-0 bg-linear-to-t from-dark-obsidian/80 via-transparent to-transparent pointer-events-none" />
            <div className="absolute bottom-0 inset-x-0 h-1 bg-linear-to-r from-transparent via-brand-500 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
          </div>
        </div>
      </div>
    </section>
  );
}
