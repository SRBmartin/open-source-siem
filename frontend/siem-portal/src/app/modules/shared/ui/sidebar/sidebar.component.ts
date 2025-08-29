import { Component, inject } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

import { AuthService } from '../../../user/services/auth.service';
import { LogoutConfirmDialogComponent } from '../dialogs/logout/logout-confirm.dialog';

type NavItem = { label: string; path: string; icon: string; };

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, NgFor, MatDialogModule, MatButtonModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  private readonly dialog = inject(MatDialog);
  private readonly auth   = inject(AuthService);
  private readonly router = inject(Router);

  navItems: NavItem[] = [
    { label: 'Users', path: '/panel', icon: 'dashboard' },
    { label: 'Logs',      path: '/logs',      icon: 'list' },
    { label: 'Tags',      path: '/tags',      icon: 'tag' },
    { label: 'Settings',  path: '/user/settings',  icon: 'settings' },
  ];

  onLogoutClick(): void {
    const ref = this.dialog.open(LogoutConfirmDialogComponent, {
      disableClose: true,
      panelClass: 'siem-dialog',
      autoFocus: false
    });

    ref.afterClosed().subscribe(result => {
      if (result === 'loggedOut') {
        // send user to login (adjust the path if yours differs)
        this.router.navigateByUrl('/login');
      }
    });
  }
}
