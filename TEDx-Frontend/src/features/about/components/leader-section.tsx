import Image from "next/image";
import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowUpRight } from "lucide-react";

export function LeaderSection() {
  const t = useTranslations("about.LeaderSection");
  const members = ["01"] as const;

  return (
    <section className="w-full py-[80px] md:py-120px bg-background transition-colors duration-300">
      <GridContainer>
        {members.map((id) => (
          <div
            key={id}
            className="col-span-4 md:col-span-8 lg:col-span-12 grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-16 items-start mb-20 last:mb-0"
          >
            <div className="lg:col-span-4 flex flex-col gap-4">
              <span className="text-3xl md:text-4xl font-black text-brand-500 tracking-tighter">
                {t(`members.0.id`)}
              </span>

              <div className="flex flex-col gap-2">
                <h3 className="text-sm font-bold tracking-[0.2em] text-foreground uppercase">
                  {t(`members.0.category`)}
                </h3>
                <p className="text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed max-w-sm">
                  {t(`members.0.categoryDesc`)}
                </p>
              </div>
            </div>

            <div className="lg:col-span-8 flex flex-col gap-6">
              <div className="relative w-full aspect-16/10 rounded-[24px] overflow-hidden bg-neutral-200 dark:bg-neutral-800 shadow-sm">
                <Image
                  src="/assets/about-1.webp"
                  alt={t(`members.0.name`)}
                  fill
                  sizes="(max-width: 1024px) 100vw, 66vw"
                  className="object-cover grayscale hover:grayscale-0 transition-all duration-700 ease-out"
                />
              </div>

              <div className="flex flex-col md:flex-row md:items-end justify-between gap-4 pt-2">
                <div className="flex flex-col gap-1">
                  <h4 className="text-2xl md:text-3xl font-bold text-foreground tracking-tight">
                    {t(`members.0.name`)}
                  </h4>
                  <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
                    {t(`members.0.role`)}
                  </span>
                </div>

                <Link
                  href="#"
                  className="group inline-flex items-center gap-1.5 text-xs font-bold text-brand-600 hover:text-brand-700 uppercase tracking-widest transition-colors pb-1"
                >
                  {t(`members.0.viewProfile`)}
                  <ArrowUpRight
                    className="size-4 group-hover:translate-x-0.5 group-hover:-translate-y-0.5 transition-transform"
                    strokeWidth={2.5}
                  />
                </Link>
              </div>

              <p className="text-sm md:text-base text-neutral-500 dark:text-neutral-400 leading-relaxed max-w-2xl pt-2 border-t border-neutral-200 dark:border-neutral-800">
                {t(`members.0.bio`)}
              </p>
            </div>
          </div>
        ))}
      </GridContainer>
    </section>
  );
}
