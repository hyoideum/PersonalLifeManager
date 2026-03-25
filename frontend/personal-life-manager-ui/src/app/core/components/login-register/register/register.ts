import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { I18nService } from '../../../services/i18n.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  form: FormGroup;

  errorMessage = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router, public i18n: I18nService) {
    this.form = this.fb.group(
      {
        firstName: ['', Validators.required],
        lastName: ['', Validators.required],
        email: ['', Validators.required, Validators.email],
        username: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.pattern(/^(?=.*[A-Z])(?=.*\d).{6,}$/)
          ]
        ],
        confirmPassword: ['', Validators.required]
      },
      { validators: this.passwordMatchValidator },
    );

    this.form.valueChanges.subscribe(() => {
      this.errorMessage = '';
    });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const pwd = control.get('password')?.value;
    const confirm = control.get('confirmPassword')?.value;

    return pwd && confirm && pwd !== confirm
      ? { mismatch: true }
      : null;
  }

  getFieldError(field: string): string | null {
  const control = this.form.get(field);

  if (!control || !(control.touched || control.dirty)) {
    return null;
  }

  if (control.hasError('required')) {
    return this.i18n.get('formErrors.required');
  }

  if (field === 'password' && control.hasError('pattern')) {
    return this.i18n.get('formErrors.password');
  }

  if (
    field === 'confirmPassword' &&
    this.form.hasError('mismatch') &&
    control.touched
  ) {
    return this.i18n.get('formErrors.passwordMissmatch');
  }

  return null;
}

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, password } = this.form.value;

    this.auth.register(this.form.value).subscribe({
      next: () => {
        Swal.fire({
          icon: 'success',
          title: this.i18n.get('messages.registerSuccess'),
          timer: 1500,
          showConfirmButton: false
        });

        this.auth.login({ username, password }).subscribe(() => {
          this.router.navigate(['/']);
        });
      },

      error: (err) => {
        Swal.fire({
          icon: 'error',
          title: this.i18n.get('messages.error'),
          text: err.error || this.i18n.get('messages.registerError')
        });
      }
    });
  }
}