import { getTranslations } from "next-intl/server";
import { profileService } from "@/features/profile/api/profile.service";
import { ProfileAvatar } from "@/features/profile/components/profile-avatar";
import { ProfileForm } from "@/features/profile/components/profile-form";
import { ChangePasswordForm } from "@/features/profile/components/change-password-form";
import { User, Shield } from "lucide-react";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "profile.layout" });
  return {
    title: `${t("title")} | TEDx Alkawmia`,
  };
}

export default async function ProfilePage() {
  const t = await getTranslations("profile");

  let userData = null;
  try {
    const res = await profileService.getProfile();
    if (res.success) {
      userData = res.data;
    }
  } catch {
    userData = null;
  }

  if (!userData) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-20 text-center">
        <p className="text-sm text-red-400">{t("errors.fetchFailed")}</p>
      </div>
    );
  }

  return (
    <main className="flex-1 max-w-4xl w-full mx-auto px-4 sm:px-6 py-10 sm:py-14">
      <section className="pb-10 border-b border-[#181820]">
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-6">
          <div className="flex items-center gap-6">
            <ProfileAvatar
              profilePictureUrl={userData.profilePictureUrl}
              firstName={userData.firstName}
              lastName={userData.lastName}
            />

            <div className="space-y-1.5">
              <div className="flex items-center gap-3 flex-wrap">
                <h1 className="font-extrabold text-2xl sm:text-3xl text-white tracking-tight">
                  {userData.firstName} {userData.lastName}
                </h1>
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-[11px] font-mono bg-[#181822] border border-[#2A2A38] text-white">
                  {userData.globalRole}
                </span>
              </div>
              <p className="text-xs sm:text-sm text-[#71717A] font-mono flex items-center gap-1.5">
                <span>{userData.email}</span>
                <span className="inline-block w-1 h-1 rounded-full bg-emerald-400" />
              </p>
            </div>
          </div>
        </div>
      </section>

      <div className="mt-8">
        <div className="flex items-center gap-8 border-b border-[#181820] mb-10">
          <label className="flex items-center gap-2 pb-3 font-semibold text-sm border-b-2 border-brand-500 text-white cursor-pointer">
            <User className="size-4" />
            <span>{t("tabs.profileDetails")}</span>
          </label>
        </div>

        <div className="space-y-12">
          <ProfileForm user={userData} />

          <div className="pt-10 border-t border-[#181820]">
            <div className="flex items-center gap-2 mb-6">
              <Shield className="size-5 text-brand-500" />
              <h2 className="font-bold text-lg text-white">
                {t("tabs.security")}
              </h2>
            </div>
            <ChangePasswordForm />
          </div>
        </div>
      </div>
    </main>
  );
}
