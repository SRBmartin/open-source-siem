// src/app/modules/shared/ui/dialogs/tags/create-tag.dialog.ts
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TagApiService } from '../../../../tags/services/tag-api.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-create-tag-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSnackBarModule],
  templateUrl: './create-tag.dialog.html',
  styleUrl: './create-tag.dialog.scss'
})
export class CreateTagDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(TagApiService);
  private readonly ref = inject(MatDialogRef<CreateTagDialogComponent>);
  private readonly snack = inject(MatSnackBar);

  creating = false;

  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    partitions: [6, [Validators.min(1)]],
    retentionDays: [7, [Validators.min(1)]]
  });

  submit(): void {
    if (this.form.invalid) return;
    this.creating = true;

    this.api.create(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.snack.open('Tag created', 'OK', { duration: 2000 });
        this.ref.close({ created: true, tagId: res.tagId, ingest: res.ingest });
      },
      error: () => {
        this.creating = false;
        this.snack.open('Failed to create tag', 'Dismiss', { duration: 3000 });
      }
    });
  }

  cancel(): void { if (!this.creating) this.ref.close(); }
}
