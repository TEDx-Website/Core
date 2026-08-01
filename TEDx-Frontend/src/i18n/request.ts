import { getRequestConfig } from 'next-intl/server';
import { routing, Locale } from './routing';
import { cookies } from 'next/headers';

export default getRequestConfig(async ({ requestLocale }) => {
  let locale = await requestLocale;

  if (!locale || !routing.locales.includes(locale as Locale)) {
    const cookieStore = await cookies();
    locale = cookieStore.get('NEXT_LOCALE')?.value || routing.defaultLocale;
  }

  const common = (await import(`../../messages/${locale}/common.json`)).default;

  return {
    locale,
    messages: {
      common,
    },
  };
});