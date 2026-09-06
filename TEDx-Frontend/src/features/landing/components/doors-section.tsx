"use client";

import { useRef } from "react";
import { useTranslations } from "next-intl";
import Image from "next/image";
import {
  ArrowLeft,
  ArrowRight,
  ArrowDownRight,
  ArrowRight as ArrowRightIcon,
  Cpu,
  BookOpen,
  Atom,
  Globe,
} from "lucide-react";
import { Button } from "@/shared/ui/button"; 

const DOORS_MOCK_DATA = [
  {
    id: "01",
    img: "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=800&q=80",
    icon: Cpu,
    speakerImg:
      "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=120&q=80",
  },
  {
    id: "02",
    img: "https://images.unsplash.com/photo-1507842229450-76905959e3f5?auto=format&fit=crop&w=800&q=80",
    icon: BookOpen,
    speakerImg:
      "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&w=120&q=80",
  },
  {
    id: "03",
    img: "https://images.unsplash.com/photo-1462331940025-496dfbfc7564?auto=format&fit=crop&w=800&q=80",
    icon: Atom,
    speakerImg:
      "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=120&q=80",
  },
  {
    id: "04",
    img: "https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=800&q=80",
    icon: Globe,
    speakerImg:
      "https://images.unsplash.com/photo-1567532939604-b6b5b0db2604?auto=format&fit=crop&w=120&q=80",
  },
];

export function DoorsSection() {
  const t = useTranslations("landing.doors");
  const scrollRef = useRef<HTMLDivElement>(null);

  const scroll = (direction: "left" | "right") => {
    if (scrollRef.current) {
      const scrollAmount = window.innerWidth < 768 ? 320 : 400; 
      scrollRef.current.scrollBy({
        left: direction === "left" ? -scrollAmount : scrollAmount,
        behavior: "smooth",
      });
    }
  };

  return (
    <section
      id="doors"
      className="py-24 bg-dark-carbon relative border-y border-dark-border/50"
    >
      <div className="max-w-7xl mx-auto px-6">
        <div className="flex flex-col md:flex-row justify-between items-end mb-16 animate-in fade-in slide-in-from-bottom-8 duration-1000">
          <div>
            <h2 className="font-bold text-4xl md:text-5xl mb-4 text-white">
              {t("title")}
            </h2>
            <p className="text-neutral-400 text-lg">{t("subtitle")}</p>
          </div>
          <div className="hidden md:flex space-x-2 mt-6 md:mt-0">
            <button
              onClick={() => scroll("left")}
              className="w-10 h-10 rounded-full border border-dark-border flex items-center justify-center text-white hover:bg-white hover:text-black transition-colors"
            >
              <ArrowLeft className="size-5" />
            </button>
            <button
              onClick={() => scroll("right")}
              className="w-10 h-10 rounded-full border border-dark-border flex items-center justify-center text-white hover:bg-white hover:text-black transition-colors"
            >
              <ArrowRight className="size-5" />
            </button>
          </div>
        </div>

        <div
          ref={scrollRef}
          className="flex overflow-x-auto gap-6 pb-8 snap-x snap-mandatory [&::-webkit-scrollbar]:hidden [-ms-overflow-style:none] [scrollbar-none]"
        >
          {DOORS_MOCK_DATA.map((door, index) => {
            const Icon = door.icon;

            return (
              <div
                key={door.id}
                className="group relative perspective-[1600px] shrink-0 snap-center w-70 md:w-70 h-125 bg-[#060608] border-[1.5px] border-dark-border rounded-2xl hover:border-brand-500 hover:shadow-[0_0_40px_rgba(235,0,40,0.3),inset_0_0_35px_rgba(235,0,40,0.15)] transition-all duration-500 cursor-pointer"
              >
                <div className="absolute inset-0 p-6 flex flex-col justify-between z-0 bg-[#08080a] overflow-hidden rounded-2xl">
                  <div className="absolute inset-0 bg-[radial-gradient(circle_at_75%_25%,rgba(235,0,40,0.3)_0%,rgba(130,0,22,0.1)_50%,transparent_80%)] mix-blend-screen blur-[20px] opacity-0 group-hover:opacity-100 group-hover:scale-110 transition-all duration-700 pointer-events-none" />

                  <div className="relative z-10">
                    <div className="flex items-center justify-between mb-4">
                      <span className="px-2.5 py-1 rounded bg-brand-500/20 border border-brand-500/40 text-[10px] font-mono font-bold text-brand-neon">
                        {t("chamberPrefix")} {door.id}
                      </span>
                      <span className="text-[11px] text-neutral-400 font-mono">
                        {t("sessions")}
                      </span>
                    </div>

                    <h4 className="font-bold text-xl text-white mb-2 group-hover:text-brand-500 transition-colors">
                      {t(`tracks.${index}.name`)}
                    </h4>

                    <p className="text-xs text-neutral-300 font-light leading-relaxed mb-4">
                      {t(`tracks.${index}.desc`)}
                    </p>

                    <div className="p-2.5 rounded-lg bg-black/60 border border-white/10 mb-4 flex items-center gap-3">
                      <Image
                        src={door.speakerImg}
                        alt="Speaker"
                        width={28}
                        height={28}
                        className="rounded-full object-cover border border-brand-500/60"
                      />
                      <div className="text-[11px] leading-tight">
                        <span className="text-neutral-400 block text-[9px] uppercase tracking-wider">
                          {t("keynote")}
                        </span>
                        <span className="text-white font-medium">
                          {t(`tracks.${index}.speakerName`)}
                        </span>
                      </div>
                    </div>

                    <ul className="space-y-2 text-[11px] text-neutral-300 border-t border-white/10 pt-3">
                      {[1, 2, 3].map((num) => (
                        <li key={num} className="flex items-center">
                          <ArrowDownRight className="text-brand-500 mr-2 size-3" />
                          {t(`tracks.${index}.theme${num}`)}
                        </li>
                      ))}
                    </ul>
                  </div>

                  <div className="relative z-10 mt-auto pt-4">
                    <Button className="w-full h-11 text-xs shadow-[0_0_20px_rgba(235,0,40,0.4)]">
                      {t("unlockBtn")}
                    </Button>
                  </div>
                </div>

                <div className="absolute inset-0 origin-left [transform-3d] transition-all duration-800 ease-[cubic-bezier(0.16,1,0.3,1)] z-10 group-hover:transform-[rotateY(88deg)_translateZ(-10px)] group-hover:opacity-5 group-hover:shadow-[25px_0_45px_rgba(0,0,0,0.95)] border border-white/10 rounded-2xl overflow-hidden shadow-[4px_0_20px_rgba(0,0,0,0.9)] bg-dark-obsidian flex flex-col justify-between p-6">
                  <Image
                    src={door.img}
                    alt={`Track ${door.id}`}
                    fill
                    className="object-cover opacity-60 grayscale-40 contrast-125"
                  />

                  <div className="absolute inset-0 bg-linear-to-b from-dark-obsidian/90 via-dark-obsidian/40 to-dark-carbon/95 pointer-events-none" />

                  <div className="absolute inset-2 border border-brand-500/10 rounded-xl pointer-events-none bg-linear-to-b from-white/1 to-brand-500/2" />

                  <div className="absolute right-3.5 top-1/2 -translate-y-1/2 w-1.5 h-12 bg-brand-500 rounded-full shadow-[0_0_10px_#FF2B44] transition-all duration-300 group-hover:bg-white group-hover:shadow-[0_0_15px_#FF4D61]" />

                  <div className="relative z-20 flex justify-between items-start">
                    <span className="text-6xl font-bold text-white/90 drop-shadow-xl">
                      {door.id}
                    </span>
                    <div className="w-11 h-11 rounded-xl bg-dark-carbon/90 border border-dark-border/80 flex items-center justify-center shadow-lg">
                      <Icon className="size-5 text-neutral-300" />
                    </div>
                  </div>

                  <div className="relative z-20 mt-auto">
                    <h3 className="text-2xl font-bold text-white mb-6 drop-shadow-md">
                      {t(`tracks.${index}.name`)}
                    </h3>

                    <div className="flex items-center justify-between pt-4 border-t border-white/20">
                      <span className="text-[11px] font-semibold text-brand-500 uppercase tracking-wider flex items-center">
                        {t("pushToEnter")}{" "}
                        <ArrowRightIcon className="ml-1 size-3" />
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>

        <div className="flex md:hidden justify-center space-x-4 mt-6">
          <button
            onClick={() => scroll("left")}
            className="w-10 h-10 rounded-full border border-dark-border flex items-center justify-center text-white bg-dark-glass"
          >
            <ArrowLeft className="size-5" />
          </button>
          <button
            onClick={() => scroll("right")}
            className="w-10 h-10 rounded-full border border-dark-border flex items-center justify-center text-white bg-dark-glass"
          >
            <ArrowRight className="size-5" />
          </button>
        </div>
      </div>
    </section>
  );
}
