"use server";

import { cookies } from "next/headers";
import { AuthTokens } from "../types/auth.types";

export async function setAuthCookies(data: AuthTokens) {
  const cookieStore = await cookies();
  const isProduction = process.env.NODE_ENV === "production";

  cookieStore.set("accessToken", data.accessToken, {
    secure: isProduction,
    path: "/",
    maxAge: data.accessTokenExpiresIn,
  });

  cookieStore.set("refreshToken", data.refreshToken, {
    secure: isProduction,
    path: "/",
    maxAge: data.refreshTokenExpiresIn,
  });

  cookieStore.set("user", JSON.stringify(data.user), {
    secure: isProduction,
    path: "/",
    maxAge: data.refreshTokenExpiresIn,
  });
}

export async function clearAuthCookies() {
  const cookieStore = await cookies();
  cookieStore.delete("accessToken");
  cookieStore.delete("refreshToken");
  cookieStore.delete("user");
}