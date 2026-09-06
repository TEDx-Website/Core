import { useTranslations } from "next-intl";
import Link from "next/link";
// import { Instagram, Twitter, Linkedin, Send } from "lucide-react";

export function Footer() {
  const t = useTranslations("footer.footer");

  return (
    <footer className="bg-dark-carbon border-t border-dark-border pt-16 pb-8">
      <div className="max-w-7xl mx-auto px-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-12 mb-16">
          <div className="col-span-1 md:col-span-2 lg:col-span-1">
            <div className="flex items-baseline mb-4">
              <span className="text-brand-500 font-black text-2xl tracking-tighter">
                TEDx
              </span>
              <span className="text-white font-bold text-2xl tracking-tight ml-1">
                Alkawmia
              </span>
            </div>
            <p className="text-sm text-neutral-400 mb-6">{t("aboutText")}</p>
            <div className="flex space-x-4">
              <Link
                href="#"
                className="w-10 h-10 rounded-full bg-dark-glass border border-dark-border flex items-center justify-center text-neutral-400 hover:text-white hover:border-brand-500 transition-all"
              >
                {/* <Instagram className="size-5" /> */}
              </Link>
              <Link
                href="#"
                className="w-10 h-10 rounded-full bg-dark-glass border border-dark-border flex items-center justify-center text-neutral-400 hover:text-white hover:border-brand-500 transition-all"
              >
                {/* <Twitter className="size-5" /> */}
              </Link>
              <Link
                href="#"
                className="w-10 h-10 rounded-full bg-dark-glass border border-dark-border flex items-center justify-center text-neutral-400 hover:text-white hover:border-brand-500 transition-all"
              >
                {/* <Linkedin className="size-5" /> */}
              </Link>
            </div>
          </div>

          <div>
            <h4 className="font-bold text-white mb-6 uppercase tracking-wider text-sm">
              {t("explore")}
            </h4>
            <ul className="space-y-3 text-sm text-neutral-400">
              <li>
                <Link
                  href="#possibilities"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("whoWeAre")}
                </Link>
              </li>
              <li>
                <Link
                  href="#doors"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("theDoors")}
                </Link>
              </li>
              <li>
                <Link
                  href="#speakers"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("speakerLineup")}
                </Link>
              </li>
              <li>
                <Link
                  href="#echoes"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("talkArchive")}
                </Link>
              </li>
            </ul>
          </div>

          <div>
            <h4 className="font-bold text-white mb-6 uppercase tracking-wider text-sm">
              {t("info")}
            </h4>
            <ul className="space-y-3 text-sm text-neutral-400">
              <li>
                <Link
                  href="#doors"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("getTickets")}
                </Link>
              </li>
              <li>
                <Link
                  href="#sponsors"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("partners")}
                </Link>
              </li>
              <li>
                <Link
                  href="#become-partner"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("becomeKeyholder")}
                </Link>
              </li>
              <li>
                <Link
                  href="#"
                  className="hover:text-brand-500 transition-colors"
                >
                  {t("contactUs")}
                </Link>
              </li>
            </ul>
          </div>

          <div>
            <h4 className="font-bold text-white mb-6 uppercase tracking-wider text-sm">
              {t("stayUpdated")}
            </h4>
            <p className="text-sm text-neutral-400 mb-4">
              {t("subscribeText")}
            </p>
            <form className="relative">
              <input
                type="email"
                placeholder={t("emailPlaceholder")}
                className="w-full bg-dark-obsidian border border-dark-border rounded-lg py-3 px-4 text-sm text-white focus:outline-none focus:border-brand-500 transition-colors"
              />
              <button
                type="button"
                className="absolute right-2 top-1/2 transform -translate-y-1/2 text-brand-500 hover:text-brand-glow"
              >
                {/* <Send className="size-5" /> */}
              </button>
            </form>
          </div>
        </div>

        <div className="border-t border-dark-border pt-8 flex flex-col md:flex-row justify-between items-center text-xs text-neutral-500">
          <p className="mb-4 md:mb-0 max-w-2xl text-center md:text-left">
            <strong className="text-neutral-400">{t("disclaimerBold")}</strong>
            <br />
            {t("disclaimerText")}
          </p>
          <div className="flex space-x-4">
            <span>&copy; 2026 TEDxAlkawmia. {t("allRightsReserved")}</span>
            <Link href="#" className="hover:text-neutral-300 transition-colors">
              {t("privacy")}
            </Link>
            <Link href="#" className="hover:text-neutral-300 transition-colors">
              {t("terms")}
            </Link>
          </div>
        </div>
      </div>
    </footer>
  );
}
