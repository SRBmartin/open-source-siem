import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BasicButtonComponent } from '../../../shared/ui/app-button/app-button.component';
import { BasicInputComponent } from '../../../shared/ui/app-input/app-input.component';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LoaderService } from '../../../../shared/services/loader.service';

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

  constructor(
    private readonly fb: FormBuilder,
    private readonly loaderService: LoaderService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onLogin(): void {
    if (this.loginForm.valid) {
      const email = this.loginForm.get('email')?.value;
      const password = this.loginForm.get('password')?.value;
      console.log('Login with:', { email, password });

      this.loaderService.startLoading();
      setTimeout(() => {
        console.log("Loader finished after 3 seconds!");
        this.loaderService.stopLoading();
        // Hide your loader or perform the next action here
      }, 3000); // 3000 milliseconds = 3 seconds
      

    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}
