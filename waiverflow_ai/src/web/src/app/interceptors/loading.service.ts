import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private _loading = new BehaviorSubject<boolean>(false);
  isLoading$ = this._loading.asObservable();

  set(v: boolean) { this._loading.next(v); }
}
