// src/app/modules/user/services/user-api.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environment/environment';

export interface UserDto {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  isActivated: boolean;
  roles: string[];
}

interface ApiEnvelope<T> {
  success: boolean;
  data: T;
  errors: unknown[];
  traceId: string;
}

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/user`;

  getUsers(): Observable<UserDto[]> {
    return this.http
      .get<ApiEnvelope<UserDto[]>>(this.base)
      .pipe(map(r => r.data ?? []));
  }

  createUser(dto: { email: string; firstName: string; lastName: string }) {
    return this.http.post<string>(this.base, dto);
  }

  modifyRole(targetUserId: string, action: 'add'|'remove', role: string) {
    return this.http.post<void>(`${this.base}/roles`, { targetUserId, action, role });
  }
}
