// src/app/modules/shared/ui/dialogs/tags/manage-access.dialog.ts
import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { CommonModule, NgFor, NgIf } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TagApiService, TagDetailDto, TagRoleLabel } from '../../../../tags/services/tag-api.service';

@Component({
  selector: 'app-manage-access-dialog',
  standalone: true,
  imports: [
    CommonModule, NgFor, NgIf, ReactiveFormsModule,
    MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSnackBarModule
  ],
  templateUrl: './manage-access.dialog.html',
  styleUrl: './manage-access.dialog.scss'
})
export class ManageAccessDialogComponent implements OnInit {
  private readonly api = inject(TagApiService);
  private readonly fb = inject(FormBuilder);
  private readonly snack = inject(MatSnackBar);
  private readonly ref = inject(MatDialogRef<ManageAccessDialogComponent>);

  loading = signal(false);
  tag = signal<TagDetailDto | null>(null);

  roles: TagRoleLabel[] = ['Reader', 'Admin'];

  form = this.fb.nonNullable.group({
    userId: ['', Validators.required],
    role: ['', Validators.required]
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: { tagId: string; name: string }) {}

  ngOnInit(): void {
    this.loading.set(true);
    this.api.getById(this.data.tagId).subscribe({
      next: d => this.tag.set(d),
      error: () => this.snack.open('Failed to load access list', 'Dismiss', { duration: 3000 }),
      complete: () => this.loading.set(false)
    });
  }

  grant(): void {
    if (this.form.invalid) return;
    const v = this.form.getRawValue();
    this.api.grantAccess(this.data.tagId, v.userId, v.role).subscribe({
      next: () => {
        this.snack.open('Access granted', 'OK', { duration: 2000 });
        this.form.reset({ userId: '', role: 'Reader' }); // reset to default
        this.ngOnInit(); // reload access list
      },
      error: () => this.snack.open('Failed to grant access', 'Dismiss', { duration: 3000 })
    });
  }

  close(): void { this.ref.close(true); }
}
