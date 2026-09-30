import { Directive, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Directive()
export abstract class DestroyComponent implements OnDestroy {
    protected readonly destroy = new Subject<void>();

    ngOnDestroy(): void {
        this.destroy.next();
        this.destroy.complete();
    }
}