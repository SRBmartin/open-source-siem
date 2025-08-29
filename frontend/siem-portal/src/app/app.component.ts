import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppLoaderComponent } from "./modules/shared/loader/loader.component";
import { SidebarComponent } from './modules/shared/ui/sidebar/sidebar.component';
import { AsyncPipe, CommonModule } from '@angular/common';

// If you already have an AuthService, use that instead of this placeholder path.
import { AuthService } from './modules/user/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, AppLoaderComponent, SidebarComponent, AsyncPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'siem-portal';
  private readonly auth = inject(AuthService);

  isLoggedIn$ = this.auth.isLoggedIn$;
}
