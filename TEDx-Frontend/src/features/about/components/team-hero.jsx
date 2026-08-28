import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";

export function TeamHero() {
  const t = useTranslations("about.Hero");

  return (
    <section className="w-full pt-35 md:pt-45 pb-15 md:pb-[80px] bg-background transition-colors duration-300">
      <GridContainer>
        <div className="col-span-4 md:col-span-8 lg:col-span-12 flex flex-col gap-6 md:gap-8">
          
          <h1 className="text-6xl sm:text-7xl md:text-8xl lg:text-[120px] font-black text-foreground tracking-tighter uppercase leading-[0.9] md:leading-[0.85]">
            <span className="block">{t("titleLine1")}</span>
            <span className="block">
              {t("titleLine2")}{" "}
              <span className="text-brand-500">{t("titleHighlight")}</span>
            </span>
          </h1>
          
          <p className="text-base md:text-xl text-neutral-500 dark:text-neutral-400 max-w-150 leading-relaxed font-medium">
            {t("description")}
          </p>

        </div>
      </GridContainer>
    </section>
  );
}