import { cookies } from "next/headers";
import { NavbarClient } from "./navbar-client";

export async function Navbar() {
  const cookieStore = await cookies();
  const userCookieString = cookieStore.get("user")?.value;

  let user = null;

  if (userCookieString) {
    try {
      user = JSON.parse(userCookieString);
    } catch (error) {
      console.error("Failed to parse user cookie", error);
    }
  }

  return <NavbarClient user={user} />;
}
