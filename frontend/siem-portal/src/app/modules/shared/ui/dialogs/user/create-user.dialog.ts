// create-user.dialog.ts
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { UserApiService } from '../../../../user/services/user-api.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-create-user-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule, NgIf,
    MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule, MatSnackBarModule
  ],
  templateUrl: './create-user.dialog.html',
  styleUrl: './create-user.dialog.scss'
})
export class CreateUserDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(UserApiService);
  private readonly ref = inject(MatDialogRef<CreateUserDialogComponent>);
  private readonly snack = inject(MatSnackBar);

  // 👉 non-nullable controls
  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required]
  });

  creating = false;

  submit(): void {
    if (this.form.invalid) return;
    this.creating = true;

    const dto = this.form.getRawValue();

    this.api.createUser(dto).subscribe({
      next: () => {
        this.snack.open('User created', 'OK', { duration: 2000 });
        this.ref.close('created');
      },
      error: () => {
        this.snack.open('Failed to create user', 'Dismiss', { duration: 3000 });
        this.creating = false;
      }
    });
  }

  cancel(): void { if (!this.creating) this.ref.close(); }
}
