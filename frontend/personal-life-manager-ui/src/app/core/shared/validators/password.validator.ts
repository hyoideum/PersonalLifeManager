import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn
} from '@angular/forms';

export function passwordValidator(): ValidatorFn {

  return (control: AbstractControl): ValidationErrors | null => {

    const value = control.value || '';

    const errors: Record<string, boolean> = {};

    if (!/[A-Z]/.test(value)) {
      errors['uppercase'] = true;
    }

    if (!/\d/.test(value)) {
      errors['number'] = true;
    }

    if (!/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
      errors['special'] = true;
    }

    if (value.length < 6) {
      errors['minlength'] = true;
    }

    return Object.keys(errors).length > 0
      ? errors
      : null;
  };
}