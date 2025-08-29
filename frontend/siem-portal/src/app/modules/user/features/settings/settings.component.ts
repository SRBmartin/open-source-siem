import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { UserApiService } from '../../../user/services/user-api.service';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

type PwForm = FormGroup<{
  currentPassword: FormControl<string>;
  newPassword: FormControl<string>;
  confirmPassword: FormControl<string>;
}>;

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatSnackBarModule
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(UserApiService);
  private readonly snack = inject(MatSnackBar);

  hideCurrent = signal(true);
  hideNew     = signal(true);
  hideConfirm = signal(true);
  saving      = signal(false);

  form: PwForm = this.fb.nonNullable.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [
      Validators.required,
      Validators.minLength(8),
      // the next four mirror your backend validator:
      Validators.pattern(/[A-Z]/),    // upper
      Validators.pattern(/[a-z]/),    // lower
      Validators.pattern(/[0-9]/),    // digit
      Validators.pattern(/[^a-zA-Z0-9]/) // special
    ]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: [differentFromCurrent, matchesNewAndConfirm] });

  // convenience getters for checklist styles
  get v() { return this.form.controls; }
  get hasLen()    { return this.v.newPassword.value.length >= 8; }
  get hasUpper()  { return /[A-Z]/.test(this.v.newPassword.value); }
  get hasLower()  { return /[a-z]/.test(this.v.newPassword.value); }
  get hasDigit()  { return /[0-9]/.test(this.v.newPassword.value); }
  get hasSpecial(){ return /[^a-zA-Z0-9]/.test(this.v.newPassword.value); }
  get notSameAsCurrent() { return this.v.currentPassword.value && this.v.newPassword.value && this.v.currentPassword.value !== this.v.newPassword.value; }
  get confirmOk() { return this.v.newPassword.value === this.v.confirmPassword.value && !!this.v.confirmPassword.value; }

  submit(): void {
    if (this.form.invalid) return;
    this.saving.set(true);

    this.api.changePassword({
      currentPassword: this.v.currentPassword.value,
      newPassword: this.v.newPassword.value
    }).subscribe({
      next: () => {
        this.snack.open('Password changed successfully', 'OK', { duration: 2500 });
        this.form.reset({ currentPassword: '', newPassword: '', confirmPassword: '' });
        this.saving.set(false);
      },
      error: (err) => {
        // surface backend errors if present
        const message =
          err?.error?.errors?.[0]?.description ??
          err?.error?.title ??
          'Failed to change password';
        this.snack.open(message, 'Dismiss', { duration: 3500 });
        this.saving.set(false);
      }
    });
  }
}

const differentFromCurrent: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const group = control as unknown as PwForm;
  const curr = group.get('currentPassword')?.value ?? '';
  const next = group.get('newPassword')?.value ?? '';
  return curr && next && curr === next ? { sameAsCurrent: true } : null;
};

/** confirmPassword must match newPassword */
const matchesNewAndConfirm: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const group = control as unknown as PwForm;
  const next = group.get('newPassword')?.value ?? '';
  const conf = group.get('confirmPassword')?.value ?? '';
  return next && conf && next !== conf ? { confirmMismatch: true } : null;
};