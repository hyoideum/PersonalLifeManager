import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { I18nService } from '../../../services/i18n.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { passwordValidator } from '../../../shared/validators/password.validator';
import { matchPasswordValidator } from '../../../shared/validators/match-password.validator';

const passwordErrorMessages: Record<string, string> = {
  uppercase: 'Must contain uppercase letter',
  number: 'Must contain number',
  special: 'Must contain special character',
  minlength: 'Minimum 6 characters'
};

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
        email: ['', [Validators.required, Validators.email]],
        username: ['', Validators.required],
        password: ['', [Validators.required, passwordValidator()]],
        confirmPassword: ['', [Validators.required, passwordValidator()]]
      },
      { validators: matchPasswordValidator() },
    );

    this.form.valueChanges.subscribe(() => {
      this.errorMessage = '';
    });
  }

  getFieldError(field: string): string | null {
    const control = this.form.get(field);

    if (!control || !control.errors || !(control.dirty || control.touched)) {
      return null;
    }

    const errors = control.errors;

    if (errors['required']) {
      return this.i18n.get('formErrors.required');
    }

    for (const key of Object.keys(errors)) {
      if (passwordErrorMessages[key]) {
        return passwordErrorMessages[key];
      }
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
        const errors = err.error?.errors;

        const message = Array.isArray(errors)
          ? errors.join('\n')
          : this.i18n.get('messages.registerError');

        Swal.fire({
          icon: 'error',
          title: this.i18n.get('messages.error'),
          text: message
        });
      }
    });
  }



}