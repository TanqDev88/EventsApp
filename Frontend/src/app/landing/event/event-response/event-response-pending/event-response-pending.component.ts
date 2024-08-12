import { Component, Input } from '@angular/core';
import { PurchaseCheckDto, PurchaseDto } from '@proxy/events';

@Component({
  selector: 'app-event-response-pending',
  templateUrl: './event-response-pending.component.html',
  styleUrls: ['./event-response-pending.component.scss']
})
export class EventResponsePendingComponent {
  @Input() purchase: PurchaseDto;
  @Input() purchaseCheck: PurchaseCheckDto;
}
