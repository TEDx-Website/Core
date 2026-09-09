"use client";

import { useTranslations } from "next-intl";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/shared/ui/accordion";

export function FAQSection() {
  const t = useTranslations("contact.faq");

  const faqs = [
    { value: "item-1", question: t("q1"), answer: t("a1") },
    { value: "item-2", question: t("q2"), answer: t("a2") },
    { value: "item-3", question: t("q3"), answer: t("a3") },
  ];

  return (
    <section className="py-24 px-6 sm:px-12 bg-dark-obsidian w-full border-b border-dark-border">
      <div className="max-w-4xl mx-auto">
        <div className="text-center mb-16 animate-in fade-in slide-in-from-bottom-8 duration-1000">
          <div className="flex items-center justify-center gap-3 mb-4">
            <div className="h-px w-6 bg-brand-500" />
            <span className="text-brand-500 font-mono text-xs font-bold tracking-[0.25em] uppercase">
              {t("eyebrow")}
            </span>
            <div className="h-px w-6 bg-brand-500" />
          </div>
          <h2 className="font-heading text-3xl sm:text-4xl font-extrabold uppercase tracking-tight text-white mb-3">
            {t("title")}
          </h2>
          <p className="text-neutral-400 text-sm font-light">{t("subtitle")}</p>
        </div>

        <Accordion type="single" collapsible className="space-y-4">
          {faqs.map((faq) => (
            <AccordionItem
              key={faq.value}
              value={faq.value}
              className="bg-dark-glass border border-dark-border rounded-2xl px-6 data-[state=open]:border-brand-500/40 data-[state=open]:shadow-[0_10px_30px_-10px_rgba(235,0,40,0.2)] transition-all duration-300"
            >
              <AccordionTrigger className="text-base font-bold text-white hover:no-underline py-6">
                {faq.question}
              </AccordionTrigger>
              <AccordionContent className="text-sm text-neutral-400 leading-relaxed font-light pb-6 pt-0">
                {faq.answer}
              </AccordionContent>
            </AccordionItem>
          ))}
        </Accordion>
      </div>
    </section>
  );
}
