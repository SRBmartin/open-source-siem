import { Component, inject } from '@angular/core';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule, NgIf } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../../../../user/services/auth.service';

@Component({
  selector: 'app-logout-confirm-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatProgressSpinnerModule, NgIf],
  templateUrl: './logout-confirm.dialog.html',
  styleUrl: './logout-confirm.dialog.scss'
})
export class LogoutConfirmDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<LogoutConfirmDialogComponent>);
  private readonly auth      = inject(AuthService);

  isLoading = false;
  errorMsg: string | null = null;

  cancel(): void {
    if (!this.isLoading) this.dialogRef.close('cancel');
  }

  confirm(): void {
    this.isLoading = true;
    this.errorMsg = null;
    this.auth.logout()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: () => this.dialogRef.close('loggedOut'),
        error: (err) => this.errorMsg = (err?.error?.message || 'Failed to sign out. Please try again.')
      });
  }
}
