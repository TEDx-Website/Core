"use client";

import { useState, useEffect } from "react";
import { useTranslations } from "next-intl";
import Image from "next/image";
import Link from "next/link";
import {
  ArrowLeft,
  ArrowRight,
  // MicStage,
  Ticket,
  // BookmarkSimple,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/shared/ui/button";

const SPEAKERS_IMAGES = [
  {
    img: "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=1400&q=85",
    thumb:
      "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=120&q=70",
  },
  {
    img: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=1400&q=85",
    thumb:
      "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=120&q=70",
  },
  {
    img: "https://images.unsplash.com/photo-1580489944761-15a19d654956?auto=format&fit=crop&w=1400&q=85",
    thumb:
      "https://images.unsplash.com/photo-1580489944761-15a19d654956?auto=format&fit=crop&w=120&q=70",
  },
  {
    img: "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=1400&q=85",
    thumb:
      "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=70",
  },
  {
    img: "https://images.unsplash.com/photo-1492562080023-ab3db95bfbce?auto=format&fit=crop&w=1400&q=85",
    thumb:
      "https://images.unsplash.com/photo-1492562080023-ab3db95bfbce?auto=format&fit=crop&w=120&q=70",
  },
];

export function SpeakersSection() {
  const t = useTranslations("landing.speakers");
  const TOTAL_SLIDES = 5;
  const AUTOPLAY_MS = 6500;

  const [current, setCurrent] = useState(0);
  const [isPaused, setIsPaused] = useState(false);

  useEffect(() => {
    if (isPaused) return;
    const timer = setTimeout(() => {
      setCurrent((prev) => (prev + 1) % TOTAL_SLIDES);
    }, AUTOPLAY_MS);
    return () => clearTimeout(timer);
  }, [current, isPaused]);

  const goToSlide = (idx: number) => {
    setCurrent(((idx % TOTAL_SLIDES) + TOTAL_SLIDES) % TOTAL_SLIDES);
  };

  return (
    <section
      id="speakers"
      className="relative overflow-hidden h-[92vh] min-h-155 max-h-225 bg-dark-obsidian"
      onMouseEnter={() => setIsPaused(true)}
      onMouseLeave={() => setIsPaused(false)}
    >
      <div className="absolute top-10 left-6 md:left-14 z-20 flex items-center gap-3 animate-in fade-in duration-1000">
        <span className="w-1.5 h-1.5 rounded-full bg-brand-500 animate-pulse" />
        <span className="text-[10px] font-bold tracking-[0.24em] uppercase text-brand-500">
          {t("sectionLabel")}
        </span>
        <span className="w-6 h-px bg-dark-border" />
        <span className="text-white/45 font-bold text-[1.1rem] tracking-tight">
          {t("brandName")}
        </span>
      </div>

      <div className="absolute top-11 left-1/2 -translate-x-1/2 z-20 text-[11px] font-semibold text-neutral-500 tracking-widest tabular-nums">
        <span className="text-brand-500">
          {String(current + 1).padStart(2, "0")}
        </span>{" "}
        / {String(TOTAL_SLIDES).padStart(2, "0")}
      </div>

      <div
        className="flex w-full h-full transition-transform duration-720 ease-[cubic-bezier(0.77,0,0.175,1)]"
        style={{ transform: `translateX(-${current * 100}%)` }}
      >
        {SPEAKERS_IMAGES.map((speaker, index) => {
          const isActive = index === current;

          return (
            <div key={index} className="min-w-full relative flex items-stretch">
              <div className="absolute inset-0 overflow-hidden">
                <Image
                  src={speaker.img}
                  alt="Speaker"
                  fill
                  className={cn(
                    "object-cover object-top grayscale-20 contrast-110 transition-transform duration-8000 ease-out",
                    isActive ? "scale-100" : "scale-105",
                  )}
                />
                <div className="absolute inset-0 bg-linear-to-br from-dark-obsidian/95 via-dark-obsidian/80 to-dark-obsidian/5" />
                <div className="absolute inset-0 bg-linear-to-t from-dark-obsidian to-transparent to-35% h-[35%] mt-auto" />

                <div
                  className={cn(
                    "absolute inset-0 bg-[radial-gradient(ellipse_55%_70%_at_62%_40%,rgba(235,0,40,0.18)_0%,transparent_65%)] mix-blend-screen transition-opacity duration-1000",
                    isActive ? "opacity-100" : "opacity-0",
                  )}
                />
              </div>

              <div className="relative z-10 w-full flex flex-col justify-end p-8 md:p-14 pb-28 md:pb-16">
                <div
                  className={cn(
                    "inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full border border-brand-500/45 bg-brand-500/10 text-[10px] font-bold tracking-[0.18em] uppercase text-brand-neon mb-5 backdrop-blur-md w-fit transition-all duration-500 delay-150",
                    isActive
                      ? "opacity-100 translate-y-0"
                      : "opacity-0 translate-y-5",
                  )}
                >
                  <div className="w-1.5 h-1.5 bg-brand-500 rounded-full animate-pulse" />
                  {t(`list.${index}.field`)}
                </div>

                <div
                  className={cn(
                    "absolute top-10 right-5 md:right-12 text-[clamp(4rem,10vw,8rem)] font-extrabold leading-none tracking-tight select-none pointer-events-none transition-colors duration-500",
                    isActive ? "text-brand-500/20" : "text-brand-500/10",
                  )}
                >
                  0{index + 1}
                </div>

                <h3
                  className={cn(
                    "text-[clamp(2.4rem,5.5vw,4.5rem)] font-extrabold leading-[0.95] tracking-tight text-white mb-2 transition-all duration-700 delay-200",
                    isActive
                      ? "opacity-100 translate-y-0"
                      : "opacity-0 translate-y-7",
                  )}
                >
                  {t(`list.${index}.nameLine1`)}
                  <br />
                  {t(`list.${index}.nameLine2`)}
                </h3>

                <p
                  className={cn(
                    "text-sm font-medium text-neutral-400 tracking-wide mb-6 transition-all duration-700 delay-300",
                    isActive
                      ? "opacity-100 translate-y-0"
                      : "opacity-0 translate-y-5",
                  )}
                >
                  {t(`list.${index}.title`)}
                </p>

                <div
                  className={cn(
                    "h-px bg-linear-to-r from-brand-500 to-brand-500/20 mb-6 transition-all duration-800 delay-300",
                    isActive ? "w-[min(360px,60%)]" : "w-0",
                  )}
                />

                <div
                  className={cn(
                    "text-[10px] font-bold tracking-widest uppercase text-brand-500 mb-1.5 flex items-center transition-opacity duration-500 delay-400",
                    isActive ? "opacity-100" : "opacity-0",
                  )}
                >
                  {/* <MicStage className="size-3 mr-1.5" /> {t("keynote")} */}
                </div>

                <h4
                  className={cn(
                    "text-[clamp(1.05rem,2vw,1.35rem)] font-bold text-white leading-snug max-w-130 mb-6 transition-all duration-500 delay-500",
                    isActive
                      ? "opacity-100 translate-y-0"
                      : "opacity-0 translate-y-3.5",
                  )}
                >
                  {t(`list.${index}.topic`)}
                </h4>

                <p
                  className={cn(
                    "text-sm text-neutral-400 leading-relaxed max-w-[480px] mb-8 transition-all duration-500 delay-600",
                    isActive
                      ? "opacity-100 translate-y-0"
                      : "opacity-0 translate-y-3",
                  )}
                >
                  {t(`list.${index}.bio`)}
                </p>

                <div
                  className={cn(
                    "flex items-center gap-4 transition-all duration-500 delay-700",
                    isActive
                      ? "opacity-100 translate-y-0"
                      : "opacity-0 translate-y-3",
                  )}
                >
                  <Link
                    href="#doors"
                    className="inline-flex items-center gap-2 px-6 py-3 bg-brand-500 hover:bg-brand-glow text-white text-xs font-bold tracking-[0.08em] rounded-lg shadow-[0_0_24px_rgba(235,0,40,0.45)] hover:shadow-[0_0_38px_rgba(235,0,40,0.65)] hover:-translate-y-0.5 transition-all"
                  >
                    <Ticket className="size-4" /> {t("reserveBtn")}
                  </Link>
                  <Button
                    variant="outline"
                    className="h-11 px-5 border-white/10 bg-white/4 hover:bg-white/10 hover:border-white/20 hover:text-white text-neutral-400 text-xs font-semibold tracking-[0.06em]"
                  >
                    {/* <BookmarkSimple className="size-4" /> {t("saveBtn")} */}
                  </Button>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      <div className="absolute top-1/2 right-10 -translate-y-1/2 z-20 hidden md:flex flex-col gap-2.5">
        {SPEAKERS_IMAGES.map((spk, idx) => (
          <button
            key={idx}
            onClick={() => goToSlide(idx)}
            className={cn(
              "w-13 h-15.5 rounded-md overflow-hidden border-[1.5px] transition-all duration-300",
              idx === current
                ? "border-brand-500 shadow-[0_0_14px_rgba(235,0,40,0.5)] opacity-100 scale-105"
                : "border-white/10 opacity-45 hover:opacity-70 hover:scale-105",
            )}
          >
            <Image
              src={spk.thumb}
              alt={`Thumb ${idx}`}
              width={52}
              height={62}
              className="object-cover w-full h-full grayscale-50"
            />
          </button>
        ))}
      </div>

      <div className="absolute bottom-10 right-6 md:right-12 z-20 flex flex-col items-end gap-5">
        <div className="flex gap-2 items-center">
          {SPEAKERS_IMAGES.map((_, idx) => (
            <button
              key={idx}
              onClick={() => goToSlide(idx)}
              className={cn(
                "h-1.5 rounded-full transition-all duration-300",
                idx === current
                  ? "w-7 bg-brand-500 shadow-[0_0_10px_rgba(235,0,40,0.6)]"
                  : "w-1.5 bg-white/20 hover:bg-white/40",
              )}
              aria-label={`Go to slide ${idx + 1}`}
            />
          ))}
        </div>
        <div className="flex gap-2.5">
          <button
            onClick={() => goToSlide(current - 1)}
            className="w-11 h-11 rounded-full border border-white/10 bg-dark-obsidian/75 backdrop-blur-md text-white flex items-center justify-center hover:border-brand-500 hover:bg-brand-500/15 hover:scale-110 transition-all"
          >
            <ArrowLeft className="size-4" />
          </button>
          <button
            onClick={() => goToSlide(current + 1)}
            className="w-11 h-11 rounded-full border border-white/10 bg-dark-obsidian/75 backdrop-blur-md text-white flex items-center justify-center hover:border-brand-500 hover:bg-brand-500/15 hover:scale-110 transition-all"
          >
            <ArrowRight className="size-4" />
          </button>
        </div>
      </div>

      <div
        key={current} 
        className="absolute bottom-0 left-0 h-.5 bg-brand-500 shadow-[0_0_12px_rgba(235,0,40,0.7)] z-20 animate-progress-bar"
        style={{ animationPlayState: isPaused ? "paused" : "running" }}
      />
    </section>
  );
}
