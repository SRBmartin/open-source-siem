// src/app/modules/logging/services/tag-api.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environment/environment';

export interface TagListItemDto {
  tagId: string;
  name: string;
  topic: string;
  myRole: string;
  ingest: unknown;
}

export interface TagAccessDto { userId: string; role: string; }
export interface TagDetailDto {
  tagId: string; name: string; topic: string;
  partitions: number; retentionMs: number;
  ingest: unknown; accesses: TagAccessDto[];
}

export interface CreateTagRequest { name: string; partitions?: number | null; retentionDays?: number | null; }

export interface IngestInstructionsDto {
  otlpHttpUrl: string; headerName: string; headerValue: string;
  authTokenUrl: string; clientId: string; audience: string;
}
export interface CreateLogTagResultDto {
  tagId: string; name: string; topic: string; ingest: IngestInstructionsDto;
}

interface ApiEnvelope<T> { success: boolean; data: T; errors: unknown[]; traceId: string; }

export type TagRoleCode = 0 | 1;
export type TagRoleLabel = 'Reader' | 'Admin';

const ROLE_TO_CODE: Record<TagRoleLabel, TagRoleCode> = {
  Reader: 0,
  Admin: 1
};

function normalizeRole(role: TagRoleLabel | TagRoleCode | string): TagRoleCode {
  if (typeof role === 'number') return role === 1 ? 1 : 0;
  const v = String(role).trim().toLowerCase();
  if (v === 'admin' || v === '1') return 1;
  return 0;
}

@Injectable({ providedIn: 'root' })
export class TagApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/tags`;

  getMyTags(): Observable<TagListItemDto[]> {
    return this.http.get<TagListItemDto[] | ApiEnvelope<TagListItemDto[]>>(this.base).pipe(
      map(r => Array.isArray(r) ? r : (r?.data ?? []))
    );
  }

  getById(tagId: string): Observable<TagDetailDto> {
    return this.http.get<TagDetailDto>(`${this.base}/${tagId}`);
  }

  create(req: CreateTagRequest): Observable<CreateLogTagResultDto> {
    return this.http.post<CreateLogTagResultDto>(this.base, req);
  }

  grantAccess(tagId: string, userId: string, role: TagRoleLabel | TagRoleCode | string) {
    return this.http.post<void>(`${this.base}/${tagId}/access`, {
      userId,
      role: normalizeRole(role)
    });
  }

}
