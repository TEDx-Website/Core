"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Button } from "@/shared/ui/button";
import { Menu, X } from "lucide-react";
import { useUserStore } from "@/shared/store/use-user-store"; 

interface UserData {
  firstName?: string;
  lastName?: string;
  profilePictureUrl?: string;
}

interface NavbarClientProps {
  user: UserData | null;
}

export function NavbarClient({ user: serverUser }: NavbarClientProps) {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const storeUser = useUserStore((state) => state.user);
  const setUser = useUserStore((state) => state.setUser);

  useEffect(() => {
    if (serverUser && !storeUser) {
      setUser(serverUser);
    }
  }, [serverUser, storeUser, setUser]);

  const currentUser = storeUser || serverUser;
  const isAuthenticated = !!currentUser;

  const initials = currentUser
    ? `${currentUser.firstName?.[0] || ""}${currentUser.lastName?.[0] || ""}`.toUpperCase()
    : "";

  return (
    <header className="fixed top-0 left-0 w-full z-50 bg-neutral-100 border-b border-neutral-200">
      <div className="mx-auto max-w-360 px-4 md:px-[32px] lg:px-[80px] h-17.5 flex items-center justify-between">
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
            href="/about"
            className="text-sm font-semibold text-neutral-600 hover:text-black transition-colors"
          >
            About
          </Link>
          <Link
            href="/team"
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

          {!isAuthenticated ? (
            <Link
              href="/login"
              prefetch={false}
              className="text-sm font-bold text-black hover:opacity-80 transition-opacity ml-4"
            >
              Login
            </Link>
          ) : (
            <Link
              href="/profile"
              className="flex items-center gap-2 ml-4 p-1 rounded-full hover:bg-neutral-200 transition-colors"
            >
              <div className="w-8 h-8 rounded-full bg-neutral-900 text-white flex items-center justify-center font-bold text-xs overflow-hidden border border-neutral-300 shadow-sm">
                {currentUser?.profilePictureUrl ? (
                  <img
                    src={currentUser.profilePictureUrl}
                    alt="Profile"
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <span>{initials || "U"}</span>
                )}
              </div>
            </Link>
          )}

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

      {/* Mobile Menu */}
      {isMobileMenuOpen && (
        <div className="lg:hidden absolute top-[70px] left-0 w-full bg-white border-b border-neutral-200 shadow-xl flex flex-col px-6 py-6 gap-6">
          <Link
            href="#events"
            className="text-base font-semibold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            Events
          </Link>
          <Link
            href="/about"
            className="text-base font-semibold text-neutral-900"
            onClick={() => setIsMobileMenuOpen(false)}
          >
            About
          </Link>
          <Link
            href="/team"
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

          {!isAuthenticated ? (
            <Link
              href="/login"
              prefetch={false}
              className="text-base font-bold text-neutral-900"
              onClick={() => setIsMobileMenuOpen(false)}
            >
              Login
            </Link>
          ) : (
            <Link
              href="/profile"
              className="flex items-center gap-3 text-base font-bold text-neutral-900"
              onClick={() => setIsMobileMenuOpen(false)}
            >
              <div className="w-8 h-8 rounded-full bg-neutral-900 text-white flex items-center justify-center font-bold text-xs overflow-hidden border border-neutral-300">
                {currentUser?.profilePictureUrl ? (
                  <img
                    src={currentUser.profilePictureUrl}
                    alt="Profile"
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <span>{initials || "U"}</span>
                )}
              </div>
              <span>Profile</span>
            </Link>
          )}

          <Button className="w-full bg-brand-500 hover:bg-brand-600 text-white rounded-full h-12 text-sm font-bold tracking-widest uppercase mt-2">
            BOOK NOW
          </Button>
        </div>
      )}
    </header>
  );
}
