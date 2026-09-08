"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useTranslations } from "next-intl";
import { Menu, X, ArrowRight } from "lucide-react";
import { cn } from "@/lib/utils";

export function Navbar() {
  const t = useTranslations("nav.nav");
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => setIsScrolled(window.scrollY > 50);
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <nav
      className={cn(
        "fixed w-full z-50 py-4 transition-all duration-300 border-b-0",
        isScrolled ? "bg-[#080808]/80 backdrop-blur-md" : "bg-transparent",
      )}
    >
      <div className="max-w-7xl mx-auto px-6 flex justify-between items-center">
        <Link href="/" className="flex flex-col group">
          <div className="flex items-baseline">
            <span className="text-brand-500 font-black text-2xl tracking-tighter">
              TEDx
            </span>
            <span className="text-white font-bold text-2xl tracking-tight ml-1 group-hover:text-neutral-200 transition-colors">
              Alkawmia
            </span>
          </div>
          <span className="text-[10px] text-neutral-400 font-medium tracking-wide mt-0.5">
            x = independently organized TED event
          </span>
        </Link>

        <div className="hidden md:flex space-x-8 items-center">
          <Link
            href="#possibilities"
            className="text-sm font-medium text-neutral-400 hover:text-white transition-colors"
          >
            {t("whoWeAre")}
          </Link>
          <Link
            href="#doors"
            className="text-sm font-medium text-neutral-400 hover:text-white transition-colors"
          >
            {t("theDoors")}
          </Link>
          <Link
            href="#speakers"
            className="text-sm font-medium text-neutral-400 hover:text-white transition-colors"
          >
            {t("speakers")}
          </Link>
          <Link
            href="#echoes"
            className="text-sm font-medium text-neutral-400 hover:text-white transition-colors"
          >
            {t("echoes")}
          </Link>
          <Link
            href="#sponsors"
            className="text-sm font-medium text-neutral-400 hover:text-white transition-colors"
          >
            {t("partners")}
          </Link>
        </div>

        <div className="flex items-center space-x-4">
          <button className="hidden lg:flex text-xs font-semibold text-neutral-400 hover:text-white transition-colors uppercase">
            {t("lang")}
          </button>
          <Link
            href="#doors"
            className="relative inline-flex h-10 items-center justify-center overflow-hidden rounded-full bg-brand-500 px-6 font-medium text-white transition-all duration-300 hover:bg-brand-glow animate-pulse-glow group"
          >
            <span className="mr-2 text-sm relative z-10">
              {t("getTickets")}
            </span>
            <ArrowRight className="size-4 relative z-10 group-hover:translate-x-1 transition-transform" />
          </Link>

          <button
            className="md:hidden text-white"
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
          >
            {isMobileMenuOpen ? (
              <X className="size-6" />
            ) : (
              <Menu className="size-6" />
            )}
          </button>
        </div>
      </div>

      {isMobileMenuOpen && (
        <div className="md:hidden absolute top-full left-0 w-full bg-[#080808]/95 backdrop-blur-lg border-b border-dark-border py-4 px-6 flex flex-col space-y-4 shadow-xl">
          <Link
            href="#possibilities"
            className="text-sm font-medium text-white"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            {t("whoWeAre")}
          </Link>
          <Link
            href="#doors"
            className="text-sm font-medium text-white"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            {t("theDoors")}
          </Link>
          <Link
            href="#speakers"
            className="text-sm font-medium text-white"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            {t("speakers")}
          </Link>
          <Link
            href="#echoes"
            className="text-sm font-medium text-white"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            {t("echoes")}
          </Link>
          <Link
            href="#sponsors"
            className="text-sm font-medium text-white"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            {t("partners")}
          </Link>
        </div>
      )}

      <div className="glow-line" />
    </nav>
  );
}
