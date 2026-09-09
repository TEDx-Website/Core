import { useTranslations } from "next-intl";
import { Sparkles, Key, DoorOpen } from "lucide-react";

export function AboutTheme() {
  const t = useTranslations("about.theme");
  const icons = [Sparkles, Key, DoorOpen];

  return (
    <section className="relative py-32 px-6 sm:px-12 bg-dark-carbon overflow-hidden w-full border-b border-dark-border/50">
      <div className="hidden lg:block absolute left-1/2 top-0 bottom-0 w-px bg-linear-to-b from-transparent via-dark-border to-transparent -translate-x-1/2" />

      <div className="max-w-7xl mx-auto animate-in fade-in slide-in-from-bottom-8 duration-1000">
        <div className="text-center mb-24">
          <div className="flex items-center justify-center gap-4 mb-4">
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
            <span className="text-brand-500 text-xs font-bold tracking-[0.2em] uppercase">
              {t("eyebrow")}
            </span>
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
          </div>
          <h2 className="text-3xl md:text-5xl font-black uppercase tracking-tight text-white mb-4">
            {t("title")}
          </h2>
          <p className="text-neutral-400 max-w-xl mx-auto text-sm sm:text-base font-light">
            {t("subtitle")}
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-16 lg:gap-8">
          {[0, 1, 2].map((idx) => {
            const Icon = icons[idx];
            return (
              <div
                key={idx}
                className="relative flex flex-col items-center text-center group"
              >
                <div className="text-[120px] font-black leading-none text-dark-glass absolute -top-16 select-none group-hover:text-[#1A1A1A] transition-colors duration-500">
                  {t(`steps.${idx}.num`)}
                </div>
                <div className="w-20 h-20 bg-dark-obsidian border border-dark-border rounded-2xl flex items-center justify-center mb-8 relative z-10 group-hover:-translate-y-2 group-hover:border-brand-500/50 group-hover:shadow-[0_0_30px_-5px_rgba(235,0,40,0.3)] transition-all duration-500">
                  <Icon className="w-8 h-8 text-white group-hover:text-brand-500 transition-colors duration-500" />
                </div>
                <h3 className="text-2xl font-bold text-white mb-4 tracking-tight uppercase relative z-10">
                  {t(`steps.${idx}.title`)}
                </h3>
                <p className="text-neutral-400 leading-relaxed max-w-sm relative z-10 font-light text-sm">
                  {t(`steps.${idx}.desc`)}
                </p>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
