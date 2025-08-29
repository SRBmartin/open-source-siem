import { Injectable } from '@angular/core';

const ACCESS_TOKEN_KEY = 'siem.access_token';
const REFRESH_TOKEN_KEY = 'siem.refresh_token';
const EXPIRES_AT_KEY   = 'siem.expires_at_utc';

@Injectable({ providedIn: 'root' })
export class JwtStorageService {
  readonly accessTokenKey = 'siem:access_token';
  readonly refreshTokenKey = 'siem:refresh_token';
  readonly expiresAtKey    = 'siem:expires_at';
  
  setTokens(accessToken: string, refreshToken?: string, expiresAtUtc?: string) {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    if (refreshToken) localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);

    // If server didn't send an ISO `expiresAtUtc`, try decode exp from JWT
    const explicit = expiresAtUtc ?? this.decodeExpToIso(accessToken);
    if (explicit) localStorage.setItem(EXPIRES_AT_KEY, explicit);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  getExpiresAtUtc(): string | null {
    return localStorage.getItem(EXPIRES_AT_KEY);
  }

  /** 30s skew, like you prefer */
  isTokenExpired(skewSeconds = 30): boolean {
    const iso = this.getExpiresAtUtc();
    if (!iso) return false; // if unknown, don't block sending – server will 401 if needed
    const expiry = new Date(iso).getTime();
    const now = Date.now() + skewSeconds * 1000;
    return now >= expiry;
  }

  clear() {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(EXPIRES_AT_KEY);
  }

  // --- helpers ---

  private decodeExpToIso(jwt?: string): string | null {
    try {
      if (!jwt) return null;
      const payload = JSON.parse(atob(jwt.split('.')[1]));
      const expSec: number | undefined = payload?.exp; // seconds since epoch
      if (!expSec) return null;
      return new Date(expSec * 1000).toISOString();
    } catch {
      return null;
    }
  }
}