import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { ToastService } from '../components/toast/toast.service';
import { LoadingService } from './loading.service';

@Injectable({ providedIn: 'root' })
export class ApiInterceptor implements HttpInterceptor {
  private activeCalls = 0;

  constructor(private toast: ToastService, private loading: LoadingService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.activeCalls++;
    this.loading.set(true);

    return next.handle(req).pipe(
      finalize(() => {
        this.activeCalls--;
        if (this.activeCalls === 0) this.loading.set(false);
      }),
      catchError((err: HttpErrorResponse) => {
        const msg = err.status === 0
          ? 'Network error — check that the API server is running'
          : err.error?.detail || err.error?.title || err.message || 'An unexpected error occurred';
        this.toast.show(msg, 'error');
        return throwError(() => err);
      })
    );
  }
}
