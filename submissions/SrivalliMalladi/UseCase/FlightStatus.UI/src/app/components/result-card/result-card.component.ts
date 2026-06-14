import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { FlightStatusResult } from '../../models/FlightStatusResult';

@Component({
  selector: 'app-result-card',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  templateUrl: './result-card.component.html',
  styleUrls: ['./result-card.component.css']
})
export class ResultCardComponent {
  @Input() status: FlightStatusResult | null = null;
  @Input() error = '';

  get statusClass(): string {
    if (!this.status) {
      return 'grey';
    }

    switch (this.status.status) {
      case 'OnTime':
        return 'green';
      case 'Delayed':
        return 'amber';
      case 'Cancelled':
      case 'Diverted':
        return 'red';
      default:
        return 'grey';
    }
  }
}
