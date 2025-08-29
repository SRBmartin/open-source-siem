// src/app/modules/logging/features/logs/log-detail.dialog.ts
import { Component, Inject, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { ClipboardModule } from '@angular/cdk/clipboard';

type LogDetailData = {
  id: string;
  timestamp: string;
  message?: string | null;
  severityText?: string | null;
  siemTopic?: string | null;
  source?: Record<string, unknown> | null;
};

@Component({
  selector: 'app-log-detail-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, ClipboardModule, DatePipe],
  templateUrl: './log-detail.dialog.html',
  styleUrl: './log-detail.dialog.scss'
})
export class LogDetailDialogComponent {
  pretty = signal<string>('');

  constructor(@Inject(MAT_DIALOG_DATA) public d: LogDetailData) {
    this.pretty.set(JSON.stringify(d.source ?? {}, null, 2));
  }
}
