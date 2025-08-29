import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BasicButtonComponent } from '../../../shared/ui/app-button/app-button.component';
import { BasicInputComponent } from '../../../shared/ui/app-input/app-input.component';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { LoaderService } from '../../../../shared/services/loader.service';
import { AuthService } from '../../services/auth.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    BasicButtonComponent,
    BasicInputComponent,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  errorMsg: string | null = null;

  constructor(
    private readonly fb: FormBuilder,
    private readonly loaderService: LoaderService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onLogin(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const email = this.loginForm.get('email')!.value as string;
    const password = this.loginForm.get('password')!.value as string;

    this.errorMsg = null;
    this.loaderService.startLoading();

    this.authService
      .loginAndStore({ username: email, password })
      .pipe(finalize(() => this.loaderService.stopLoading()))
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/');
        },
        error: (err) => {
          this.errorMsg = 'Invalid credentials.';
        }
      });
  }
}
