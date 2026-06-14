import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ResultCardComponent } from './result-card.component';
import { FlightStatusResult } from '../../models/FlightStatusResult';

describe('ResultCardComponent', () => {
  let component: ResultCardComponent;
  let fixture: ComponentFixture<ResultCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResultCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ResultCardComponent);
    component = fixture.componentInstance;
  });

  function setStatus(status: FlightStatusResult) {
    component.status = status;
    component.error = '';
    fixture.detectChanges();
  }

  it('should render OnTime status with green badge', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'OnTime',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: null,
      gate: null,
      delayReason: null,
      lastUpdatedUtc: null,
      sourceProvider: null,
      message: null
    });

    const badge = fixture.debugElement.query(By.css('.status-badge'))?.nativeElement;
    expect(badge.textContent.trim()).toBe('OnTime');
    expect(badge.classList).toContain('green');
  });

  it('should render Delayed status with amber badge', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'Delayed',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: null,
      gate: null,
      delayReason: null,
      lastUpdatedUtc: null,
      sourceProvider: null,
      message: null
    });

    const badge = fixture.debugElement.query(By.css('.status-badge'))?.nativeElement;
    expect(badge.textContent.trim()).toBe('Delayed');
    expect(badge.classList).toContain('amber');
  });

  it('should render Cancelled status with red badge', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'Cancelled',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: null,
      gate: null,
      delayReason: null,
      lastUpdatedUtc: null,
      sourceProvider: null,
      message: null
    });

    const badge = fixture.debugElement.query(By.css('.status-badge'))?.nativeElement;
    expect(badge.textContent.trim()).toBe('Cancelled');
    expect(badge.classList).toContain('red');
  });

  it('should render Diverted status with red badge', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'Diverted',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: null,
      gate: null,
      delayReason: null,
      lastUpdatedUtc: null,
      sourceProvider: null,
      message: null
    });

    const badge = fixture.debugElement.query(By.css('.status-badge'))?.nativeElement;
    expect(badge.textContent.trim()).toBe('Diverted');
    expect(badge.classList).toContain('red');
  });

  it('should render Unknown status with grey badge', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'Unknown',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: null,
      gate: null,
      delayReason: null,
      lastUpdatedUtc: null,
      sourceProvider: null,
      message: null
    });

    const badge = fixture.debugElement.query(By.css('.status-badge'))?.nativeElement;
    expect(badge.textContent.trim()).toBe('Unknown');
    expect(badge.classList).toContain('grey');
  });

  it('should display AeroTrack fields when present', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'Delayed',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: 'T3',
      gate: 'C7',
      delayReason: 'Airport closed',
      lastUpdatedUtc: null,
      sourceProvider: 'AeroTrack',
      message: null
    });

    expect(fixture.nativeElement.textContent).toContain('Gate');
    expect(fixture.nativeElement.textContent).toContain('Terminal');
    expect(fixture.nativeElement.textContent).toContain('Delay reason');
  });

  it('should hide AeroTrack fields when absent', () => {
    setStatus({
      flightNumber: 'AT100',
      date: new Date('2026-06-04'),
      status: 'OnTime',
      scheduledDepartureUtc: null,
      actualDepartureUtc: null,
      scheduledArrivalUtc: null,
      actualArrivalUtc: null,
      terminal: null,
      gate: null,
      delayReason: null,
      lastUpdatedUtc: null,
      sourceProvider: null,
      message: null
    });

    expect(fixture.nativeElement.textContent).not.toContain('Gate');
    expect(fixture.nativeElement.textContent).not.toContain('Terminal');
    expect(fixture.nativeElement.textContent).not.toContain('Delay reason');
  });

  it('should display an error message when error is provided', () => {
    component.status = null;
    component.error = 'Failed to load status';
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Failed to load status');
    expect(fixture.debugElement.query(By.css('mat-card.error-card'))).toBeTruthy();
  });
});
