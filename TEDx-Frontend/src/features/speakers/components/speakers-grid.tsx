"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import Image from "next/image";
import { Search, ArrowRight, X, Globe
    // Linkedin, Twitter
 } from "lucide-react";
import { cn } from "@/lib/utils";

export function SpeakersGrid() {
  const t = useTranslations("speakers.grid");
  const [activeFilter, setActiveFilter] = useState("all");
  const [searchQuery, setSearchQuery] = useState("");
  const [hoveredId, setHoveredId] = useState<string | null>(null);
  const [selectedSpeaker, setSelectedSpeaker] = useState<any | null>(null);

  const filters = ["all", "tech", "human", "design", "youth"];
  const speakerCount = 6;
  const speakers = Array.from({ length: speakerCount }).map((_, idx) => ({
    id: t(`list.${idx}.id`),
    category: t(`list.${idx}.category`),
    door: t(`list.${idx}.door`),
    name: t(`list.${idx}.name`),
    track: t(`list.${idx}.track`),
    talkTitle: t(`list.${idx}.talkTitle`),
    shortDesc: t(`list.${idx}.shortDesc`),
    role: t(`list.${idx}.role`),
    bio: t(`list.${idx}.bio`),
    index: String(idx + 1).padStart(2, "0"),
  }));

  const filteredSpeakers = speakers.filter((speaker) => {
    const matchesFilter =
      activeFilter === "all" || speaker.category === activeFilter;
    const matchesSearch =
      speaker.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      speaker.talkTitle.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesFilter && matchesSearch;
  });

  return (
    <section className="py-20 px-6 sm:px-12 bg-dark-carbon w-full border-b border-dark-border">
      <div className="max-w-7xl mx-auto">
        <div className="flex flex-col sm:flex-row sm:items-end justify-between mb-8 animate-in fade-in slide-in-from-bottom-4 duration-1000">
          <div>
            <div className="flex items-center gap-3 mb-2">
              <div className="h-[1px] w-6 bg-brand-500" />
              <span className="text-brand-500 font-mono text-xs font-bold tracking-[0.25em] uppercase">
                {t("eyebrow")}
              </span>
            </div>
            <h2 className="font-heading text-3xl sm:text-4xl font-extrabold uppercase tracking-tight text-white">
              {t("title")}
            </h2>
          </div>
          <p className="text-neutral-400 text-xs sm:text-sm font-light mt-2 sm:mt-0 font-mono">
            {t("showing")}{" "}
            <span className="text-white font-bold">
              {String(filteredSpeakers.length).padStart(2, "0")}
            </span>{" "}
            {t("of")} {String(speakerCount).padStart(2, "0")} {t("speakers")}
          </p>
        </div>

        <div className="bg-dark-glass border border-dark-border rounded-2xl p-4 sm:p-5 flex flex-col md:flex-row items-center justify-between gap-4 mb-10 shadow-2xl animate-in fade-in slide-in-from-bottom-4 duration-1000 delay-150">
          <div className="flex items-center gap-2 overflow-x-auto w-full md:w-auto pb-2 md:pb-0 scrollbar-none">
            {filters.map((filter) => (
              <button
                key={filter}
                onClick={() => setActiveFilter(filter)}
                className={cn(
                  "px-4 py-2 rounded-full border text-xs font-mono font-medium tracking-wider whitespace-nowrap transition-all",
                  activeFilter === filter
                    ? "bg-brand-500 text-white border-brand-500 shadow-[0_0_20px_rgba(235,0,40,0.35)]"
                    : "border-dark-border text-neutral-400 hover:text-white",
                )}
              >
                {t(`filters.${filter}`)}{" "}
                {filter === "all" &&
                  `(${String(speakerCount).padStart(2, "0")})`}
              </button>
            ))}
          </div>

          <div className="relative w-full md:w-72 flex-shrink-0">
            <Search className="absolute left-3.5 top-1/2 -translate-y-1/2 size-3.5 text-neutral-500" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder={t("search")}
              className="w-full bg-dark-obsidian border border-dark-border rounded-full py-2 pl-9 pr-4 text-xs text-white placeholder-neutral-500 focus:outline-none focus:border-brand-500 transition-colors"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 lg:gap-7 animate-in fade-in slide-in-from-bottom-8 duration-1000 delay-300">
          {filteredSpeakers.map((speaker) => {
            const isDimmed = hoveredId !== null && hoveredId !== speaker.id;

            return (
              <div
                key={speaker.id}
                onMouseEnter={() => setHoveredId(speaker.id)}
                onMouseLeave={() => setHoveredId(null)}
                onClick={() => setSelectedSpeaker(speaker)}
                className="group relative overflow-hidden bg-[#0C0C0E] cursor-pointer rounded-[24px] border border-[#1E1E22] h-[500px] flex flex-col justify-end p-7 transition-all duration-400 hover:border-brand-500/55 hover:shadow-[0_20px_45px_-15px_rgba(235,0,40,0.3)] hover:-translate-y-1"
              >
                <Image
                  src={`/assets/${speaker.id}.jpeg`}
                  alt={speaker.name}
                  fill
                  sizes="(max-width: 768px) 100vw, (max-width: 1024px) 50vw, 33vw"
                  className={cn(
                    "object-cover object-[center_top] scale-[1.06] transition-all duration-[1100ms] ease-out",
                    isDimmed
                      ? "grayscale contrast-[1.3] brightness-[0.18]"
                      : "grayscale contrast-125 brightness-50 group-hover:grayscale-0 group-hover:contrast-[1.12] group-hover:brightness-100 group-hover:scale-100",
                  )}
                />

                <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(8,8,8,0.15)_0%,rgba(8,8,8,0.4)_45%,rgba(8,8,8,0.96)_100%)] pointer-events-none z-10 transition-colors duration-500 group-hover:bg-[linear-gradient(180deg,rgba(8,8,8,0.05)_0%,rgba(8,8,8,0.25)_40%,rgba(8,8,8,0.92)_100%)]" />

                <div className="absolute left-0 right-0 bottom-0 h-[3px] bg-brand-500 scale-x-0 origin-left transition-transform duration-600 z-20 group-hover:scale-x-100" />

                <span className="absolute top-5 left-6 z-20 font-mono text-[11px] font-bold text-white/35 transition-colors duration-400 group-hover:text-brand-500">
                  {speaker.index}
                </span>

                <span className="absolute top-[18px] right-5 z-20 bg-[#080808]/80 backdrop-blur-md border border-white/10 px-3 py-1 rounded-full font-mono text-[10px] font-semibold tracking-[0.1em] uppercase text-neutral-400 transition-all duration-350 group-hover:border-brand-500/50 group-hover:text-white group-hover:bg-[#080808]/92">
                  {speaker.door}
                </span>

                <div className="relative z-20 translate-y-2.5 transition-transform duration-[450ms] ease-out group-hover:translate-y-0">
                  <b className="block font-heading font-bold text-2xl tracking-tight text-white mb-1 leading-[1.15]">
                    {speaker.name}
                  </b>
                  <span className="block font-mono text-[10px] font-bold tracking-[0.18em] uppercase text-brand-neon mb-2">
                    {speaker.track}
                  </span>
                  <p className="text-[13px] text-neutral-200 font-light italic leading-snug mb-1">
                    {speaker.talkTitle}
                  </p>
                  <p className="text-xs text-neutral-400 font-light leading-relaxed max-h-0 opacity-0 overflow-hidden transition-all duration-[450ms] ease-out group-hover:max-h-[75px] group-hover:opacity-100 group-hover:mt-1.5">
                    {speaker.shortDesc}
                  </p>
                  <div className="flex items-center gap-1.5 font-mono text-[11px] font-bold text-brand-500 uppercase tracking-[0.12em] mt-3 opacity-0 translate-y-1.5 transition-all duration-350 ease-out group-hover:opacity-100 group-hover:translate-y-0">
                    <span>{t("cta")}</span>
                    <ArrowRight className="size-2.5" />
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {selectedSpeaker && (
        <div className="fixed inset-0 z-50 bg-[#080808]/90 backdrop-blur-xl flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-300">
          <div className="bg-dark-glass border border-dark-border rounded-3xl max-w-2xl w-full p-6 sm:p-8 relative overflow-hidden shadow-2xl max-h-[90vh] overflow-y-auto animate-in zoom-in-95 duration-300">
            <button
              onClick={() => setSelectedSpeaker(null)}
              className="absolute top-6 right-6 w-9 h-9 rounded-full bg-dark-obsidian border border-dark-border flex items-center justify-center text-neutral-400 hover:text-white hover:border-brand-500 transition-colors z-20"
            >
              <X className="size-4" />
            </button>

            <div className="flex flex-col sm:flex-row items-start sm:items-center gap-5 mb-6 pr-10">
              <div className="relative w-20 h-20 sm:w-24 sm:h-24 rounded-2xl overflow-hidden border-2 border-brand-500/40 flex-shrink-0 shadow-lg">
                <Image
                  src={`/assets/${selectedSpeaker.id}.webp`}
                  alt={selectedSpeaker.name}
                  fill
                  className="object-cover object-top"
                />
              </div>
              <div>
                <div className="flex items-center gap-2 mb-1.5">
                  <span className="w-2 h-2 rounded-full bg-brand-500 animate-pulse" />
                  <span className="text-brand-500 font-mono text-[10px] font-bold tracking-widest uppercase">
                    TRACK // {selectedSpeaker.category}
                  </span>
                </div>
                <h3 className="font-heading text-2xl sm:text-3xl font-extrabold uppercase tracking-tight text-white mb-0.5">
                  {selectedSpeaker.name}
                </h3>
                <p className="text-xs font-mono text-neutral-400">
                  {selectedSpeaker.role}
                </p>
              </div>
            </div>

            <div className="bg-dark-carbon border border-dark-border rounded-2xl p-5 mb-6">
              <p className="text-[10px] font-mono text-brand-500 font-bold uppercase tracking-wider mb-1">
                Stage Talk Title
              </p>
              <h4 className="font-heading text-base sm:text-lg font-bold text-white italic">
                {selectedSpeaker.talkTitle}
              </h4>
            </div>

            <div className="space-y-4 text-xs sm:text-sm text-neutral-400 font-light leading-relaxed mb-6">
              <p>{selectedSpeaker.bio}</p>
            </div>

            <div className="flex flex-wrap items-center justify-between gap-4 pt-4 border-t border-dark-border/50">
              <div className="flex items-center gap-3 text-sm text-neutral-400">
                <span className="text-xs font-mono text-neutral-500">
                  Follow:
                </span>
                <button className="hover:text-brand-500 transition-colors">
                  {/* <Linkedin className="size-4" /> */}
                </button>
                <button className="hover:text-brand-500 transition-colors">
                  {/* <Twitter className="size-4" /> */}
                </button>
                <button className="hover:text-brand-500 transition-colors">
                  <Globe className="size-4" />
                </button>
              </div>
              <button
                onClick={() => setSelectedSpeaker(null)}
                className="text-xs font-mono font-bold uppercase tracking-wider text-neutral-300 hover:text-white px-5 py-2.5 rounded-full border border-dark-border hover:border-neutral-500 transition-colors"
              >
                Close Profile
              </button>
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
