import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";

export function FeaturesGrid() {
  const t = useTranslations("landing.FeaturesGrid");
  const items = ["item1", "item2", "item3", "item4", "item5", "item6"] as const;

  return (
    <section className="w-full py-[80px] md:py-120px bg-neutral-100 dark:bg-neutral-950 transition-colors duration-300">
      <GridContainer>
        <div className="col-span-4 md:col-span-8 lg:col-span-12 flex flex-col items-center text-center md:items-start md:text-left gap-3 mb-12">
          <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
            {t("tagline")}
          </span>
          <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-foreground tracking-tight max-w-2xl leading-[1.15]">
            {t("title")}
          </h2>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-12 grid grid-cols-1 md:grid-cols-2 border border-neutral-300 dark:border-neutral-800 rounded-[20px] overflow-hidden bg-white dark:bg-neutral-900 shadow-sm">
          {items.map((item, index) => {
            const isRightCol = index % 2 === 1;
            const isLastRow = index >= 4;

            return (
              <div
                key={item}
                className={`flex flex-col md:flex-row items-center md:items-start gap-6 p-8 md:p-10 border-b border-neutral-300 dark:border-neutral-800 ${
                  !isRightCol
                    ? "md:border-r border-neutral-300 dark:border-neutral-800"
                    : ""
                } ${isLastRow ? "md:border-b-0" : ""} ${index === 5 ? "border-b-0" : ""}`}
              >
                <div className="w-10 h-10 rounded-lg bg-brand-600 flex items-center justify-center shrink-0 shadow-sm">
                  <div className="w-4 h-4 bg-white rounded-[2px]" />
                </div>

                <div className="flex flex-col items-center text-center md:items-start md:text-left gap-2">
                  <span className="text-[11px] font-bold tracking-[0.15em] text-brand-600 uppercase">
                    {t(`items.${item}.badge`)}
                  </span>
                  <h3 className="text-xl font-bold text-foreground tracking-tight">
                    {t(`items.${item}.title`)}
                  </h3>
                  <p className="text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed">
                    {t(`items.${item}.description`)}
                  </p>
                </div>
              </div>
            );
          })}
        </div>
      </GridContainer>
    </section>
  );
}
