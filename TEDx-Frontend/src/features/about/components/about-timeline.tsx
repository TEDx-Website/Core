import { useTranslations } from "next-intl";
import { Flag, Mic, Rocket, Ticket } from "lucide-react";

export function AboutTimeline() {
  const t = useTranslations("about.timeline");
  const icons = [Flag, Mic, Rocket, Ticket];

  return (
    <section className="relative py-32 px-6 sm:px-12 bg-dark-obsidian w-full">
      <div className="max-w-4xl mx-auto animate-in fade-in slide-in-from-bottom-8 duration-1000">
        <div className="text-center mb-20">
          <div className="flex items-center justify-center gap-4 mb-4">
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
            <span className="text-brand-500 text-xs font-bold tracking-[0.2em] uppercase">
              {t("eyebrow")}
            </span>
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
          </div>
          <h2 className="text-3xl md:text-5xl font-black uppercase text-white mb-6 tracking-tight">
            {t("title")}
          </h2>
          <p className="text-neutral-400 max-w-2xl mx-auto text-base sm:text-lg">
            {t("subtitle")}
          </p>
        </div>

        <div className="relative">
          <div className="absolute left-6 md:left-10 top-2 bottom-4 w-0.5 bg-[#1a1a1a] -translate-x-px" />

          {[0, 1, 2, 3].map((idx) => {
            const Icon = icons[idx];
            const isLast = idx === 3;
            return (
              <div
                key={idx}
                className={`relative flex gap-8 md:gap-14 group ${isLast ? "" : "mb-16"}`}
              >
                <div
                  className={`relative z-10 w-12 h-12 md:w-20 md:h-20 shrink-0 rounded-full flex items-center justify-center transition-all duration-500 group-hover:scale-110 ${
                    isLast
                      ? "border-2 md:border-[3px] border-brand-500 bg-brand-500 shadow-[0_0_25px_rgba(235,0,40,0.5)]"
                      : "border-2 md:border-[3px] border-[#1f1f1f] bg-dark-carbon group-hover:border-neutral-600"
                  }`}
                >
                  <Icon
                    className={`text-lg md:text-2xl ${isLast ? "text-white" : "text-neutral-500 group-hover:text-neutral-300"}`}
                  />
                </div>
                <div className="grow pt-1 md:pt-5">
                  <div className="flex flex-col md:flex-row md:items-baseline gap-1 md:gap-3 mb-3">
                    <span className="text-brand-500 font-bold uppercase tracking-wider text-sm md:text-base whitespace-nowrap">
                      {t(`items.${idx}.date`)}
                    </span>
                    <h3 className="text-white text-xl md:text-2xl font-bold leading-tight">
                      {t(`items.${idx}.title`)}
                    </h3>
                  </div>
                  <p className="text-neutral-400 leading-relaxed text-sm md:text-base">
                    {t(`items.${idx}.desc`)}
                  </p>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}
