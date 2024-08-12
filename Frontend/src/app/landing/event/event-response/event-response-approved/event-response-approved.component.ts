import { Component, Input } from '@angular/core';
import { PurchaseCheckDto, PurchaseDto } from '@proxy/events';

@Component({
  selector: 'app-event-response-approved',
  templateUrl: './event-response-approved.component.html',
  styleUrls: ['./event-response-approved.component.scss']
})
export class EventResponseApprovedComponent {
  @Input() purchase: PurchaseDto;
  @Input() purchaseCheck: PurchaseCheckDto;
}
