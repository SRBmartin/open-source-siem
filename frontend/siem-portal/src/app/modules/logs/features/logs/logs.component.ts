// src/app/modules/logging/features/logs/logs.component.ts
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { LogsApiService, LogDocDto, LogSearchRequestDto } from '../../services/logs-api.service';
import { TagApiService, TagListItemDto } from '../../../tags/services/tag-api.service';
import { LogDetailDialogComponent } from '../../../shared/ui/dialogs/log/log-detail.dialog';

@Component({
  selector: 'app-logs',
  standalone: true,
  imports: [
    CommonModule, NgFor, NgIf, DatePipe,
    ReactiveFormsModule,
    MatButtonModule, MatDialogModule, MatSnackBarModule,
    MatFormFieldModule, MatInputModule, MatSelectModule,
    MatDatepickerModule, MatNativeDateModule
  ],
  templateUrl: './logs.component.html',
  styleUrl: './logs.component.scss'
})
export class LogsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(LogsApiService);
  private readonly tagsApi = inject(TagApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);

  // filters form
  form = this.fb.nonNullable.group({
    tagId: [''],            // select from my tags (optional)
    severity: [''],         // '', ERROR, WARN, INFO, DEBUG, TRACE
    text: [''],             // lucene-like
    from: <Date | null>(null),
    to: <Date | null>(null),
    size: 50
  });

  tags = signal<TagListItemDto[]>([]);
  loading = signal(false);
  hits = signal<LogDocDto[]>([]);
  nextCursor = signal<string | null>(null);

  severities = ['ERROR','WARN','INFO','DEBUG','TRACE'];

  ngOnInit(): void {
    // preload tags for filter
    this.tagsApi.getMyTags().subscribe({
      next: t => this.tags.set(t),
      error: () => this.snack.open('Failed to load tags', 'Dismiss', { duration: 3000 })
    });

    // initial search (empty request => latest logs)
    this.search(true);
  }

  private buildRequest(resetCursor: boolean): LogSearchRequestDto {
    const v = this.form.getRawValue();
    const req: LogSearchRequestDto = {
      tagId: v.tagId || null,
      severity: v.severity || null,
      text: v.text?.trim() || null,
      from: v.from ? new Date(v.from).toISOString() : null,
      to: v.to ? new Date(v.to).toISOString() : null,
      size: v.size ?? 50,
      cursor: resetCursor ? null : this.nextCursor()
    };
    return req;
  }

  search(resetCursor = true): void {
    this.loading.set(true);
    const req = this.buildRequest(resetCursor);

    this.api.search(req).subscribe({
      next: res => {
        if (resetCursor) {
          this.hits.set(res.hits);
        } else {
          this.hits.update(prev => [...prev, ...(res.hits || [])]);
        }
        this.nextCursor.set(res.nextCursor ?? null);
      },
      error: () => this.snack.open('Failed to load logs', 'Dismiss', { duration: 3000 }),
      complete: () => this.loading.set(false)
    });
  }

  reset(): void {
    this.form.reset({ tagId: '', severity: '', text: '', from: null, to: null, size: 50 });
    this.search(true);
  }

  loadMore(): void {
    if (!this.nextCursor()) return;
    this.search(false);
  }

  openDetails(row: LogDocDto): void {
    this.dialog.open(LogDetailDialogComponent, {
      width: '900px',
      maxWidth: '96vw',
      panelClass: ['siem-dialog', 'siem-dialog--light'],
      autoFocus: false,
      data: {
        id: row.id,
        timestamp: row.timestamp,
        message: row.message,
        severityText: row.severityText,
        siemTopic: row.siemTopic,
        source: row.source
      }
    });
  }

  severityClass(s?: string | null) {
    switch ((s || '').toUpperCase()) {
      case 'ERROR': return 'sev error';
      case 'WARN':  return 'sev warn';
      case 'INFO':  return 'sev info';
      case 'DEBUG': return 'sev debug';
      case 'TRACE': return 'sev trace';
      default:      return 'sev';
    }
  }

getSeverity(row: LogDocDto): string {
    if (row.severityText) return row.severityText;
    const s = (row as any)['severity_text'];               // just in case it’s snake_cased
    if (s) return s;
    const src = row.source as any;
    // OTLP envelope fallback
    return src?.payload?.resourceLogs?.[0]?.scopeLogs?.[0]?.logRecords?.[0]?.severityText ?? 'n/a';
  }

}
