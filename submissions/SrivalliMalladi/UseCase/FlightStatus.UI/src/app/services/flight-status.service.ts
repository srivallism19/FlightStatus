import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { FlightStatusResult } from '../models/FlightStatusResult';


@Injectable({
  providedIn: 'root'
})
export class FlightStatusService {
  private readonly apiUrl = environment.apiUrl + '/v1/flights/status';

  constructor(private readonly http: HttpClient) {}

  getStatus(flightNumber: string, date: string): Observable<FlightStatusResult> {
    const params = new HttpParams()
      .set('flightNumber', flightNumber)
      .set('date', date);

    //const parseDate = (v: any): Date | null => (v ? new Date(v) : null);

    return this.http.get<any>(this.apiUrl, { params }).pipe(
      map((resp) => {
        const parseUtc = (value: any): Date | null => (value ? new Date(value) : null);

        const result: FlightStatusResult = {
          flightNumber: resp.flightNumber,
          date: resp.date ? new Date(resp.date) : new Date(),
          status: (resp.status as FlightStatusResult['status']) || 'Unknown',
          scheduledDepartureUtc: parseUtc(resp.scheduledDepartureUtc),
          actualDepartureUtc: parseUtc(resp.actualDepartureUtc),
          scheduledArrivalUtc: parseUtc(resp.scheduledArrivalUtc),
          actualArrivalUtc: parseUtc(resp.actualArrivalUtc),
          terminal: resp.terminal ?? null,
          gate: resp.gate ?? null,
          delayReason: resp.delayReason ?? null,
          lastUpdatedUtc: parseUtc(resp.lastUpdatedUtc),
          sourceProvider: resp.sourceProvider ?? null,
          message: resp.message ?? null
        };

        return result;
      }),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(this.getErrorMessage(error)));
      })
    );
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 400) {
      return 'The flight number or date is invalid. Please verify your search and try again.';
    }

    if (error.status === 0) {
      return 'Unable to reach the flight status service. Check your network connection.';
    }

    return error.error?.message || `Flight status request failed with status ${error.status}.`;
  }
}
