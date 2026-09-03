import { NextRequest, NextResponse } from 'next/server';
import createMiddleware from 'next-intl/middleware';

const intlMiddleware = createMiddleware({
  locales: ['en', 'ar'],
  defaultLocale: 'en',
  localePrefix: 'always'
});

export default function proxy(request: NextRequest) {
  const token = request.cookies.get('accessToken')?.value;
  const { pathname } = request.nextUrl;

  if (pathname.startsWith('/api/proxy')) {
    const requestHeaders = new Headers(request.headers);
    if (token) {
      requestHeaders.set('Authorization', `Bearer ${token}`);
    }
    return NextResponse.next({
      request: {
        headers: requestHeaders,
      },
    });
  }

  const isAuthRoute = ['/login', '/register', '/forgot-password', '/reset-password'].some(route => pathname.endsWith(route));
  const isProtectedRoute = ['/profile'].some(route => pathname.endsWith(route));

  const locale = pathname.split('/')[1] || 'en';

  if (isProtectedRoute && !token) {
    return NextResponse.redirect(new URL(`/${locale}/login`, request.url));
  }

  if (isAuthRoute && token) {
    return NextResponse.redirect(new URL(`/${locale}`, request.url));
  }

  return intlMiddleware(request);
}

export const config = {
  matcher: ['/((?!_next/static|_next/image|favicon.ico).*)'],
};