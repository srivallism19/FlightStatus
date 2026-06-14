import { CommonModule } from '@angular/common';
import { Component, ChangeDetectorRef } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { FlightStatusResult } from '../../models/FlightStatusResult';
import { FlightStatusService } from '../../services/flight-status.service';
import { ResultCardComponent } from '../result-card/result-card.component';

@Component({
  selector: 'app-search-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatDatepickerModule, MatNativeDateModule, MatIconModule, ResultCardComponent],
  templateUrl: './search-form.component.html',
  styleUrls: ['./search-form.component.css']
})
export class SearchFormComponent {
    status: FlightStatusResult | null = null;
      error = '';
      loading = false;
    
    constructor(private readonly flightStatusService: FlightStatusService, private cd: ChangeDetectorRef) {}

  readonly form = new FormGroup<{ flightNumber: FormControl<string>; date: FormControl<string | Date | null> }>({
    flightNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    }),
    date: new FormControl<string | Date | null>(null, [Validators.required])
  });

  get flightNumber() {
    return this.form.controls.flightNumber;
  }

  get date() {
    return this.form.controls.date;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dateValue = this.date.value;
    const formattedDate = this.formatDate(dateValue);

    const criteria = {
      flightNumber: this.flightNumber.value.trim(),
      date: formattedDate
    };

    this.onSearch(criteria);
  }

  private formatDate(date: any): string {
    if (!date) return '';
    
    // If date is already a string in the correct format, return it
    if (typeof date === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(date)) {
      return date;
    }

    // If date is a Date object, format it
    if (date instanceof Date) {
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    }

    return '';
  }

  onSearch(criteria: { flightNumber: string; date: string }): void {
    this.error = '';
    this.status = null;
    this.loading = true;

    this.flightStatusService.getStatus(criteria.flightNumber, criteria.date).subscribe({
      next: (result) => {
        this.status = result;
        this.loading = false;
        this.cd.detectChanges();
      },
      error: (error: Error) => {
        this.error = error.message;
        this.loading = false;
      }
    });
  }
}
