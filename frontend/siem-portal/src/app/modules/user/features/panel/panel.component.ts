// src/app/modules/user/features/panel/panel.component.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../user/services/auth.service';
import { UserApiService, UserDto } from '../../../user/services/user-api.service';
import { CreateUserDialogComponent } from '../../../shared/ui/dialogs/user/create-user.dialog';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-user-panel',
  standalone: true,
  imports: [CommonModule, NgFor, NgIf, AsyncPipe, MatButtonModule, MatDialogModule, MatSnackBarModule],
  templateUrl: './panel.component.html',
  styleUrl: './panel.component.scss'
})
export class UserPanelComponent implements OnInit {
  private readonly api = inject(UserApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snack = inject(MatSnackBar);
  readonly auth = inject(AuthService);

  users = signal<UserDto[]>([]);
  loading = signal<boolean>(false);

  ngOnInit(): void { this.reload(); }

  reload(): void {
    this.loading.set(true);
    this.api.getUsers().subscribe({
      next: list => this.users.set(list),
      error: () => this.snack.open('Failed to load users', 'Dismiss', { duration: 3500 }),
      complete: () => this.loading.set(false)
    });
  }

  openCreateUser(): void {
    const ref = this.dialog.open(CreateUserDialogComponent, {
        width: '640px',
        maxWidth: '95vw',
        panelClass: ['siem-dialog', 'siem-dialog--light'],
        backdropClass: 'siem-backdrop',
        autoFocus: false
    });
    ref.afterClosed().subscribe(r => { if (r === 'created') this.reload(); });
  }

  hasMonitoring(u: UserDto) { return u.roles?.includes('monitoring'); }
  isAdmin(u: UserDto) { return Array.isArray(u.roles) && u.roles.includes('administrator'); }


  toggleMonitoring(u: UserDto): void {
    const action = this.hasMonitoring(u) ? 'remove' : 'add';
    this.api.modifyRole(u.userId, action as 'add'|'remove', 'monitoring').subscribe({
      next: () => {
        this.snack.open(
          action === 'add' ? 'Granted monitoring role' : 'Removed monitoring role',
          'OK',
          { duration: 2500 }
        );
        this.reload();
      },
      error: () => this.snack.open('Role change failed', 'Dismiss', { duration: 3500 })
    });
  }
}
