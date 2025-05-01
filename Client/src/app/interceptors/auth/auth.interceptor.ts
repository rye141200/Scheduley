import { HttpClient, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { CookieService } from 'ngx-cookie-service';
import { environment } from '../../../environments/environment';
import { catchError, switchMap, Observable, of, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const cookieService = inject(CookieService);
  const httpClient = inject(HttpClient);

  // Add common headers to all requests
  req = req.clone({
    setHeaders: {
      'X-Client': 'AngularApp',
    },
    withCredentials: true,
  });

  // Skip token validation for refresh-token endpoint to avoid infinite loop
  if (req.url.includes('refresh-token')) return next(req);

  const token = cookieService.get('ScheduleyAccessToken');

  // If no token exists, proceed with request (might be a public endpoint)
  if (!token) return next(req);

  try {
    const decoded: { exp: number } = jwtDecode(token);

    // If token is still valid, proceed with request
    if (decoded.exp >= Date.now() / 1000) return next(req);

    // Token is expired, attempt to refresh
    return httpClient
      .get(`${environment.apiURL}api/account/refresh-token`)
      .pipe(
        switchMap(() => {
          // After successful refresh, proceed with original request
          return next(req);
        }),
        catchError((error) => {
          console.error('Token refresh failed:', error);
          // Handle refresh failure (could redirect to login)
          // For now, just propagate the error
          return throwError(() => error);
        })
      );
  } catch (error) {
    // Error decoding token - likely invalid format
    console.error('Error decoding token:', error);
    return next(req); // Proceed anyway and let server reject if needed
  }
};
