import Link from "next/link";
import { useTranslations } from "next-intl";
import { GridContainer } from "@/shared/ui/grid-container";

export function Footer() {
  const t = useTranslations("landing.Footer");

  return (
    <footer className="w-full py-[80px] md:py-25 bg-neutral-1000 text-white border-t border-neutral-800 transition-colors duration-300">
      <GridContainer className="gap-y-12 justify-between">
        <div className="col-span-4 md:col-span-8 lg:col-span-6 flex flex-col gap-3">
          <h3 className="text-2xl font-bold tracking-tight text-white">
            {t("brandName")}
          </h3>
          <p className="text-sm text-neutral-400 max-w-sm leading-relaxed">
            {t("slogan")}
          </p>
        </div>

        <div className="col-span-4 md:col-span-8 lg:col-span-6 grid grid-cols-3 gap-6 lg:justify-end">
          <div className="flex flex-col gap-4">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-500 uppercase">
              {t("columns.discover.title")}
            </span>
            <ul className="flex flex-col gap-3">
              <li>
                <Link
                  href="#events"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.discover.events")}
                </Link>
              </li>
              <li>
                <Link
                  href="#about"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.discover.about")}
                </Link>
              </li>
              <li>
                <Link
                  href="#team"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.discover.team")}
                </Link>
              </li>
            </ul>
          </div>

          <div className="flex flex-col gap-4">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-500 uppercase">
              {t("columns.connect.title")}
            </span>
            <ul className="flex flex-col gap-3">
              <li>
                <Link
                  href="#contact"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.connect.contact")}
                </Link>
              </li>
              <li>
                <Link
                  href="https://instagram.com"
                  target="_blank"
                  rel="noreferrer"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.connect.instagram")}
                </Link>
              </li>
              <li>
                <Link
                  href="https://linkedin.com"
                  target="_blank"
                  rel="noreferrer"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.connect.linkedin")}
                </Link>
              </li>
            </ul>
          </div>

          <div className="flex flex-col gap-4">
            <span className="text-xs font-bold tracking-[0.15em] text-brand-500 uppercase">
              {t("columns.account.title")}
            </span>
            <ul className="flex flex-col gap-3">
              <li>
                <Link
                  href="/login"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.account.login")}
                </Link>
              </li>
              <li>
                <Link
                  href="/tickets"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.account.myTickets")}
                </Link>
              </li>
              <li>
                <Link
                  href="/notifications"
                  className="text-sm text-neutral-400 hover:text-white transition-colors"
                >
                  {t("columns.account.notifications")}
                </Link>
              </li>
            </ul>
          </div>
        </div>
      </GridContainer>
    </footer>
  );
}
