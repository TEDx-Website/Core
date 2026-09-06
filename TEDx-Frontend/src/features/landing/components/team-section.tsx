"use client";

import { useTranslations } from "next-intl";
import Link from "next/link";
import { Users, Palette, Cog, ArrowUpRight } from "lucide-react";

export function TeamSection() {
  const t = useTranslations("landing.team");

  return (
    <section className="py-24 relative border-t border-dark-border/50 bg-dark-obsidian overflow-hidden">
      <div className="absolute top-1/2 right-0 w-[50vw] h-[50vw] bg-brand-500/2 rounded-full blur-[120px] -translate-y-1/2 pointer-events-none" />
      <div className="absolute bottom-0 left-0 w-full h-px bg-linear-to-r from-transparent via-dark-border to-transparent" />

      <div className="max-w-7xl mx-auto px-6">
        
        {/* Header Area */}
        <div className="flex flex-col items-center text-center mb-16 animate-in fade-in slide-in-from-bottom-8 duration-1000">
          <div className="flex items-center gap-3 mb-5">
            <span className="w-1.5 h-1.5 rounded-full bg-brand-500 animate-pulse" />
            <span className="text-[10px] font-mono font-bold tracking-[0.28em] text-brand-500 uppercase">
              {t("eyebrow")}
            </span>
            <span className="w-1.5 h-1.5 rounded-full bg-brand-500 animate-pulse" />
          </div>
          
          <h2 className="font-bold text-4xl md:text-5xl leading-[1.05] mb-6 text-white">
            {t("titleLine1")} <span className="text-neutral-500">{t("titleLine2")}</span>
          </h2>
          
          <p className="text-neutral-400 text-base max-w-2xl font-light leading-relaxed mb-10">
            {t("description")}
          </p>

          <Link
            href="#apply"
            className="group relative inline-flex items-center justify-center gap-2 px-8 py-4 rounded-full bg-white text-dark-obsidian text-sm font-bold hover:bg-neutral-200 transition-all shadow-[0_0_20px_rgba(255,255,255,0.1)] hover:shadow-[0_0_30px_rgba(255,255,255,0.25)]"
          >
            {t("cta")}
            <ArrowUpRight className="size-4 group-hover:translate-x-0.5 group-hover:-translate-y-0.5 transition-transform" />
          </Link>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 lg:gap-6 animate-in fade-in slide-in-from-bottom-8 duration-1000 delay-150">
          
          <div className="group relative md:col-span-2 overflow-hidden rounded-2xl bg-[#0B0B0E] border border-dark-border p-8 md:p-10 flex flex-col justify-end min-h-75 transition-colors hover:border-brand-500/50">
            <div className="absolute inset-0 bg-linear-to-br from-brand-500/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
            <div className="absolute top-0 right-0 p-8 opacity-10 group-hover:opacity-20 group-hover:scale-110 transition-all duration-700">
              <Users className="w-32 h-32 text-brand-500" />
            </div>
            
            <div className="relative z-10">
              <div className="w-12 h-12 rounded-xl bg-dark-obsidian border border-dark-border flex items-center justify-center mb-6 group-hover:border-brand-500/50 group-hover:text-brand-500 transition-colors">
                <Users className="size-5" />
              </div>
              <h3 className="font-bold text-2xl text-white mb-3 tracking-tight">
                {t("roles.curators.title")}
              </h3>
              <p className="text-neutral-400 font-light leading-relaxed max-w-md">
                {t("roles.curators.desc")}
              </p>
            </div>
          </div>

          <div className="group relative overflow-hidden rounded-2xl bg-[#0B0B0E] border border-dark-border p-8 md:p-10 flex flex-col justify-end min-h-75 transition-colors hover:border-brand-500/50">
            <div className="absolute inset-0 bg-linear-to-t from-brand-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500" />
            
            <div className="relative z-10">
              <div className="w-12 h-12 rounded-xl bg-dark-obsidian border border-dark-border flex items-center justify-center mb-6 group-hover:border-brand-500/50 group-hover:text-brand-500 transition-colors">
                <Palette className="size-5" />
              </div>
              <h3 className="font-bold text-2xl text-white mb-3 tracking-tight">
                {t("roles.creators.title")}
              </h3>
              <p className="text-neutral-400 font-light leading-relaxed">
                {t("roles.creators.desc")}
              </p>
            </div>
          </div>

          <div className="group relative md:col-span-3 overflow-hidden rounded-2xl bg-[#0B0B0E] border border-dark-border p-8 md:p-10 flex flex-col md:flex-row items-start md:items-center justify-between gap-8 transition-colors hover:border-brand-500/50">
            <div className="absolute left-0 top-0 w-1 h-full bg-brand-500 scale-y-0 group-hover:scale-y-100 transition-transform duration-500 origin-bottom" />
            
            <div className="relative z-10 flex-1">
              <div className="w-12 h-12 rounded-xl bg-dark-obsidian border border-dark-border flex items-center justify-center mb-6 group-hover:border-brand-500/50 group-hover:text-brand-500 transition-colors">
                <Cog className="size-5" />
              </div>
              <h3 className="font-bold text-2xl text-white mb-3 tracking-tight">
                {t("roles.operators.title")}
              </h3>
              <p className="text-neutral-400 font-light leading-relaxed max-w-2xl">
                {t("roles.operators.desc")}
              </p>
            </div>

            <div className="relative z-10 shrink-0">
               <span className="text-[10px] font-mono tracking-widest text-neutral-500 uppercase flex items-center gap-2 group-hover:text-brand-500 transition-colors">
                 Apply Now <ArrowUpRight className="size-3" />
               </span>
            </div>
          </div>

        </div>
      </div>
    </section>
  );
}