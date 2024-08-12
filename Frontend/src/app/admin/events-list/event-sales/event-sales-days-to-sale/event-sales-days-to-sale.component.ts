import { LocalizationService } from '@abp/ng.core';
import { Component, Input, OnChanges, OnInit } from '@angular/core';

@Component({
  selector: 'app-event-sales-days-to-sale',
  templateUrl: './event-sales-days-to-sale.component.html',
  styleUrls: ['./event-sales-days-to-sale.component.scss']
})
export class EventSalesDaysToSaleComponent implements OnChanges {

  @Input() daysToSale: number;
  load:boolean = false;
  days: number = 0;

  constructor(private localizationService: LocalizationService) {}

  ngOnChanges(): void {
    this.load = true;
    setTimeout(()=>{
      this.load = false;
      this.days = this.daysToSale;
    },5000);
  }

  get dayLabel(): string {
    return this.days === 1 ? this.localizationService.instant("::Day") : this.localizationService.instant("::Days");
  }
}
