import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-event-sales-total-available',
  templateUrl: './event-sales-total-available.component.html',
  styleUrls: ['./event-sales-total-available.component.scss']
})
export class EventSalesTotalAvailableComponent implements OnChanges {
  @Input() ticketsAvailable: number;

  load:boolean = false;
  total: number = 0;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.ticketsAvailable && changes.ticketsAvailable.currentValue !== undefined) {
      this.total = changes.ticketsAvailable.currentValue;
      this.load = false;
    }
  }

}
