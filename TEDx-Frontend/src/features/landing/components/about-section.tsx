"use client";

import { useTranslations } from "next-intl";

export function AboutSection() {
  const t = useTranslations("landing.about");

  return (
    <section
      id="possibilities"
      className="py-24 md:py-32 relative border-t border-dark-border/50 overflow-hidden bg-dark-obsidian"
    >
      <div className="absolute top-1/4 right-[10vw] w-[45vw] h-[45vw] bg-brand-500/5 rounded-full blur-[150px] -z-10 pointer-events-none" />
      <div className="absolute -bottom-24 left-0 w-[35vw] h-[35vw] bg-brand-500/3 rounded-full blur-[130px] -z-10 pointer-events-none" />

      <div className="max-w-7xl mx-auto px-6 grid lg:grid-cols-[1.18fr_1fr] gap-14 lg:gap-20 items-center">
        <div className="animate-in fade-in slide-in-from-bottom-8 duration-1000">
          <p className="font-script text-brand-500 text-[clamp(1.75rem,3.4vw,2.85rem)] leading-[1.15] drop-shadow-[0_0_28px_rgba(235,0,40,0.45)] mb-3">
            {t("eyebrow")}
          </p>

          <h2 className="font-bold text-white tracking-tight leading-[0.95] mb-7 text-[clamp(2.4rem,5.2vw,4.25rem)]">
            {t("titleLine1")}
            <br />
            {t("titleLine2")}
          </h2>

          <div className="w-23 h-.75 bg-linear-to-r from-brand-500 to-brand-500/10 shadow-[0_0_14px_rgba(235,0,40,0.55)] mb-9" />

          <div className="space-y-6 text-neutral-300 text-lg font-light leading-relaxed max-w-[58ch]">
            <p>
              <strong className="text-white font-medium">
                {t("brandName")}
              </strong>
              {t("p1Text")}
            </p>
            <p>{t("p2")}</p>
          </div>
        </div>

        <div className="w-full lg:max-w-110 justify-self-center lg:justify-self-end animate-in fade-in slide-in-from-bottom-8 duration-1000 delay-150">
          <div className="group relative w-full aspect-4/5 bg-[#09090B] border border-dark-border transition-colors duration-700 hover:border-brand-500/50 overflow-hidden cursor-pointer">
            <div
              className="absolute inset-0 grid place-items-center"
              aria-hidden="true"
            >
              {[0, 1, 2, 3].map((i) => (
                <div
                  key={i}
                  className="absolute aspect-[1/1.55]"
                  style={{
                    width: `calc(26% + ${i} * 15%)`,
                    border: `1px solid rgba(235, 0, 40, calc(0.30 - ${i} * 0.055))`,
                    borderTopColor: `rgba(255, 255, 255, calc(0.10 - ${i} * 0.02))`,
                  }}
                />
              ))}
              <div className="w-[14%] aspect-1/1.5 bg-[radial-gradient(ellipse_at_50%_58%,#FFF5E6,#FF2E4D_26%,#EB0028_48%,transparent_78%)] blur-[13px] animate-plate-breathe" />
            </div>


            <div
              className="absolute inset-0 bg-linear-to-b from-brand-500/40 to-dark-obsidian/10 mix-blend-color opacity-10 z-20 pointer-events-none"
              aria-hidden="true"
            />

            <div
              className="absolute inset-x-0 bottom-0 h-.5 bg-linear-to-r from-transparent via-brand-glow to-transparent shadow-[0_0_20px_rgba(235,0,40,0.6)] z-20 pointer-events-none"
              aria-hidden="true"
            />
          </div>
        </div>
      </div>
    </section>
  );
}
