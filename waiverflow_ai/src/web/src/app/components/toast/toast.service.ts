import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface ToastMessage {
  id: number;
  text: string;
  type: 'success' | 'error' | 'info';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private idx = 0;
  private _messages = new BehaviorSubject<ToastMessage[]>([]);
  messages$ = this._messages.asObservable();

  show(text: string, type: 'success' | 'error' | 'info' = 'info') {
    const msg: ToastMessage = { id: ++this.idx, text, type };
    this._messages.next([...this._messages.value, msg]);
    setTimeout(() => this.dismiss(msg.id), 4000);
  }

  dismiss(id: number) {
    this._messages.next(this._messages.value.filter(m => m.id !== id));
  }
}
