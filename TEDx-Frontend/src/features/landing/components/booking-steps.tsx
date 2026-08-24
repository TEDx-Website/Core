import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";

export function BookingSteps() {
  const t = useTranslations("landing.BookingSteps");
  const steps = ["step1", "step2", "step3", "step4"] as const;

  return (
    <section className="w-full py-[80px] md:py-120px bg-background transition-colors duration-300">
      <GridContainer className="gap-y-12">
        <div className="col-span-4 md:col-span-8 lg:col-span-12 flex flex-col gap-3">
          <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
            {t("tagline")}
          </span>
          <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-foreground tracking-tight max-w-xl leading-[1.15]">
            {t("title")}
          </h2>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-12 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {steps.map((step) => (
            <div
              key={step}
              className="relative flex flex-col justify-between bg-white dark:bg-neutral-900 border border-neutral-200 dark:border-neutral-800 rounded-[20px] p-6 md:p-8 shadow-sm hover:shadow-md transition-shadow duration-300"
            >
              <div className="flex items-center justify-between mb-8">
                <span className="text-brand-600 font-bold text-lg tracking-wider">
                  {t(`steps.${step}.number`)}
                </span>
                <div className="w-3 h-3 bg-neutral-900 dark:bg-white rounded-[2px]" />
              </div>

              <div className="flex flex-col gap-2">
                <h3 className="text-xl font-bold text-foreground tracking-tight">
                  {t(`steps.${step}.title`)}
                </h3>
                <p className="text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed">
                  {t(`steps.${step}.description`)}
                </p>
              </div>
            </div>
          ))}
        </div>
      </GridContainer>
    </section>
  );
}
