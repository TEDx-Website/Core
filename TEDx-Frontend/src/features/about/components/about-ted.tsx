import { useTranslations } from "next-intl";
import { Globe, Lightbulb, CheckCircle2 } from "lucide-react";

export function AboutTed() {
  const t = useTranslations("about.tedInfo");

  return (
    <section className="relative py-24 px-6 sm:px-12 bg-dark-carbon w-full border-b border-dark-border/50">
      <div className="max-w-5xl mx-auto animate-in fade-in slide-in-from-bottom-8 duration-1000">
        <div className="text-center mb-16">
          <div className="flex items-center justify-center gap-4 mb-4">
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
            <span className="text-brand-500 text-xs font-bold tracking-[0.2em] uppercase">
              {t("eyebrow")}
            </span>
            <div className="h-px w-8 sm:w-12 bg-brand-500" />
          </div>
          <h2 className="text-3xl md:text-5xl font-black uppercase tracking-tight text-white mb-4">
            {t("title")}
          </h2>
          <p className="text-neutral-400 max-w-xl mx-auto text-sm sm:text-base font-light">
            {t("subtitle")}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
          <div className="bg-dark-glass border border-dark-border rounded-2xl p-8 sm:p-10 hover:border-neutral-700 transition-colors duration-300">
            <div className="flex items-center gap-3 mb-6">
              <Globe className="size-6 text-brand-500" />
              <h3 className="text-xl font-bold text-white uppercase tracking-wide">
                {t("tedTitle")}
              </h3>
            </div>
            <p className="text-neutral-400 leading-relaxed text-sm">
              {t("tedDesc")}
            </p>
          </div>

          <div className="bg-dark-glass border border-dark-border rounded-2xl p-8 sm:p-10 hover:border-neutral-700 transition-colors duration-300">
            <div className="flex items-center gap-3 mb-6">
              <Lightbulb className="size-6 text-brand-500" />
              <h3 className="text-xl font-bold text-white uppercase tracking-wide">
                {t("tedxTitle")}
              </h3>
            </div>
            <p className="text-neutral-400 leading-relaxed text-sm">
              {t("tedxDesc")}
            </p>
          </div>
        </div>

        <div className="bg-dark-glass border-t border-r border-dark-border border-l-2 border-b-2 border-l-brand-500 border-b-brand-500 rounded-2xl p-6 sm:p-8 flex flex-col sm:flex-row gap-5 items-start">
          <div className="mt-1">
            <CheckCircle2 className="size-6 text-brand-500" />
          </div>
          <div>
            <h4 className="text-sm font-bold text-white uppercase tracking-[0.15em] mb-2">
              {t("licenseTitle")}
            </h4>
            <p className="text-neutral-400 leading-relaxed text-sm">
              {t("licenseDesc")}
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}
