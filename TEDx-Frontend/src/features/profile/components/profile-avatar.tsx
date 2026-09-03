"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { Loader2, Camera } from "lucide-react";
import { useUploadProfilePicture } from "../api/profile.hooks";

interface ProfileAvatarProps {
  profilePictureUrl?: string;
  firstName: string;
  lastName: string;
}

export function ProfileAvatar({
  profilePictureUrl,
  firstName,
  lastName,
}: ProfileAvatarProps) {
  const t = useTranslations("profile.errors");
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const { mutate: uploadPicture, isPending } = useUploadProfilePicture();

  const initials =
    `${firstName?.[0] || ""}${lastName?.[0] || ""}`.toUpperCase() || "OA";

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setErrorMessage(null);

    uploadPicture(file, {
      onError: () => {
        setErrorMessage(t("uploadFailed"));
      },
    });
  };

  return (
    <div className="relative">
      <div className="relative w-20 h-20 sm:w-24 sm:h-24 rounded-2xl bg-gradient-to-br from-[#181822] to-[#0D0D12] border border-[#262636] flex items-center justify-center text-white font-black text-2xl sm:text-3xl shadow-xl overflow-hidden group">
        {profilePictureUrl ? (
          <img
            src={profilePictureUrl}
            alt="Profile"
            className="w-full h-full object-cover"
          />
        ) : (
          <span>{initials}</span>
        )}

        {isPending && (
          <div className="absolute inset-0 bg-black/85 backdrop-blur-sm flex flex-col items-center justify-center gap-1 text-brand-400">
            <Loader2 className="size-5 animate-spin text-white" />
          </div>
        )}
      </div>

      <label
        htmlFor="avatarUploadInput"
        className="absolute -bottom-1 -right-1 p-2 rounded-xl bg-neutral-900 border border-neutral-800 text-neutral-300 hover:text-white hover:border-brand-500/60 cursor-pointer shadow-lg transition-all"
      >
        <Camera className="size-3.5" />
        <input
          id="avatarUploadInput"
          type="file"
          accept="image/*"
          className="sr-only"
          onChange={handleFileChange}
          disabled={isPending}
        />
      </label>

      {errorMessage && (
        <div className="absolute top-full mt-2 left-0 z-25 p-2 rounded bg-red-950/80 border border-red-500 text-[10px] text-red-200 whitespace-nowrap">
          {errorMessage}
        </div>
      )}
    </div>
  );
}
