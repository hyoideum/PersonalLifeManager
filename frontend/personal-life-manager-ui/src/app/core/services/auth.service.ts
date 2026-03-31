import { Inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, throwError } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { jwtDecode } from 'jwt-decode';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

export interface RefreshResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = environment.apiUrl + "/auth";
  private readonly TOKEN_KEY = 'token';
  private readonly REFRESH_KEY = 'refreshToken';
  currentUser = signal<string | null>(null);
  isBrowser = false;
  private logoutTimer: any;

  constructor(private http: HttpClient, @Inject(PLATFORM_ID) platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId)

    if (this.isBrowser) {
      const token = localStorage.getItem(this.TOKEN_KEY);

      if (token) {
        this.currentUser.set(token);
        this.startAutoRefresh(token);
      }
    }
  }

  get token(): string | null {
    return this.currentUser();
    if (!this.isBrowser) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  register(data: LoginRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/register`, data);
  }

  login(data: LoginRequest): Observable<RefreshResponse> {
    return this.http.post<RefreshResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(response => {
        if (response?.accessToken) {
          this.setToken(response.accessToken);
          this.currentUser.set(response.accessToken);
          localStorage.setItem(this.REFRESH_KEY, response.refreshToken);
          this.startAutoRefresh(response.accessToken);
          // this.router.navigate(['/dashboard']);
        }
      })
    );
  }

  private setToken(token: string): void {
    if (!this.isBrowser) return;
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  logout() {
    if (this.isBrowser) {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.REFRESH_KEY);
    }
    // this.router.navigate(['auth/login']);
  }

  isLoggedIn(): boolean {
    if (!this.isBrowser) return false;

    const token = localStorage.getItem(this.TOKEN_KEY);

    return !!token;
  }

  isTokenExpired(token: string): boolean {
    try {
      const decoded: any = jwtDecode(token);

      if (!decoded.exp) return true;

      const expiry = decoded.exp * 1000;
      return Date.now() > expiry;

    } catch {
      return true;
    }
  }

  startAutoRefresh(token: string) {
    const decoded: any = jwtDecode(token);

    if (!decoded.exp) return;

    const expiryTime = decoded.exp * 1000;
    const now = Date.now();

    const refreshTime = expiryTime - now - 30000;


    if (refreshTime <= 0) {
      this.refreshToken().subscribe();
      return;
    }

    if (this.logoutTimer) clearTimeout(this.logoutTimer);

    this.logoutTimer = setTimeout(() => {
      this.refreshToken().subscribe({
        error: () => this.logout()
      });
    }, refreshTime);
  }

  refreshToken(): Observable<RefreshResponse> {
    const refreshToken = localStorage.getItem(this.REFRESH_KEY);
    if (!refreshToken) return throwError(() => new Error('No refresh token'));

    return this.http.post<RefreshResponse>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap(res => {
        this.setToken(res.accessToken);
        if (res.refreshToken) localStorage.setItem(this.REFRESH_KEY, res.refreshToken);
        this.currentUser.set(res.accessToken);
        this.startAutoRefresh(res.accessToken);
      })
    );
  }
}