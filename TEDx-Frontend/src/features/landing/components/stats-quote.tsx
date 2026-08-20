import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { Quote } from "lucide-react";

export function StatsQuote() {
  const t = useTranslations("landing.StatsQuote");
  const stats = ["stat1", "stat2", "stat3", "stat4"] as const;

  return (
    <section className="w-full py-[80px] md:py-120px bg-neutral-1000 text-white transition-colors duration-300">
      <GridContainer className="items-center gap-y-16 lg:gap-y-0">
        <div className="col-span-4 md:col-span-8 lg:col-span-6 flex flex-col border border-neutral-800 rounded-[20px] overflow-hidden bg-neutral-900/40">
          {stats.map((stat) => (
            <div
              key={stat}
              className="flex flex-col sm:flex-row sm:items-center justify-between gap-2 p-6 md:p-8 border-b border-neutral-800 last:border-b-0 bg-white/1"
            >
              <span className="text-4xl md:text-5xl font-black text-brand-500 tracking-tight">
                {t(`stats.${stat}.value`)}
              </span>
              <span className="text-xs md:text-sm font-medium text-neutral-400 sm:text-right max-w-50">
                {t(`stats.${stat}.label`)}
              </span>
            </div>
          ))}
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-5 lg:col-start-8 flex flex-col gap-6 lg:pl-6">
          <Quote
            className="text-brand-500 size-12 fill-brand-500/1k shrink-0"
            strokeWidth={1}
          />

          <blockquote className="text-3xl md:text-5xl font-normal text-neutral-100 leading-relaxed font-script tracking-wide">
            {t("quote")}
          </blockquote>

          <div className="flex flex-col gap-1 pt-4">
            <span className="text-xs font-bold tracking-widest text-white uppercase">
              {t("author")}
            </span>
            <span className="text-xs font-semibold tracking-wider text-neutral-400 uppercase">
              {t("meta")}
            </span>
          </div>
        </div>
      </GridContainer>
    </section>
  );
}
