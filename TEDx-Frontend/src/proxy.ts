import { NextRequest, NextResponse } from 'next/server';
import createMiddleware from 'next-intl/middleware';

const intlMiddleware = createMiddleware({
  locales: ['en', 'ar'],
  defaultLocale: 'en',
  localePrefix: 'always'
});

export default function middleware(request: NextRequest) {
  const token = request.cookies.get('accessToken')?.value;
  const { pathname } = request.nextUrl;

  const isAuthRoute = ['/login', '/register', '/forgot-password', '/reset-password'].some(route => pathname.endsWith(route));
  const isProtectedRoute = ['/profile'].some(route => pathname.endsWith(route));

  const locale = pathname.split('/')[1] || 'en';

  if (isProtectedRoute && !token) {
    return NextResponse.redirect(new URL(`/${locale}/login`, request.url));
  }

  if (isAuthRoute && token) {
    return NextResponse.redirect(new URL(`/${locale}`, request.url));
  }

  const response = intlMiddleware(request);

  if (token) {
    response.headers.set('Authorization', `Bearer ${token}`);
  }

  return response;
}

export const config = {
  matcher: ['/((?!api/proxy|_next/static|_next/image|favicon.ico).*)'],
};