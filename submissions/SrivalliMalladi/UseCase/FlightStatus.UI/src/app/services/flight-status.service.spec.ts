import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { FlightStatusService } from './flight-status.service';

describe('FlightStatusService', () => {
  let service: FlightStatusService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [FlightStatusService]
    });

    service = TestBed.inject(FlightStatusService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should request status with query params', () => {
    const rawApiResponse = {
      flightNumber: 'AB123',
      date: '2026-06-14',
      status: 'OnTime',
      scheduledDepartureUtc: '2026-06-14T09:00:00Z'
    };

    service.getStatus('AB123', '2026-06-14').subscribe((result) => {
      expect(result.flightNumber).toBe('AB123');
      expect(result.status).toBe('OnTime');
      expect(result.date instanceof Date).toBe(true);
      expect(result.scheduledDepartureUtc instanceof Date).toBe(true);
    });

    const req = httpMock.expectOne((request) =>
      request.method === 'GET' &&
      request.url.endsWith('/flights/status') &&
      request.params.get('flightNumber') === 'AB123' &&
      request.params.get('date') === '2026-06-14'
    );

    req.flush(rawApiResponse);
  });

  it('should propagate a BadRequest error message', () => {
    service.getStatus('INVALID', '2026-06-14').subscribe({
      next: () => {
        fail('expected error');
      },
      error: (error) => {
        expect(error.message).toContain('invalid');
      }
    });

    const req = httpMock.expectOne((request) =>
      request.method === 'GET' &&
      request.url === 'https://localhost:44380/flights/status' &&
      request.params.get('flightNumber') === 'INVALID' &&
      request.params.get('date') === '2026-06-14'
    );

    req.flush({ message: 'Bad request' }, { status: 400, statusText: 'Bad Request' });
  });
});
