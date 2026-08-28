"use client";

import { useState } from "react";
import Link from "next/link";
import { Button } from "@/shared/ui/button";
import { Menu, X } from "lucide-react";

export function Navbar() {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  return (
    <header className="fixed top-0 left-0 w-full z-50 bg-neutral-100   border-b border-neutral-200">
      <div className="mx-auto max-w-360 px-4 md:px-[32px] lg:px-[80px] h-[80px] flex items-center justify-between">
        <Link
          href="/"
          className="text-xl md:text-2xl font-bold tracking-tight text-neutral-900"
        >
          TEDx Alkawmia
        </Link>

        <nav className="hidden lg:flex items-center gap-8">
          <Link
            href="#events"
            className="text-sm font-semibold text-neutral-600 hover:text-black transition-colors"
          >
            Events
          </Link>
          <Link
            href="about"
            className="text-sm font-semibold text-neutral-600 hover:text-black transition-colors"
          >
            About
          </Link>
          <Link
            href="team"
            className="text-sm font-semibold text-neutral-600 hover:text-black transition-colors"
          >
            Team
          </Link>
          <Link
            href="#contact"
            className="text-sm font-semibold text-neutral-600 hover:text-black transition-colors"
          >
            Contact
          </Link>
          <Link
            href="/login"
            className="text-sm font-bold text-black hover:opacity-80 transition-opacity ml-4"
          >
            Login
          </Link>

          <Button className="bg-brand-500 hover:bg-brand-600 text-white rounded-full px-6 h-10 text-xs font-bold tracking-widest uppercase ml-2">
            BOOK NOW
          </Button>
        </nav>

        <button
          className="lg:hidden p-2 text-neutral-900"
          onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
        >
          {isMobileMenuOpen ? <X size={28} /> : <Menu size={28} />}
        </button>
      </div>

      {isMobileMenuOpen && (
        <div className="lg:hidden absolute top-[80px] left-0 w-full bg-white border-b border-neutral-200 shadow-xl flex flex-col px-6 py-6 gap-6">
          <Link
            href="#events"
            className="text-base font-semibold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            Events
          </Link>
          <Link
            href="#about"
            className="text-base font-semibold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            About
          </Link>
          <Link
            href="#team"
            className="text-base font-semibold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            Team
          </Link>
          <Link
            href="#contact"
            className="text-base font-semibold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            Contact
          </Link>
          <hr className="border-neutral-100" />
          <Link
            href="/login"
            className="text-base font-bold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            Login
          </Link>
          <Button className="w-full bg-brand-500 hover:bg-brand-600 text-white rounded-full h-12 text-sm font-bold tracking-widest uppercase mt-2">
            BOOK NOW
          </Button>
        </div>
      )}
    </header>
  );
}
