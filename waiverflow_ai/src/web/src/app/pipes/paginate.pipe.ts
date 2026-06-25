import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'paginate', standalone: true })
export class PaginatePipe implements PipeTransform {
  transform<T>(arr: T[], page: number, pageSize: number): T[] {
    return arr.slice((page - 1) * pageSize, page * pageSize);
  }
}
