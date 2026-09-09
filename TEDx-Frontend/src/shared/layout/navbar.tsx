"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useTranslations } from "next-intl";
import { Menu, X, User } from "lucide-react";
import { cn } from "@/lib/utils";
import { useUserStore } from "@/shared/store/use-user-store";

export function Navbar() {
  const t = useTranslations("nav");
  const pathname = usePathname();
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const { user } = useUserStore();
  const isAuthenticated = !!user;

  useEffect(() => {
    const handleScroll = () => setIsScrolled(window.scrollY > 20);
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  const navLinks = [
    { href: "/speakers", label: t("speakers") },
    { href: "/about", label: t("about") },
    { href: "/contact", label: t("contact") },
  ];

  const isActive = (href: string) => pathname.includes(href);

  return (
    <header
      className={cn(
        "fixed top-0 z-50 w-full transition-all duration-300",
        isScrolled
          ? "bg-dark-obsidian/80 backdrop-blur-xl border-b border-dark-border"
          : "bg-transparent border-b border-transparent",
      )}
    >
      <div className="max-w-7xl mx-auto px-6 sm:px-12 h-20 flex items-center justify-between">
        <div className="flex-shrink-0 flex items-center">
          <Link href="/" className="flex flex-col group">
            <div className="flex items-baseline">
              <span className="text-brand-500 font-black text-2xl tracking-tighter">
                TEDx
              </span>
              <span className="text-white font-bold text-2xl tracking-tight ml-1 group-hover:text-neutral-200 transition-colors">
                AlKawmia
              </span>
            </div>
            <span className="text-[9px] text-neutral-400 font-mono font-medium tracking-wider -mt-1">
              x = independently organized TED event
            </span>
          </Link>
        </div>

        <nav className="hidden lg:flex items-center gap-6 xl:gap-8">
          {navLinks.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className={cn(
                "text-sm font-semibold transition-colors duration-200 tracking-wide",
                isActive(link.href)
                  ? "text-white relative after:content-[''] after:absolute after:-bottom-2 after:left-1/2 after:-translate-x-1/2 after:w-full after:h-[2px] after:bg-brand-500 after:rounded-full"
                  : "text-neutral-400 hover:text-white",
              )}
            >
              {link.label}
            </Link>
          ))}
        </nav>

        <div className="flex items-center gap-3 sm:gap-5">
          {isAuthenticated ? (
            <Link
              href="/profile"
              className="inline-flex items-center justify-center gap-2 text-xs sm:text-sm font-bold uppercase tracking-wider text-white bg-dark-glass border border-dark-border hover:border-brand-500 transition-all duration-300 px-4 py-2 sm:px-5 sm:py-2.5 rounded-full"
            >
              <User className="size-4" />
              <span className="hidden sm:inline">
                {user?.firstName || t("profile")}
              </span>
            </Link>
          ) : (
            <>
              <Link
                href="/login"
                className="hidden sm:block text-xs sm:text-sm font-semibold text-neutral-400 hover:text-white transition-colors duration-200 tracking-wide px-2 py-1"
              >
                {t("signIn")}
              </Link>
              <Link
                href="/login"
                className="text-xs sm:text-sm font-bold uppercase tracking-wider text-white bg-brand-500 hover:bg-brand-glow transition-all duration-300 px-4 py-2 sm:px-5 sm:py-2.5 rounded-full shadow-[0_0_20px_rgba(235,0,40,0.3)] hover:shadow-[0_0_30px_rgba(235,0,40,0.5)] hover:-translate-y-0.5 active:translate-y-0"
              >
                {t("cta")}
              </Link>
            </>
          )}

          <button
            className="lg:hidden text-neutral-300 hover:text-white p-2 ml-1"
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
        <div className="lg:hidden absolute top-full left-0 w-full bg-dark-obsidian/95 backdrop-blur-xl border-b border-dark-border py-6 px-6 flex flex-col gap-6 shadow-2xl">
          {navLinks.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              onClick={() => setIsMobileMenuOpen(false)}
              className={cn(
                "text-lg font-semibold transition-colors",
                isActive(link.href) ? "text-brand-500" : "text-white",
              )}
            >
              {link.label}
            </Link>
          ))}
          {!isAuthenticated && (
            <Link
              href="/login"
              onClick={() => setIsMobileMenuOpen(false)}
              className="text-lg font-semibold text-neutral-400"
            >
              {t("signIn")}
            </Link>
          )}
        </div>
      )}
    </header>
  );
}
