import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { EventService } from '@proxy/events';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EventCodeGuard implements CanActivate {

  constructor(private router: Router, private eventService: EventService) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    const eventCode = route.params['code'];
    return this.eventService.validateCodeByCode(eventCode).pipe(
      map(response => {
        if (!response) {
          this.router.navigate(['/events']);
        }
        return response;
      })
    );
  }
}
