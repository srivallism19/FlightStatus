import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { FlightStatusResult } from '../../models/FlightStatusResult';
import { FlightStatusService } from '../../services/flight-status.service';
import { SearchFormComponent } from './search-form.component';

describe('SearchFormComponent', () => {
  let component: SearchFormComponent;
  let fixture: ComponentFixture<SearchFormComponent>;
  let flightStatusServiceSpy: { getStatus: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    flightStatusServiceSpy = { getStatus: vi.fn() };

    await TestBed.configureTestingModule({
      imports: [
        SearchFormComponent,
        MatDatepickerModule,
        MatNativeDateModule,
        MatIconModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule
      ],
      providers: [{ provide: FlightStatusService, useValue: flightStatusServiceSpy }]
    }).compileComponents();

    fixture = TestBed.createComponent(SearchFormComponent);
    component = fixture.componentInstance;
  });

  it('should require flight number', () => {
    component.form.setValue({ flightNumber: '', date: new Date('2026-06-04') });
    expect(component.form.invalid).toBe(true);
    expect(component.flightNumber.errors?.['required']).toBe(true);
  });

  it('should require date', () => {
    component.form.setValue({ flightNumber: 'AT100', date: '' });
    expect(component.form.invalid).toBe(true);
    expect(component.date.errors?.['required']).toBe(true);
  });

  it('should call getStatus when form is valid', async () => {
    const apiResult: FlightStatusResult = {
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'OnTime',
      scheduledDepartureUtc: new Date('2026-06-04T09:00:00Z'),
      actualDepartureUtc: new Date('2026-06-04T09:05:00Z'),
      scheduledArrivalUtc: new Date('2026-06-04T12:00:00Z'),
      actualArrivalUtc: null,
      terminal: 'T3',
      gate: 'C7',
      delayReason: 'Airport closed',
      lastUpdatedUtc: new Date('2026-06-04T10:00:00Z'),
      sourceProvider: 'AeroTrack',
      message: 'Flight rerouted to alternate airport'
    };

    flightStatusServiceSpy.getStatus.mockReturnValue(of(apiResult));
    component.form.setValue({ flightNumber: 'AT100', date: new Date('2026-06-04') });

    fixture.detectChanges();
    const submitButton = fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement;
    submitButton.click();
    fixture.detectChanges();

    expect(flightStatusServiceSpy.getStatus).toHaveBeenCalledWith('AT100', '2026-06-04');
    expect(flightStatusServiceSpy.getStatus).toHaveBeenCalledTimes(1);
    expect(component.status).toEqual(apiResult);
    expect(component.error).toBe('');
  });

  it('should display error state when service fails', async () => {
    flightStatusServiceSpy.getStatus.mockReturnValue(throwError(() => new Error('Bad request')));
    component.form.setValue({ flightNumber: 'AT100', date: new Date('2026-06-04') });

    fixture.detectChanges();
    const submitButton = fixture.debugElement.query(By.css('button[type="submit"]')).nativeElement;
    submitButton.click();
    fixture.detectChanges();

    expect(component.error).toBe('Bad request');
    expect(component.status).toBeNull();
    const errorCard = fixture.debugElement.query(By.css('app-result-card'));
    expect(errorCard).toBeTruthy();
  });
});
