import { Component, Input } from '@angular/core';
import { PurchaseCheckDto, PurchaseDto } from '@proxy/events';

@Component({
  selector: 'app-event-response-error',
  templateUrl: './event-response-error.component.html',
  styleUrls: ['./event-response-error.component.scss']
})
export class EventResponseErrorComponent {
  @Input() purchase: PurchaseDto;
  @Input() purchaseCheck: PurchaseCheckDto;
}
