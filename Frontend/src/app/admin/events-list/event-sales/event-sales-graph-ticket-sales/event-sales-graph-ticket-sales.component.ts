import { LocalizationService } from '@abp/ng.core';
import { Component, OnInit, Input, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-event-sales-graph-ticket-sales',
  templateUrl: './event-sales-graph-ticket-sales.component.html',
  styleUrls: ['./event-sales-graph-ticket-sales.component.scss']
})
export class EventSalesGraphTicketSalesComponent implements OnInit, OnChanges {
  @Input() ticketsAvailableAndSold: any;

  load:boolean = false;
  isPurchase = true;
  lastLoad: string = 'cargando...';

  constructor(private localizationService: LocalizationService) {}

  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
  public barChartLegend = true;
  public barChartPlugins = [];

  public barChartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      { data: [], label: this.localizationService.instant("::Solds") , maxBarThickness: 100 },
      { data: [], label: this.localizationService.instant("::Availables") , maxBarThickness: 100 }
    ]
  };

  public barChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    scales:{
      x: {
        stacked: true,
      },
      y: {
        stacked: true
      }
    }
  };

  ngOnInit(): void {
    this.load = true;
    setTimeout(() => {
      this.load = false;

      if(!this.ticketsAvailableAndSold.lastTimePurchase){
        this.isPurchase = false;
        this.lastLoad = this.localizationService.instant("::NoSales");
      }

      // Forzar re-renderización del gráfico
      this.chart?.update();
    }, 4000);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.ticketsAvailableAndSold && this.ticketsAvailableAndSold) {
      this.updateChartData();
    }
  }

  updateChartData(): void {
    if (this.ticketsAvailableAndSold && this.ticketsAvailableAndSold.ticketCounts) {
      const labels = this.ticketsAvailableAndSold.ticketCounts.map(ticket => `${ticket.ticketCategoryName} - ${ticket.ticketSectorName}`);
      const soldData = this.ticketsAvailableAndSold.ticketCounts.map(ticket => ticket.sold);
      const availableData = this.ticketsAvailableAndSold.ticketCounts.map(ticket => ticket.available);

      this.barChartData.labels = labels;
      this.barChartData.datasets[0].data = soldData;
      this.barChartData.datasets[1].data = availableData;

      if(!this.ticketsAvailableAndSold.lastTimePurchase){
        this.isPurchase = false;
        this.lastLoad = this.localizationService.instant("::NoSales");
      }else{
        this.isPurchase = true;
        this.lastLoad = this.ticketsAvailableAndSold.lastTimePurchase;
      }

      // Forzar re-renderización del gráfico
      this.chart?.update();
    }
  }
}
