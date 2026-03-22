import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.authService.getToken();

    console.log('JwtInterceptor - URL:', request.url);
    console.log('JwtInterceptor - Token exists:', !!token);
    
    if (token) {
      console.log('JwtInterceptor - Adding Authorization header');
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    } else {
      console.warn('JwtInterceptor - No token found!');
    }

    return next.handle(request);
  }
}
