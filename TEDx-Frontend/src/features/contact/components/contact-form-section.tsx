"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Image from "next/image";
import { Key, ArrowRight, CheckCircle2 } from "lucide-react";
import { Input } from "@/shared/ui/input";
import { Button } from "@/shared/ui/button";
import { Spinner } from "@/shared/ui/spinner";
import { useSubmitContact } from "../api/contact.hooks";
import { ContactPayload } from "../types/contact.types";
import {
  getContactSchema,
  type ContactFormValues,
} from "../schema/contact.schema";

export function ContactFormSection() {
  const t = useTranslations("contact");
  const [isSuccess, setIsSuccess] = useState(false);

  const { mutate, isPending } = useSubmitContact();
  const schema = getContactSchema(t);

  const {
    register,
    handleSubmit,
    watch,
    reset,
    setError,
    formState: { errors },
  } = useForm<ContactFormValues>({
    resolver: zodResolver(schema),
  });

  const messageValue = watch("message") || "";

  const onSubmit = (data: ContactFormValues) => {
    const payload: ContactPayload = {
      name: data.name,
      email: data.email,
      subject: data.subject,
      message: data.message,
    };

    mutate(payload, {
      onSuccess: (response) => {
        if (!response?.success) {
          setError("root", {
            message: response?.error?.message || "Failed to submit",
          });
          return;
        }
        setIsSuccess(true);
        reset();
        setTimeout(() => setIsSuccess(false), 5000);
      },
      onError: (error) => {
        const serverMessage =
          error?.response?.data?.error?.message ||
          error?.message ||
          "Failed to connect to server";

        setError("root", {
          message: serverMessage,
        });
      },
    });
  };

  return (
    <section className="relative py-16 sm:py-40 px-6 sm:px-12 bg-dark-obsidian w-full border-b border-dark-border overflow-hidden">
      <div className="absolute top-1/3 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[60vw] h-[60vw] max-w-175 max-h-175 bg-brand-500/10 rounded-full blur-[170px] pointer-events-none" />

      <div className="max-w-7xl mx-auto relative z-10">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-10 items-stretch">
          <div className="lg:col-span-7 flex flex-col">
            <div className="bg-dark-glass border border-dark-border rounded-3xl p-8 sm:p-10 lg:p-12 relative overflow-hidden shadow-2xl flex-1 flex flex-col justify-between">
              <div className="absolute top-0 inset-x-0 h-1 bg-linear-to-r from-transparent via-brand-500 to-transparent" />

              <div>
                <div className="mb-8 text-left">
                  <div className="flex items-center gap-3 mb-2">
                    <div className="h-px w-6 bg-brand-500" />
                    <span className="text-brand-500 font-mono text-xs font-bold tracking-[0.25em] uppercase">
                      {t("hero.eyebrow")}
                    </span>
                  </div>
                  <h1 className="font-heading text-3xl sm:text-4xl font-extrabold uppercase tracking-tight text-white mb-2">
                    {t("hero.title")}
                  </h1>
                  <p className="text-xs sm:text-sm text-neutral-400 font-light max-w-xl">
                    {t("hero.subtitle")}
                  </p>
                </div>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                  {errors.root && (
                    <div className="p-3 rounded-xl bg-red-950/40 border border-red-500/40 text-red-300 text-xs">
                      {errors.root.message}
                    </div>
                  )}

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
                    <div>
                      <label className="block text-xs font-mono font-bold tracking-wider uppercase text-neutral-300 mb-2">
                        {t("form.fullName")}{" "}
                        <span className="text-brand-500">*</span>
                      </label>
                      <Input
                        {...register("name")}
                        placeholder={t("form.fullNamePh")}
                        className={
                          errors.name
                            ? "border-red-500 focus-visible:ring-red-500/20"
                            : ""
                        }
                      />
                      {errors.name && (
                        <span className="text-[10px] text-red-400 mt-1">
                          {errors.name.message}
                        </span>
                      )}
                    </div>

                    <div>
                      <label className="block text-xs font-mono font-bold tracking-wider uppercase text-neutral-300 mb-2">
                        {t("form.email")}{" "}
                        <span className="text-brand-500">*</span>
                      </label>
                      <Input
                        {...register("email")}
                        type="email"
                        placeholder={t("form.emailPh")}
                        className={
                          errors.email
                            ? "border-red-500 focus-visible:ring-red-500/20"
                            : ""
                        }
                      />
                      {errors.email && (
                        <span className="text-[10px] text-red-400 mt-1">
                          {errors.email.message}
                        </span>
                      )}
                    </div>
                  </div>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
                    <div>
                      <label className="block text-xs font-mono font-bold tracking-wider uppercase text-neutral-300 mb-2">
                        {t("form.department")}
                      </label>
                      <select
                        {...register("department")}
                        className="flex h-12 w-full rounded-xl border border-neutral-200 dark:border-dark-border bg-transparent sm:bg-neutral-100 dark:bg-dark-obsidian px-4 py-3 text-sm text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500/20 focus-visible:border-brand-500 transition-all cursor-pointer"
                      >
                        <option
                          value="General"
                          className="bg-dark-carbon text-white"
                        >
                          {t("form.deptOptions.general")}
                        </option>
                        <option
                          value="Curation"
                          className="bg-dark-carbon text-white"
                        >
                          {t("form.deptOptions.curation")}
                        </option>
                        <option
                          value="Partnership"
                          className="bg-dark-carbon text-white"
                        >
                          {t("form.deptOptions.partnership")}
                        </option>
                        <option
                          value="Media"
                          className="bg-dark-carbon text-white"
                        >
                          {t("form.deptOptions.media")}
                        </option>
                      </select>
                    </div>

                    <div>
                      <label className="block text-xs font-mono font-bold tracking-wider uppercase text-neutral-300 mb-2">
                        {t("form.phone")}{" "}
                        <span className="text-neutral-500 text-[10px] font-normal">
                          {t("form.optional")}
                        </span>
                      </label>
                      <Input
                        {...register("phone")}
                        type="tel"
                        placeholder={t("form.phonePh")}
                      />
                    </div>
                  </div>

                  <div>
                    <label className="block text-xs font-mono font-bold tracking-wider uppercase text-neutral-300 mb-2">
                      {t("form.subject")}{" "}
                      <span className="text-brand-500">*</span>
                    </label>
                    <Input
                      {...register("subject")}
                      placeholder={t("form.subjectPh")}
                      className={
                        errors.subject
                          ? "border-red-500 focus-visible:ring-red-500/20"
                          : ""
                      }
                    />
                    {errors.subject && (
                      <span className="text-[10px] text-red-400 mt-1">
                        {errors.subject.message}
                      </span>
                    )}
                  </div>

                  <div>
                    <label className="block text-xs font-mono font-bold tracking-wider uppercase text-neutral-300 mb-2">
                      {t("form.message")}{" "}
                      <span className="text-brand-500">*</span>
                    </label>
                    <textarea
                      {...register("message")}
                      rows={4}
                      placeholder={t("form.messagePh")}
                      className={`flex w-full rounded-xl border bg-transparent dark:bg-dark-obsidian px-4 py-3 text-sm text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-brand-500/20 focus-visible:border-brand-500 resize-none transition-all ${
                        errors.message
                          ? "border-red-500"
                          : "dark:border-dark-border"
                      }`}
                    />
                    <div className="flex justify-between items-center text-[10px] text-neutral-500 mt-1.5 font-mono">
                      <span>{t("form.maxChars")}</span>
                      <span>{messageValue.length} / 2000</span>
                    </div>
                    {errors.message && (
                      <span className="text-[10px] text-red-400 mt-1">
                        {errors.message.message}
                      </span>
                    )}
                  </div>

                  <div className="pt-2">
                    {isSuccess ? (
                      <div className="p-4 rounded-xl text-xs font-medium text-center bg-emerald-950/60 border border-emerald-500/40 text-emerald-400 flex items-center justify-center gap-2">
                        <CheckCircle2 className="size-4" />
                        {t("form.success")}
                      </div>
                    ) : (
                      <Button
                        type="submit"
                        disabled={isPending}
                        className="w-full bg-brand-500 hover:bg-brand-600 text-white uppercase tracking-wider h-14 rounded-full shadow-[0_0_25px_rgba(235,0,40,0.35)] hover:-translate-y-0.5"
                      >
                        {isPending ? (
                          <>
                            <Spinner className="mr-2 text-white" />
                            <span>{t("form.submitting")}</span>
                          </>
                        ) : (
                          <>
                            <span>{t("form.submit")}</span>
                            <ArrowRight className="ml-2 size-4" />
                          </>
                        )}
                      </Button>
                    )}
                  </div>
                </form>
              </div>
            </div>
          </div>

          <div className="lg:col-span-5 flex flex-col">
            <div className="relative w-full h-full min-h-125 lg:min-h-full rounded-3xl overflow-hidden border border-dark-border bg-dark-glass shadow-2xl group flex flex-col justify-between p-6 sm:p-8">
              <Image
                src="/assets/contact.webp"
                alt="Contact"
                fill
                sizes="(max-width: 1024px) 100vw, 40vw" 
                className="object-cover object-center group-hover:scale-105 transition-transform duration-700 ease-out"
              />
              <div className="absolute inset-0 bg-linear-to-t from-dark-obsidian via-dark-obsidian/40 to-transparent pointer-events-none" />
              <div className="absolute inset-0 bg-linear-to-b from-dark-obsidian/60 via-transparent to-transparent pointer-events-none" />
              <div className="absolute inset-0 border border-brand-500/20 rounded-3xl pointer-events-none group-hover:border-brand-500/40 transition-colors duration-500" />

              <div className="relative z-10 flex items-center justify-end w-full">
                <div className="w-8 h-8 rounded-full bg-dark-obsidian/80 backdrop-blur-md border border-dark-border flex items-center justify-center text-brand-500 text-xs shadow-lg">
                  <Key className="size-3.5" />
                </div>
              </div>

              <div className="relative z-10 space-y-2 pt-24 lg:pt-auto mt-auto">
                <div className="flex items-center gap-2">
                  <div className="h-px w-4 bg-brand-500" />
                  <span className="text-brand-500 font-mono text-[11px] font-bold tracking-[0.25em] uppercase">
                    {t("hero.thresholdProtocol")}
                  </span>
                </div>
                <h3 className="font-heading text-2xl sm:text-3xl font-black uppercase tracking-tight text-white">
                  {t("hero.unlimitedDoors")}
                </h3>
                <p className="text-xs sm:text-sm text-neutral-300 font-light leading-relaxed">
                  {t("hero.imageDesc")}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
