import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../../environment/environment';
import { JwtStorageService } from '../../../shared/services/jwt-storage.service';
import { LoginRequestDto, LoginResponseDto, VerifyEmailDto } from '../models/auth.dtos';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly base = `${environment.apiBaseUrl}${environment.authApi}`;

  constructor(
    private readonly http: HttpClient,
    private readonly jwtStorage: JwtStorageService
  ) {}

  login(req: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.base}/login`, req);
  }

  /** Convenience: login + store tokens in one call */
  loginAndStore(req: LoginRequestDto): Observable<LoginResponseDto> {
    return this.login(req).pipe(
      tap((resp) => {
        // Accept either `accessToken` or `token` from backend
        const token = (resp as any).accessToken ?? (resp as any).token;
        if (!token) throw new Error('No access token returned from server.');

        this.jwtStorage.setTokens(
          token,
          (resp as any).refreshToken,
          (resp as any).expiresAtUtc
        );
      })
    );
  }

  verifyEmail(dto: VerifyEmailDto): Observable<void> {
    return this.http.post<void>(`${this.base}/verify-email`, dto);
  }

  logout(): Observable<void> {
    // Server extracts user id from the Bearer token (RequireBearerToken)
    return this.http.post<void>(`${this.base}/logout`, null).pipe(
      tap(() => this.jwtStorage.clear())
    );
  }

  isAuthenticated(): boolean {
    const token = this.jwtStorage.getAccessToken();
    return !!token && !this.jwtStorage.isTokenExpired();
  }
}
