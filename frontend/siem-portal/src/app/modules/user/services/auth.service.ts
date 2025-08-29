import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, startWith, distinctUntilChanged, map } from 'rxjs';
import { environment } from '../../../environment/environment';
import { JwtStorageService } from '../../../shared/services/jwt-storage.service';
import { LoginRequestDto, LoginResponseDto, VerifyEmailDto } from '../models/auth.dtos';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly base = `${environment.apiBaseUrl}${environment.authApi}`;

  // start from LocalStorage state (NOT true by default)
  private _loggedIn = new BehaviorSubject<boolean>(false);
  isLoggedIn$ = this._loggedIn.asObservable().pipe(
    startWith(this.isAuthenticated()),
    distinctUntilChanged()
  );

  constructor(
    private readonly http: HttpClient,
    private readonly jwtStorage: JwtStorageService,
    private readonly zone: NgZone
  ) {
    // initialize from storage on app start
    this._loggedIn.next(this.isAuthenticated());

    // cross-tab sync: when LocalStorage changes in another tab/window
    window.addEventListener('storage', (e) => {
      // guard: only react to our auth keys
      if (!e.key) return;
      if (this.isAuthStorageKey(e.key)) {
        // bring it back into Angular zone so async pipes update
        this.zone.run(() => this._loggedIn.next(this.isAuthenticated()));
      }
    });
  }

  /** Adjust this list to match JwtStorageService keys */
  private isAuthStorageKey(key: string): boolean {
    const keys = [
      this.jwtStorage.accessTokenKey,
      this.jwtStorage.refreshTokenKey,
      this.jwtStorage.expiresAtKey
    ].filter(Boolean) as string[];
    return keys.includes(key);
  }

  /** LocalStorage-driven check */
  isAuthenticated(): boolean {
    const token = this.jwtStorage.getAccessToken();
    return !!token && !this.jwtStorage.isTokenExpired();
  }

  /** Manual sync if ever needed */
  refreshAuthState(): void {
    this._loggedIn.next(this.isAuthenticated());
  }

  setLoggedIn(v: boolean) { this._loggedIn.next(v); } // optional helper

  login(req: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.base}/login`, req);
  }

  /** Convenience: login + store tokens + push state */
  loginAndStore(req: LoginRequestDto): Observable<LoginResponseDto> {
    return this.login(req).pipe(
      tap((resp) => {
        const token = (resp as any).accessToken ?? (resp as any).token;
        if (!token) throw new Error('No access token returned from server.');

        this.jwtStorage.setTokens(
          token,
          (resp as any).refreshToken,
          (resp as any).expiresAtUtc
        );

        // update stream in this tab
        this._loggedIn.next(true);
      })
    );
  }

  verifyEmail(dto: VerifyEmailDto): Observable<void> {
    return this.http.post<void>(`${this.base}/verify-email`, dto);
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.base}/logout`, null).pipe(
      tap(() => {
        this.jwtStorage.clear();
        this._loggedIn.next(false);
      })
    );
  }

hasRealmRole(role: string): boolean {
  const token = this.jwtStorage.getAccessToken();
  if (!token) return false;
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const roles: string[] = payload?.realm_access?.roles ?? [];
    return Array.isArray(roles) && roles.includes(role);
  } catch {
    return false;
  }
}

isAdmin$ = this.isLoggedIn$.pipe(
  // re-check on any login state change
  map(() => this.hasRealmRole('administrator'))
);

}
