import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, catchError } from 'rxjs';
import { throwError } from 'rxjs';

export interface AuthResponse {
  token: string;
  username: string;
  userId: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/auth';
  private tokenKey = 'excuse_token';
  private userKey = 'excuse_user';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  private userSubject = new BehaviorSubject<{ username: string; userId: string } | null>(
    this.getStoredUser()
  );
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, {
      email,
      password
    } as LoginRequest).pipe(
      tap(response => this.handleAuthResponse(response)),
      catchError(error => {
        const errorMessage = error.error?.error || 'Login failed. Please try again.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  register(username: string, email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, {
      username,
      email,
      password
    } as RegisterRequest).pipe(
      tap(response => this.handleAuthResponse(response)),
      catchError(error => {
        const errorMessage = error.error?.error || 'Registration failed. Please try again.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  private handleAuthResponse(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    localStorage.setItem(this.userKey, JSON.stringify({
      username: response.username,
      userId: response.userId
    }));
    this.isAuthenticatedSubject.next(true);
    this.userSubject.next({
      username: response.username,
      userId: response.userId
    });
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  private getStoredUser() {
    const user = localStorage.getItem(this.userKey);
    return user ? JSON.parse(user) : null;
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticatedSubject.next(false);
    this.userSubject.next(null);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  getUser() {
    return this.getStoredUser();
  }
}
