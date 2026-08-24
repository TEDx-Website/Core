import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/shared/ui/accordion";

export function FaqSection() {
  const t = useTranslations("landing.Faq");
  const questions = ["q1", "q2", "q3", "q4"] as const;

  return (
    <section className="w-full py-[80px] md:py-120px bg-background transition-colors duration-300">
      <GridContainer className="items-start gap-y-12">
        <div className="col-span-4 md:col-span-8 lg:col-span-5 flex flex-col gap-4 lg:pr-8">
          <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
            {t("tagline")}
          </span>
          <h2 className="text-3xl md:text-4xl lg:text-[44px] font-bold text-foreground tracking-tight leading-[1.15]">
            {t("title")}
          </h2>
          <p className="text-base text-neutral-500 dark:text-neutral-400 leading-relaxed max-w-sm">
            {t("description")}
          </p>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-7">
          <Accordion type="single" collapsible className="w-full flex flex-col">
            {questions.map((q) => (
              <AccordionItem
                key={q}
                value={q}
                className="border-b border-neutral-200 dark:border-neutral-800 py-2"
              >
                <AccordionTrigger className="text-left text-lg md:text-xl font-bold text-foreground hover:no-underline py-4 [&[data-state=open]>svg]:rotate-45 transition-all">
                  {t(`items.${q}.question`)}
                </AccordionTrigger>
                <AccordionContent className="text-sm md:text-base text-neutral-500 dark:text-neutral-400 pb-6 leading-relaxed">
                  {t(`items.${q}.answer`)}
                </AccordionContent>
              </AccordionItem>
            ))}
          </Accordion>
        </div>
      </GridContainer>
    </section>
  );
}
