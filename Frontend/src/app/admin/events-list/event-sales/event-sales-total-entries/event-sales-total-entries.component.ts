import { LocalizationService } from '@abp/ng.core';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-event-sales-total-entries',
  templateUrl: './event-sales-total-entries.component.html',
  styleUrls: ['./event-sales-total-entries.component.scss']
})
export class EventSalesTotalEntriesComponent implements OnChanges{
  @Input() ticketsSold: number;
  @Input() lastTimePurchase: string;

  load:boolean = false;
  lastLoad: string = 'cargando...';
  total: number = 0;
  isPurchase: boolean = true;

  constructor(private localizationService: LocalizationService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.ticketsSold) {
      this.total = this.ticketsSold;
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
