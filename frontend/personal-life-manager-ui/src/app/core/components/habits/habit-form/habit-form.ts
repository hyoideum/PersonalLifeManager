import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { HabitStatistics } from '../../../models/habit.statistics.model';
import { Habit } from '../../../models/habit.model';

@Component({
  selector: 'app-habit-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './habit-form.html',
})
export class HabitFormComponent {
  @Input() habit?: HabitStatistics; // za edit
  @Output() save = new EventEmitter<Partial<Habit>>();

  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit() {
    if (this.habit) {
      this.form.patchValue({
        name: this.habit.name,
        description: this.habit.description
      });
    }
  }

  submit() {
    if (this.form.invalid) return;
    this.save.emit(this.form.value);
  }
}