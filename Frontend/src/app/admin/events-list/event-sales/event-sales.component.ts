import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { EventService } from '@proxy/events';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-event-sales',
  templateUrl: './event-sales.component.html',
  styleUrls: ['./event-sales.component.scss']
})
export class EventSalesComponent implements OnInit{
  eventId: number;
  ticketsAvailableAndSold: any = {};
  ticketsAvailableAndSoldForType: any = {};
  ticketsSold: number;
  ticketsAvailable: number;
  totalTickets: number;
  totalTicketsSold: number;
  lastTimePurchase: string;
  daysToRemaining: number;
  daysToSale: number;
  eventName: string;
  private connection: HubConnection;

  constructor(private route: ActivatedRoute, private eventService: EventService) {
    this.eventId = parseInt(this.route.snapshot.paramMap.get('id'));

  }

  ngOnInit(): void {
    // -- Set Hub
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.apis.default.url + '/signalr-hubs/event')
      .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
      .build();

    // -- Receive Message NotificationTicket
    this.connection.on('TicketsSoldAndAvailable', data => {
      this.ticketsAvailableAndSold = data;
    });

    // -- Receive Message NotificationTicket
    this.connection.on('TicketsSoldAndAvailableForType', data => {
      this.ticketsAvailableAndSoldForType = data;
    });

    // -- Receive Message NotificationTicket
    this.connection.on('TicketStatistic', data => {
      if (data.ticketCounts.length > 0) {
        this.ticketsSold = data.ticketCounts[0].sold;
        this.ticketsAvailable = data.ticketCounts[0].available;
        this.totalTickets = data.ticketCounts[0].total;
        this.totalTicketsSold = data.ticketCounts[0].totalSold;
    }
    this.lastTimePurchase = data.lastTimePurchase;
    });

    // -- Receive Message NotificationTicket
    this.connection.on('UpdateEventDates', data => {
      this.daysToRemaining = data.daysRemaining;
      this.daysToSale = data.daysOnSale;
    });

    // -- Receive Message NotificationTicket
    this.connection.on('ShowUpdatedEventName', data => {
      this.eventName = data;
    });


    // -- Init connection
    const startConnection = () => {
      this.connection
        .start()
        .then(_ => {
          console.log('Connection Started');
          this.eventService.getTicketsSoldAndAvailableByEventId(this.eventId).subscribe();
          this.eventService.getTicketsSoldAndAvailableForTypeByEventId(this.eventId).subscribe();
          this.eventService.getTicketsStatisticsByEventId(this.eventId).subscribe();
          this.eventService.updateEventDatesByEventId(this.eventId).subscribe();
          this.eventService.showUpdatedEventNameByEventId(this.eventId).subscribe();
        })
        .catch(error => {
          console.error('Error starting connection:', error);
          // Retrying connection after 5 seconds
          setTimeout(startConnection, 5000);
        });
    };

    startConnection();

    // -- Close connection
    this.connection.onclose(error => {
      console.log('Connection Closed:', error);
      // Reconnect
      startConnection();
    });
  }
}
