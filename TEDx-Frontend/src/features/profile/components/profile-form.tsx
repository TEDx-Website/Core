"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { Loader2, Check, Edit2 } from "lucide-react";
import {
  getUpdateProfileSchema,
  UpdateProfileInput,
} from "../schema/profile.schema";
import { UserProfile } from "../types/profile.types";
import { useUpdateProfile } from "../api/profile.hooks";

interface ProfileFormProps {
  user: UserProfile;
}

export function ProfileForm({ user }: ProfileFormProps) {
  const t = useTranslations("profile");
  const [isEditing, setIsEditing] = useState(false);
  const [successMsg, setSuccessMsg] = useState(false);

  const { mutate: updateProfile, isPending } = useUpdateProfile();

  const {
    register,
    handleSubmit,
    watch,
    setError,
    formState: { errors },
  } = useForm<UpdateProfileInput>({
    resolver: zodResolver(getUpdateProfileSchema(t)),
    defaultValues: {
      firstName: user.firstName || "",
      lastName: user.lastName || "",
      phone: user.phone || "",
      bio: user.bio || "",
    },
  });

  const firstNameValue = watch("firstName") || "";
  const lastNameValue = watch("lastName") || "";
  const phoneValue = watch("phone") || "";
  const bioValue = watch("bio") || "";

  const onSubmit = (data: UpdateProfileInput) => {
    updateProfile(data, {
      onSuccess: (res) => {
        if (!res.success) {
          setError("root", {
            message: res.error?.message || "Failed to update",
          });
          return;
        }
        setIsEditing(false);
        setSuccessMsg(true);
        setTimeout(() => setSuccessMsg(false), 3000);
      },
      onError: (err) => {
        setError("root", { message: err.message || "Server connection error" });
      },
    });
  };

  return (
    <div className="space-y-6">
      {successMsg && (
        <div className="p-3 rounded-xl bg-emerald-950/40 border border-emerald-500/40 text-emerald-300 text-xs flex items-center gap-2">
          <Check className="size-4" />
          <span>{t("details.updateSuccess")}</span>
        </div>
      )}

      {errors.root && (
        <div className="p-3 rounded-xl bg-red-950/40 border border-red-500/40 text-red-300 text-xs">
          {errors.root.message}
        </div>
      )}

      {!isEditing ? (
        <div className="space-y-6">
          <div className="flex justify-end">
            <button
              type="button"
              onClick={() => setIsEditing(true)}
              className="inline-flex items-center gap-2 bg-[#15151B] border border-[#22222D] text-white font-medium text-xs px-4 py-2.5 rounded-xl hover:bg-[#1C1C24] transition-all"
            >
              <Edit2 className="size-3.5 text-neutral-400" />
              <span>{t("header.editProfile")}</span>
            </button>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="p-5 rounded-2xl bg-[#111115] border border-[#1E1E26] space-y-1">
              <span className="text-[11px] font-mono uppercase tracking-widest text-[#71717A] block">
                {t("details.fullName")}
              </span>
              <p className="text-sm font-semibold text-white">
                {user.firstName} {user.lastName}
              </p>
            </div>

            <div className="p-5 rounded-2xl bg-[#111115] border border-[#1E1E26] space-y-1">
              <span className="text-[11px] font-mono uppercase tracking-widest text-[#71717A] block">
                {t("details.emailAddress")}
              </span>
              <p className="text-sm font-semibold text-white font-mono">
                {user.email}
              </p>
            </div>

            <div className="p-5 rounded-2xl bg-[#111115] border border-[#1E1E26] space-y-1">
              <span className="text-[11px] font-mono uppercase tracking-widest text-[#71717A] block">
                {t("details.phoneNumber")}
              </span>
              <p className="text-sm font-semibold text-white font-mono">
                {user.phone || t("details.notProvided")}
              </p>
            </div>

            <div className="p-5 rounded-2xl bg-[#111115] border border-[#1E1E26] space-y-1">
              <span className="text-[11px] font-mono uppercase tracking-widest text-[#71717A] block">
                {t("details.accountRole")}
              </span>
              <p className="text-sm font-semibold text-brand-500">
                {user.globalRole}
              </p>
            </div>
          </div>

          <div className="p-5 rounded-2xl bg-[#111115] border border-[#1E1E26] space-y-2">
            <span className="text-[11px] font-mono uppercase tracking-widest text-[#71717A] block">
              {t("details.biography")}
            </span>
            <p className="text-xs sm:text-sm text-[#D4D4D8] leading-relaxed">
              {user.bio || t("details.noBio")}
            </p>
          </div>
        </div>
      ) : (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="p-4 rounded-xl bg-[#16161D] border border-amber-500/30 text-xs flex items-start gap-3">
            <p className="leading-relaxed text-[#D4D4D8]">
              <strong className="text-amber-300 font-semibold">
                {t("details.replaceWarningTitle")}
              </strong>{" "}
              {t("details.replaceWarningText")}
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <div>
              <label
                className="block text-xs font-medium text-foreground mb-1.5"
                htmlFor="firstName"
              >
                {t("details.firstNameLabel")}
              </label>
              <input
                {...register("firstName")}
                id="firstName"
                maxLength={100}
                className="w-full bg-[#111115] border border-[#22222D] rounded-xl text-white px-4 py-3 text-sm outline-none focus:border-brand-500"
              />
              <span className="text-[10px] text-neutral-500 font-mono mt-1 block">
                {firstNameValue.length}/100
              </span>
              {errors.firstName && (
                <span className="text-[11px] text-red-400 mt-1 block">
                  {errors.firstName.message}
                </span>
              )}
            </div>

            <div>
              <label
                className="block text-xs font-medium text-foreground mb-1.5"
                htmlFor="lastName"
              >
                {t("details.lastNameLabel")}
              </label>
              <input
                {...register("lastName")}
                id="lastName"
                maxLength={100}
                className="w-full bg-[#111115] border border-[#22222D] rounded-xl text-white px-4 py-3 text-sm outline-none focus:border-brand-500"
              />
              <span className="text-[10px] text-neutral-500 font-mono mt-1 block">
                {lastNameValue.length}/100
              </span>
              {errors.lastName && (
                <span className="text-[11px] text-red-400 mt-1 block">
                  {errors.lastName.message}
                </span>
              )}
            </div>

            <div>
              <label
                className="block text-xs font-medium text-foreground mb-1.5"
                htmlFor="phone"
              >
                {t("details.phoneNumber")} {t("details.optional")}
              </label>
              <input
                {...register("phone")}
                id="phone"
                maxLength={32}
                className="w-full bg-[#111115] border border-[#22222D] rounded-xl text-white px-4 py-3 text-sm outline-none focus:border-brand-500 font-mono"
              />
              <span className="text-[10px] text-neutral-500 font-mono mt-1 block">
                {phoneValue.length}/32
              </span>
              {errors.phone && (
                <span className="text-[11px] text-red-400 mt-1 block">
                  {errors.phone.message}
                </span>
              )}
            </div>
          </div>

          <div>
            <label
              className="block text-xs font-medium text-foreground mb-1.5"
              htmlFor="bio"
            >
              {t("details.biography")} {t("details.optional")}
            </label>
            <textarea
              {...register("bio")}
              id="bio"
              rows={4}
              maxLength={1000}
              className="w-full bg-[#111115] border border-[#22222D] rounded-xl text-white px-4 py-3 text-sm outline-none focus:border-brand-500"
            />
            <span className="text-[10px] text-neutral-500 font-mono mt-1 block">
              {bioValue.length}/1000
            </span>
            {errors.bio && (
              <span className="text-[11px] text-red-400 mt-1 block">
                {errors.bio.message}
              </span>
            )}
          </div>

          <div className="flex items-center justify-end gap-3 pt-4 border-t border-[#181820]">
            <button
              type="button"
              onClick={() => setIsEditing(false)}
              className="bg-[#15151B] border border-[#22222D] text-white font-medium text-xs px-4 py-2.5 rounded-xl hover:bg-[#1C1C24]"
            >
              {t("details.cancel")}
            </button>
            <button
              type="submit"
              disabled={isPending}
              className="bg-brand-500 hover:bg-brand-600 text-white font-semibold text-xs px-5 py-2.5 rounded-xl flex items-center gap-2 shadow-md disabled:opacity-50"
            >
              {isPending ? (
                <>
                  <Loader2 className="size-4 animate-spin" />
                  <span>{t("details.saving")}</span>
                </>
              ) : (
                <span>{t("details.saveChanges")}</span>
              )}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
