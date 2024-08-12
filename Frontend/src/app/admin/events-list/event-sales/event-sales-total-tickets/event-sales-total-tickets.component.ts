import { LocalizationService } from '@abp/ng.core';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-event-sales-total-tickets',
  templateUrl: './event-sales-total-tickets.component.html',
  styleUrls: ['./event-sales-total-tickets.component.scss']
})
export class EventSalesTotalTicketsComponent implements OnChanges {
  @Input() totalTickets: number;
  @Input() lastTimePurchase: string;

  load:boolean = false;
  lastLoad: string = 'cargando...';
  total: number = 0;
  isPurchase: boolean = true;

  constructor(private localizationService: LocalizationService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.totalTickets) {
      this.total = this.totalTickets;
    }
    if (changes.lastTimePurchase) {
      if (!this.lastTimePurchase) {
        this.isPurchase = false;
        this.lastLoad = this.localizationService.instant("::NoSales");
      } else {
        this.isPurchase = true;
        this.lastLoad = this.lastTimePurchase;
      }
    }
    this.load = false;
  }

}
