import { getRequestConfig } from 'next-intl/server';
import { routing, Locale } from './routing';
import { cookies } from 'next/headers';

export default getRequestConfig(async ({ requestLocale }) => {
  let locale = await requestLocale;

  if (!locale || !routing.locales.includes(locale as Locale)) {
    const cookieStore = await cookies();
    locale = cookieStore.get('NEXT_LOCALE')?.value || routing.defaultLocale;
  }

  const landing = (await import(`../../messages/${locale}/landing.json`)).default;
  const about = (await import(`../../messages/${locale}/about.json`)).default;

  return {
    locale,
    messages: {
        landing,
        about,
    },
  };
});