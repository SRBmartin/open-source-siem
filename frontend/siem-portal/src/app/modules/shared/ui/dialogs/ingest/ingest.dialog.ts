// src/app/modules/shared/ui/dialogs/tags/ingest.dialog.ts
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { ClipboardModule } from '@angular/cdk/clipboard';          // ← add
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar'; // optional feedback

type IngestData = {
  otlpHttpUrl: string;
  headerName: string;
  headerValue: string;
  authTokenUrl: string;
  clientId: string;
  audience: string;
};

@Component({
  selector: 'app-ingest-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    ClipboardModule,              // ← add
    MatSnackBarModule             // ← optional
  ],
  templateUrl: './ingest.dialog.html',
  styleUrl: './ingest.dialog.scss'
})
export class IngestDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public d: IngestData,
    private snack: MatSnackBar
  ) {}

  copied(msg: string) { this.snack.open(msg, 'OK', { duration: 1500 }); }
}
