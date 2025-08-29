import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../user/services/auth.service';

@Component({
  selector: 'app-email-verify',
  standalone: true,
  imports: [CommonModule, RouterLink, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './email-verify.component.html',
  styleUrl: './email-verify.component.scss'
})
export class EmailVerifyComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly auth   = inject(AuthService);

  loading   = signal(true);
  success   = signal<boolean | null>(null);
  message   = signal<string>('Verifying your email…');
  countdown = signal<number>(5); // seconds to redirect on success

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('userId');
    const token  = this.route.snapshot.paramMap.get('token');

    if (!userId || !token) {
      this.loading.set(false);
      this.success.set(false);
      this.message.set('Invalid verification link.');
      return;
    }

    this.auth.verifyEmail({ userId, activationToken: token }).subscribe({
      next: () => {
        this.success.set(true);
        this.message.set('Your email has been verified. You can now sign in.');
        this.loading.set(false);
        // auto-redirect to login with a short countdown
        const int = setInterval(() => {
          const n = this.countdown() - 1;
          this.countdown.set(n);
          if (n <= 0) {
            clearInterval(int);
            this.router.navigate(['/user', 'login'], { queryParams: { verified: 1 } });
          }
        }, 1000);
      },
      error: (err) => {
        this.loading.set(false);
        this.success.set(false);
        // try to surface backend errors; fall back to generic text
        const backend = err?.error;
        const firstErr = Array.isArray(backend) ? backend[0]?.description
                     : backend?.errors?.[0]?.description ?? backend?.title;
        this.message.set(firstErr || 'Verification failed. The link may be expired or already used.');
      }
    });
  }

  toLogin(): void {
    this.router.navigate(['/user', 'login']);
  }
}
