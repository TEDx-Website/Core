import { useTranslations } from "next-intl";
import { Button } from "@/shared/ui/button";
import { GridContainer } from "@/shared/ui/grid-container";
import { Mail } from "lucide-react";

export function Newsletter() {
  const t = useTranslations("landing.Newsletter");

  return (
    <section className="w-full py-[80px] md:py-25 bg-background border-t border-neutral-200 dark:border-neutral-800 transition-colors duration-300">
      <GridContainer className="items-center gap-y-8">
        <div className="col-span-4 md:col-span-8 lg:col-span-6 flex flex-col gap-3">
          <span className="text-xs font-bold tracking-[0.15em] text-neutral-500 dark:text-neutral-400 uppercase">
            {t("tagline")}
          </span>
          <h2 className="text-2xl md:text-3xl lg:text-4xl font-bold text-foreground tracking-tight">
            {t("title")}
          </h2>
          <p className="text-sm md:text-base text-neutral-500 dark:text-neutral-400">
            {t("description")}
          </p>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-6 flex items-center lg:justify-end">
          <form
            // onSubmit={(e) => e.preventDefault()}
            className="w-full max-w-lg flex flex-col sm:flex-row gap-2 sm:gap-3 bg-neutral-100 dark:bg-neutral-900 border border-neutral-300 dark:border-neutral-800 rounded-[24px] sm:rounded-full p-2"
          >
            <div className="flex items-center gap-3 px-4 pt-2 pb-1 sm:py-0 w-full">
              <Mail
                className="size-5 text-neutral-400 shrink-0"
                strokeWidth={1.5}
              />
              <input
                type="email"
                placeholder={t("placeholder")}
                className="w-full bg-transparent border-none outline-none text-sm text-white placeholder:text-neutral-500 h-10"
              />
            </div>

            <Button
              type="submit"
              className="w-full sm:w-auto bg-brand-600 hover:bg-brand-700 text-white rounded-full px-8 h-12 text-xs font-bold tracking-widest uppercase shrink-0"
            >
              {t("cta")}
            </Button>
          </form>
        </div>
      </GridContainer>
    </section>
  );
}
