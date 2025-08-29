// src/app/modules/logging/features/tags/tags.component.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TagApiService, TagListItemDto } from '../../services/tag-api.service';
import { CreateTagDialogComponent } from '../../../shared/ui/dialogs/tags/create-tag.dialog';
import { IngestDialogComponent } from '../../../shared/ui/dialogs/ingest/ingest.dialog';
import { ManageAccessDialogComponent } from '../../../shared/ui/dialogs/tags-access/manage-access.dialog';

@Component({
  selector: 'app-tags',
  standalone: true,
  imports: [CommonModule, NgFor, NgIf, MatButtonModule, MatDialogModule, MatSnackBarModule],
  templateUrl: './tags.component.html',
  styleUrl: './tags.component.scss'
})
export class TagsComponent implements OnInit {
  private readonly api = inject(TagApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);

  loading = signal(false);
  tags = signal<TagListItemDto[]>([]);

  ngOnInit(): void { this.reload(); }

  reload(): void {
    this.loading.set(true);
    this.api.getMyTags().subscribe({
      next: list => this.tags.set(list),
      error: () => this.snack.open('Failed to load tags', 'Dismiss', { duration: 3000 }),
      complete: () => this.loading.set(false)
    });
  }

  openCreate(): void {
    const ref = this.dialog.open(CreateTagDialogComponent, {
      width: '640px',
      maxWidth: '95vw',
      panelClass: ['siem-dialog', 'siem-dialog--light'],
      autoFocus: false
    });

    ref.afterClosed().subscribe((res?: { created: boolean; tagId: string; ingest: any }) => {
      if (res?.created) {
        this.reload();
        // immediately show ingest instructions
        this.dialog.open(IngestDialogComponent, {
          width: '680px',
          maxWidth: '95vw',
          panelClass: ['siem-dialog', 'siem-dialog--light'],
          data: res.ingest
        });
      }
    });
  }

  openIngest(tag: TagListItemDto): void {
    // If the list item already contains ingest, show it; otherwise fetch by id
    if (tag.ingest) {
      this.dialog.open(IngestDialogComponent, {
        width: '680px',
        maxWidth: '95vw',
        panelClass: ['siem-dialog', 'siem-dialog--light'],
        data: tag.ingest
      });
    } else {
      this.api.getById(tag.tagId).subscribe({
        next: d => this.dialog.open(IngestDialogComponent, {
          width: '680px',
          maxWidth: '95vw',
          panelClass: ['siem-dialog', 'siem-dialog--light'],
          data: d.ingest
        }),
        error: () => this.snack.open('Failed to load ingest instructions', 'Dismiss', { duration: 3000 })
      });
    }
  }

  manageAccess(tag: TagListItemDto): void {
    const ref = this.dialog.open(ManageAccessDialogComponent, {
      width: '760px',
      maxWidth: '96vw',
      panelClass: ['siem-dialog', 'siem-dialog--light'],
      autoFocus: false,
      data: { tagId: tag.tagId, name: tag.name }
    });
    ref.afterClosed().subscribe(changed => { if (changed) this.reload(); });
  }
}
