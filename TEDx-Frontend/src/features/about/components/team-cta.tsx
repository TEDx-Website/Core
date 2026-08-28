import Link from "next/link";
import { useTranslations } from "next-intl";
import { Button } from "@/shared/ui/button";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowUpRight } from "lucide-react";

export function TeamCta() {
  const t = useTranslations("about.TeamCta");

  return (
    <section className="w-full py-25 md:py-35 bg-[#EBE9E9] transition-colors duration-300 border-t border-neutral-200 dark:border-neutral-800">
      <GridContainer className="items-center justify-center text-center">
        <div className="col-span-4 md:col-span-8 lg:col-span-8 lg:col-start-3 flex flex-col items-center gap-6 px-4 md:px-0">
          <h2 className="text-3xl sm:text-4xl md:text-5xl font-black text-brand-500 tracking-tight leading-tight">
            {t("title")}
          </h2>

          <p className="text-sm md:text-base text-neutral-900 max-w-lg leading-relaxed">
            {t("description")}
          </p>

          <div className="pt-2 w-full sm:w-auto flex justify-center">
            <Button
              asChild
              size="lg"
              className="w-full sm:w-auto bg-brand-500 hover:bg-brand-600 text-white rounded-full px-8 h-12 md:h-14 text-xs md:text-sm font-bold tracking-widest uppercase shadow-md"
            >
              <Link href="#join">
                {t("cta")}
                <ArrowUpRight className="ml-2 size-4" strokeWidth={3} />
              </Link>
            </Button>
          </div>
        </div>
      </GridContainer>
    </section>
  );
}
