import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";
import { ArrowRight } from "lucide-react";

export function Tracks() {
  const t = useTranslations("landing.Tracks");
  const tracks = ["track1", "track2", "track3"] as const;

  return (
    <section className="w-full py-[80px] md:py-120px bg-brand-900 text-white transition-colors duration-300">
      <GridContainer className="items-center gap-y-12">
        <div className="col-span-4 md:col-span-8 lg:col-span-6 flex flex-col gap-6 lg:pr-8">
          <div className="flex flex-col gap-4">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-500 uppercase">
              {t("tagline")}
            </span>

            <h2 className="text-3xl md:text-4xl lg:text-[52px] font-bold text-white tracking-tight leading-[1.15]">
              {t("title")}
            </h2>
          </div>

          <p className="text-base md:text-lg text-neutral-300 leading-relaxed max-w-xl">
            {t("description")}
          </p>

          <div className="pt-2">
            <Link
              href="#tracks"
              className="inline-flex items-center gap-2 text-xs font-bold text-white hover:text-brand-500 transition-colors uppercase tracking-widest group/link"
            >
              {t("cta")}
              <ArrowRight
                className="size-4 text-brand-500 group-hover/link:translate-x-1 transition-transform"
                strokeWidth={2.5}
              />
            </Link>
          </div>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-6 flex flex-col border border-white/20 rounded-[20px] overflow-hidden">
          {tracks.map((track) => (
            <div
              key={track}
              className="flex items-center justify-between p-6 md:p-8 border-b border-white/20 last:border-b-0 bg-white/2 hover:bg-white/5 transition-colors duration-200"
            >
              <div className="flex items-center gap-6 md:gap-8">
                <span className="text-brand-500 font-bold text-lg md:text-xl tracking-wider">
                  {t(`items.${track}.number`)}
                </span>
                <div className="flex flex-col gap-1">
                  <h3 className="text-lg md:text-xl font-bold text-white tracking-tight">
                    {t(`items.${track}.title`)}
                  </h3>
                  <p className="text-xs md:text-sm text-neutral-400">
                    {t(`items.${track}.description`)}
                  </p>
                </div>
              </div>

              <div className="w-3 h-3 bg-white rounded-[2px] shrink-0 ml-4" />
            </div>
          ))}
        </div>
      </GridContainer>
    </section>
  );
}
