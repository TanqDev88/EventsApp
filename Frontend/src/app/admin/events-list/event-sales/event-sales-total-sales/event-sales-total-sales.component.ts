import { LocalizationService } from '@abp/ng.core';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-event-sales-total-sales',
  templateUrl: './event-sales-total-sales.component.html',
  styleUrls: ['./event-sales-total-sales.component.scss']
})
export class EventSalesTotalSalesComponent implements OnChanges {
  @Input() totalTicketsSold: number;
  @Input() lastTimePurchase: string;

  load:boolean = false;
  lastLoad: string = 'cargando...';
  total: number = 0;
  isPurchase: boolean = true;

  constructor(private localizationService: LocalizationService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.totalTicketsSold) {
      this.total = this.totalTicketsSold;
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
