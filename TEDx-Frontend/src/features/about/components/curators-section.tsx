import Image from "next/image";
import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowUpRight } from "lucide-react";

export function CuratorsSection() {
  const t = useTranslations("about.CuratorsSection");

  const members = t.raw("members") as Array<{
    name: string;
    role: string;
    bio: string;
  }>;

  return (
    <section className="w-full py-[80px] md:py-120px bg-background transition-colors duration-300 border-t border-neutral-200 dark:border-neutral-800">
      <GridContainer>
        <div className="col-span-4 md:col-span-8 lg:col-span-12 grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-16 items-start">
          <div className="lg:col-span-4 flex flex-col gap-4 lg:sticky lg:top-32">
            <span className="text-3xl md:text-4xl font-black text-brand-500 tracking-tighter">
              {t("id")}
            </span>

            <div className="flex flex-col gap-2">
              <h3 className="text-sm font-bold tracking-[0.2em] text-foreground uppercase">
                {t("category")}
              </h3>
              <p className="text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed max-w-sm">
                {t("categoryDesc")}
              </p>
            </div>
          </div>

          <div className="lg:col-span-8 grid grid-cols-1 md:grid-cols-2 gap-8">
            {members.map((member, index) => (
              <div key={member.name} className="flex flex-col gap-6">
                <div className="relative w-full aspect-4/5 rounded-[24px] overflow-hidden bg-neutral-200 dark:bg-neutral-800 shadow-sm">
                  <Image
                    src={`/assets/about-2.${index + 1}.webp`}
                    alt={member.name}
                    fill
                    sizes="(max-width: 768px) 100vw, 33vw"
                    className="object-cover grayscale hover:grayscale-0 transition-all duration-700 ease-out"
                  />
                </div>

                <div className="flex items-start justify-between gap-2 pt-2">
                  <div className="flex flex-col gap-1">
                    <h4 className="text-xl md:text-2xl font-bold text-foreground tracking-tight">
                      {member.name}
                    </h4>
                    <span className="text-xs font-bold tracking-[0.15em] text-brand-600 uppercase">
                      {member.role}
                    </span>
                  </div>

                  <Link
                    href="#"
                    className="group inline-flex items-center gap-1 text-xs font-bold text-brand-600 hover:text-brand-700 uppercase tracking-widest transition-colors pt-1 shrink-0"
                  >
                    {t("viewProfile")}
                    <ArrowUpRight
                      className="size-3.5 group-hover:translate-x-0.5 group-hover:-translate-y-0.5 transition-transform"
                      strokeWidth={2.5}
                    />
                  </Link>
                </div>

                <p className="text-sm text-neutral-500 dark:text-neutral-400 leading-relaxed">
                  {member.bio}
                </p>
              </div>
            ))}
          </div>
        </div>
      </GridContainer>
    </section>
  );
}
