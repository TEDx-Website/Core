import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'
import { routing } from '@/i18n/routing'

export function middleware(request: NextRequest) {
  if (!request.cookies.has('NEXT_LOCALE')) {
    const response = NextResponse.next()
    response.cookies.set('NEXT_LOCALE', routing.defaultLocale, {
      path: '/',
      maxAge: 60 * 60 * 24 * 365,
      sameSite: 'lax',
    })
    return response
  }

  return NextResponse.next()
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|assets|favicon.ico|sw.js|site.webmanifest).*)'],
} 