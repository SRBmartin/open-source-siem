// src/app/modules/logging/services/logs-api.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environment/environment';

export interface LogDocDto {
  id: string;
  timestamp: string;              // ISO from backend
  message?: string | null;
  severityText?: string | null;
  severityNumber?: number | null;
  siemTopic?: string | null;
  source?: Record<string, unknown> | null;
}

export interface LogSearchRequestDto {
  tagId?: string | null;
  topic?: string | null;
  text?: string | null;
  severity?: string | null;       // e.g. ERROR | WARN | INFO | DEBUG | TRACE
  from?: string | null;           // ISO string
  to?: string | null;             // ISO string
  size?: number;                  // default 50
  cursor?: string | null;
}

export interface LogSearchResultDto {
  hits: LogDocDto[];
  nextCursor?: string | null;
}

@Injectable({ providedIn: 'root' })
export class LogsApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/logs`;

  search(req: LogSearchRequestDto): Observable<LogSearchResultDto> {
    const clean = {
      tagId: req.tagId ?? null,
      topic: req.topic ?? null,
      text: req.text ?? null,
      severity: req.severity ?? null,
      from: req.from ?? null,
      to: req.to ?? null,
      size: req.size ?? 50,
      cursor: req.cursor ?? null
    };
    return this.http.post<LogSearchResultDto>(`${this.base}/search`, clean).pipe(
      map(r => ({
        hits: r.hits ?? [],
        nextCursor: r.nextCursor ?? null
      }))
    );
  }
}
