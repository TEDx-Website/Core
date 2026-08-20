import Link from "next/link";
import { useTranslations } from "next-intl";
import { Button } from "@/shared/ui/button";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowUpRight } from "lucide-react";

export function CtaBanner() {
  const t = useTranslations("landing.CtaBanner");

  return (
    <section className="w-full py-[80px] md:py-140px bg-brand-900 text-white transition-colors duration-300 relative overflow-hidden">
      <GridContainer className="items-center justify-center text-center">
        <div className="col-span-4 md:col-span-8 lg:col-span-8 lg:col-start-3 flex flex-col items-center gap-5 md:gap-6 px-4 md:px-0">
          <span className="text-[10px] md:text-xs font-bold tracking-[0.2em] text-brand-500 uppercase">
            {t("tagline")}
          </span>

          <h2 className="text-3xl sm:text-4xl md:text-5xl lg:text-6xl font-black text-white tracking-tight leading-[1.2] md:leading-tight">
            {t("title")}
          </h2>

          <p className="text-sm md:text-lg text-neutral-300 max-w-lg leading-relaxed">
            {t("description")}
          </p>

          <div className="pt-4 md:pt-6 w-full flex justify-center sm:w-auto">
            <Button
              asChild
              className="w-full sm:w-auto bg-brand-500 hover:bg-brand-600 text-white rounded-full px-8 h-12 md:h-14 text-xs md:text-sm font-bold tracking-widest uppercase shadow-lg"
            >
              <Link href="#events">
                {t("cta")}
                <ArrowUpRight
                  className="ml-2 size-4 shrink-0"
                  strokeWidth={3}
                />
              </Link>
            </Button>
          </div>
        </div>
      </GridContainer>
    </section>
  );
}
