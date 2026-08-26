import {
  Inter,
  Alexandria,
  Noto_Sans_Arabic,
  Great_Vibes,
} from "next/font/google";
import { NextIntlClientProvider } from "next-intl";
import { getMessages } from "next-intl/server";
import { ThemeProvider } from "@/components/theme-provider";

import "../globals.css";
import { Footer } from "@/shared/layout/footer";
import { Navbar } from "@/shared/layout/navbar";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });
const alexandria = Alexandria({
  subsets: ["arabic"],
  variable: "--font-alexandria",
});
const notoSansArabic = Noto_Sans_Arabic({
  subsets: ["arabic"],
  variable: "--font-noto",
});

const greatVibes = Great_Vibes({
  weight: "400",
  subsets: ["latin"],
  variable: "--font-script",
});

export default async function LocaleLayout({
  children,
  params,
}: {
  children: React.ReactNode;
  params: Promise<{ locale: string }>;
}) {
  const resolvedParams = await params;
  const messages = await getMessages();

  return (
    <html
      lang={resolvedParams.locale}
      dir={resolvedParams.locale === "ar" ? "rtl" : "ltr"}
      suppressHydrationWarning
    >
      <body
        className={`${inter.variable} ${alexandria.variable} ${notoSansArabic.variable} ${greatVibes.variable} font-sans antialiased`}
      >
        <NextIntlClientProvider messages={messages}>
          <ThemeProvider attribute="class" defaultTheme="light" enableSystem>
            <Navbar />
            {children}
            <Footer />
          </ThemeProvider>
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
