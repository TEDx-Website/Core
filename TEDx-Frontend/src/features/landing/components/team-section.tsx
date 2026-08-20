import Image from "next/image";
import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowRight } from "lucide-react";

export function TeamSection() {
  const t = useTranslations("landing.Team");

  return (
    <section className="w-full py-[80px] md:py-120px bg-neutral-100 dark:bg-neutral-950 transition-colors duration-300">
      <GridContainer className="items-center gap-y-12">
        <div className="col-span-4 md:col-span-8 lg:col-span-6 relative w-full aspect-4/3 rounded-[24px] overflow-hidden">
          <Image
            src="/assets/team.webp"
            alt="TEDx Alkawmia Team"
            fill
            sizes="(max-width: 768px) 100vw, 50vw"
            className="object-cover"
          />
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-5 lg:col-start-8 flex flex-col gap-6 lg:pl-8">
          <div className="flex flex-col gap-4">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
              {t("tagline")}
            </span>

            <h2 className="text-3xl md:text-4xl lg:text-[44px] font-bold text-foreground tracking-tight leading-[1.15]">
              {t("title")}
            </h2>
          </div>

          <p className="text-base md:text-lg text-neutral-600 dark:text-neutral-400 leading-relaxed">
            {t("description")}
          </p>

          <div className="pt-2">
            <Link
              href="#team"
              className="inline-flex items-center gap-2 text-xs font-bold text-foreground hover:text-brand-600 transition-colors uppercase tracking-widest group/link"
            >
              {t("cta")}
              <ArrowRight
                className="size-4 text-brand-600 group-hover/link:translate-x-1 transition-transform"
                strokeWidth={2.5}
              />
            </Link>
          </div>
        </div>
      </GridContainer>
    </section>
  );
}
