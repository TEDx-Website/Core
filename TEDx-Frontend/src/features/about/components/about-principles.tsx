import { useTranslations } from "next-intl";
import Image from "next/image";
import { Mic, Lightbulb, Users, Zap, Shield } from "lucide-react";
import { cn } from "@/lib/utils";

export function AboutPrinciples() {
  const t = useTranslations("about.principles");
  const icons = [Lightbulb, Users, Zap, Shield];
  const shapes = [
    "rounded-tl-[1.5rem] rounded-tr-[4rem] rounded-bl-[1.5rem] rounded-br-[1.5rem]",
    "rounded-tl-[4rem] rounded-tr-[1.5rem] rounded-bl-[1.5rem] rounded-br-[4rem]",
    "rounded-tl-[1.5rem] rounded-tr-[1.5rem] rounded-bl-[1.5rem] rounded-br-[4rem]",
    "rounded-tl-[1.5rem] rounded-tr-[4rem] rounded-bl-[4rem] rounded-br-[1.5rem]",
  ];

  return (
    <section className="relative py-24 sm:py-32 px-6 sm:px-12 w-full bg-dark-obsidian border-b border-dark-border/50 overflow-hidden">
      <div className="absolute top-1/2 left-1/4 -translate-x-1/2 -translate-y-1/2 w-[50vw] h-[50vw] max-w-137.5 max-h-137.5 bg-brand-500/10 rounded-full blur-[160px] pointer-events-none" />

      <div className="max-w-7xl mx-auto w-full relative z-10">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-12 items-stretch">
          <div className="lg:col-span-5 flex flex-col animate-in fade-in slide-in-from-left-8 duration-1000">
            <div className="relative w-full h-full min-h-125 lg:min-h-155 rounded-3xl overflow-hidden border border-dark-border bg-dark-glass shadow-2xl group flex flex-col justify-between p-6 sm:p-8">
              <Image
                src="/assets/about-2.webp"
                alt="Principles"
                fill
                className="absolute inset-0 object-cover object-center group-hover:scale-105 transition-transform duration-700 ease-out"
              />
              <div className="absolute inset-0 bg-linear-to-t from-dark-obsidian/80 via-dark-obsidian/40 to-transparent pointer-events-none" />
              <div className="absolute inset-0 bg-linear-to-b from-dark-obsidian/70 via-transparent to-transparent pointer-events-none" />
              <div className="absolute inset-0 border border-brand-500/20 rounded-3xl pointer-events-none group-hover:border-brand-500/40 transition-colors duration-500" />

              <div className="relative z-10 flex items-center justify-between w-full">
                <div className="w-8 h-8 rounded-full bg-dark-obsidian/80 backdrop-blur-md border border-dark-border flex items-center justify-center text-brand-500 text-xs shadow-lg">
                  <Mic className="size-4" />
                </div>
              </div>

              <div className="relative z-10 space-y-2 mt-auto">
                <div className="flex items-center gap-2">
                  <div className="h-px w-4 bg-brand-500" />
                  <span className="text-brand-500 text-[11px] font-bold tracking-[0.25em] uppercase">
                    {t("imageBadge")}
                  </span>
                </div>
                <h3 className="text-2xl sm:text-3xl font-black uppercase tracking-tight text-white">
                  {t("imageTitle")}
                </h3>
                <p className="text-xs sm:text-sm text-neutral-300 font-light leading-relaxed">
                  {t("imageDesc")}
                </p>
              </div>
            </div>
          </div>

          <div className="lg:col-span-7 flex flex-col justify-between animate-in fade-in slide-in-from-right-8 duration-1000">
            <div className="mb-8">
              <div className="flex items-center gap-3 mb-3">
                <div className="h-px w-6 bg-brand-500" />
                <span className="text-brand-500 text-xs font-bold tracking-[0.25em] uppercase">
                  {t("eyebrow")}
                </span>
              </div>
              <h2 className="text-3xl sm:text-4xl lg:text-5xl font-black uppercase tracking-tight text-white mb-3">
                {t("title")}
              </h2>
              <p className="text-neutral-400 text-sm sm:text-base font-light leading-relaxed max-w-2xl">
                {t("subtitle")}
              </p>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 sm:gap-5 flex-1">
              {[0, 1, 2, 3].map((idx) => {
                const Icon = icons[idx];
                return (
                  <div
                    key={idx}
                    className={cn(
                      "bg-dark-glass p-6 sm:p-7 relative overflow-hidden border border-dark-border flex flex-col justify-between transition-all duration-400 hover:-translate-y-2 hover:shadow-[0_15px_30px_-10px_rgba(0,0,0,0.7)]",
                      shapes[idx],
                    )}
                  >
                    <div className="flex justify-between items-start mb-4">
                      <span className="text-brand-500 font-bold text-lg">
                        {t(`items.${idx}.num`)}
                      </span>
                      <div className="w-9 h-9 rounded-full bg-black flex items-center justify-center border border-dark-border text-brand-500 text-sm">
                        <Icon className="size-4" />
                      </div>
                    </div>
                    <div>
                      <h3 className="text-base sm:text-lg font-bold mb-2 uppercase leading-tight text-white">
                        {t(`items.${idx}.title`)}
                      </h3>
                      <p className="text-neutral-400 text-xs sm:text-sm leading-relaxed font-light">
                        {t(`items.${idx}.desc`)}
                      </p>
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
