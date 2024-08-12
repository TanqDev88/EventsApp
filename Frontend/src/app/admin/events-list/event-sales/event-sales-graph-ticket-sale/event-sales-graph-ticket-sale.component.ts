import { LocalizationService } from '@abp/ng.core';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ChartConfiguration } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-event-sales-graph-ticket-sale',
  templateUrl: './event-sales-graph-ticket-sale.component.html',
  styleUrls: ['./event-sales-graph-ticket-sale.component.scss']
})
export class EventSalesGraphTicketSaleComponent implements OnInit {

  @Input() sectorId: number = 0;
  @Input() sector: any;
  load:boolean = false;
  lastLoad: string = 'cargando...';
  total: number = 0;
  price: number = 0.00;
  priceTotal: number = 0.00;
  isPurchase = true;

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  constructor(private localizationService: LocalizationService) {}

  entriesSale: number = 0;
  entriesAvailable: number = 240;

  // Doughnut
  public doughnutChartLabels: string[] = [ this.localizationService.instant("::Solds"), this.localizationService.instant("::Availables") ];
  public doughnutChartDatasets: ChartConfiguration<'doughnut'>['data']['datasets'] = [
      { data: [ ], label: "" }
  ];

  public doughnutChartOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true
  };

  ngOnInit(): void {
    this.load = true;
    if(!this.sector.timeLastPurchase){
      this.isPurchase = false;
    }

    setTimeout(()=>{
      this.load = false;
      this.updateChartData();
    },4000);
  }

  updateChartData(): void {
    // Actualiza los datos del gráfico aquí
    this.doughnutChartDatasets = [
      { data: [ this.sector.sold, this.sector.available ], label: this.sector.ticketCategoryName + " - " + this.sector.ticketSectorName} // Nuevos datos
    ];

    this.total = this.sector.sold + this.sector.available;
    this.price = this.sector.price;
    this.priceTotal = this.sector.totalSold

    if(!this.sector.timeLastPurchase){
      this.isPurchase = false;
      this.lastLoad = this.localizationService.instant("::NoSales");
    }else{
      this.isPurchase = true;
      this.lastLoad = this.sector.timeLastPurchase;
    }

    // Si tienes una referencia al gráfico, puedes actualizarlo y animarlo
    if (this.chart) {
      this.chart.chart?.update();
    }
  }
}
