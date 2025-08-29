export interface LoginRequestDto {
  /** Backend expects Username, you map email -> username in the component */
  username: string;
  password: string;
}

export interface LoginResponseDto {
  /** Assumed main token field; fallback handled in service if it's named `token` server-side */
  accessToken: string;
  /** Optional, if your backend returns it */
  refreshToken?: string;
  /** ISO string if backend returns explicit expiration */
  expiresAtUtc?: string;
  /** e.g., "Bearer" */
  tokenType?: string;
  /** Allow extra fields without breaking */
  [key: string]: unknown;
}

export interface VerifyEmailDto {
  userId: string;
  activationToken: string;
}