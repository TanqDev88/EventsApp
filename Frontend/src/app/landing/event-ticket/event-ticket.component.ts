import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { EventService } from '@proxy/events';
import { TicketInputDto, TicketOutDto } from '@proxy/tickets';
import { Subscription, interval } from 'rxjs';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-event-ticket',
  templateUrl: './event-ticket.component.html',
  styleUrls: ['./event-ticket.component.scss']
})
export class EventTicketComponent implements OnInit, OnDestroy {
  loadTicket: boolean = false;
  eventCode: string;
  public ticketId: number;
  public purchaseCode: string;
  input: TicketInputDto;
  public ticket: TicketOutDto = null;
  private connection: HubConnection;
  codeUrl: string = '';
  buttons: boolean = false;
  showQrDisabled: boolean = true;
  counter: number = 15;
  subscriptionCounter: Subscription;

  /**
   *
   */
  constructor(private route: ActivatedRoute, private eventService: EventService) {
    this.eventCode = this.route.snapshot.paramMap.get('eventcode');
    this.ticketId = Number.parseInt(this.route.snapshot.paramMap.get('ticketid'));
    this.purchaseCode = this.route.snapshot.paramMap.get('purchasecode');
  }

  ngOnInit(): void {
    this.input = {
      purchaseCode: this.purchaseCode,
      ticketId: this.ticketId,
    } as TicketInputDto;
    this.getTicket();
  }

  getTicket(loadSignalR:boolean = true): void {
    if (!this.loadTicket) {
      this.loadTicket = true;
      this.eventService.getTicketByInput(this.input).subscribe(result => {
        var date = new Date(result.eventDateStartDate);
        result.eventDateStartDate = date.toISOString().substring(0, date.toISOString().length - 1);
        this.ticket = result;
        const parts = this.ticket.title.split(':');
        this.ticket.title = parts[1].trim();
        const tenHour = 36000000;
        const dateEvent = new Date(this.ticket.eventDateStartDate);
        const timeLeft = dateEvent.getTime() - Date.now();

        if (timeLeft < tenHour) {
          this.showQrDisabled = false;
        }

        this.loadTicket = false;
        this.codeUrl = this.getCodeUrl();

        if (loadSignalR){
          this.initSignalR();
        }
      })
    }
  }

  ngOnDestroy(): void {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
    this.subscriptionCounter.unsubscribe();
  }

  initSignalR(): void {
    // -- Set Hub
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.apis.default.url + '/signalr-hubs/event')
      .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
      .build();
    console.log(this.ticket.eventDateStartDate);
    let conKey = this.eventCode + '-' + this.formatDate(this.ticket.eventDateStartDate);
    console.log(conKey);
    // -- Receive Message NotificationTicketSold
    this.connection.on(conKey, data => {
      const tenHour = 36000000;
      const dateEvent = new Date(this.ticket.eventDateStartDate);
      const timeLeft = dateEvent.getTime() - Date.now();

      if (timeLeft < tenHour) {
        this.showQrDisabled = false;
      }

      if (data){
        this.getTicket(false);
      }else{
        this.codeUrl = this.getCodeUrl();
      }
    });

    // -- Init connection
    const startConnection = () => {
      this.connection.start().then(_ => {
        console.log('Connection Started');
      }).catch(error => {
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

  formatDate(eventDateStartDate: string): string {
    const date = new Date(eventDateStartDate);

    const year = date.getFullYear().toString().slice(-2);
    const month = ('0' + (date.getMonth() + 1)).slice(-2);
    const day = ('0' + date.getDate()).slice(-2);
    const hour = ('0' + date.getHours()).slice(-2);
    const minute = ('0' + date.getMinutes()).slice(-2);
    return year + month + day + hour + minute;
  }

  shareByWhatsapp() {
    const url = window.location.href;
    const title = this.ticket.title;
    const description = '1 x ' + this.ticket.ticketCategoryName + ' - ' + this.ticket.ticketSectorName;

    const message = `${title} - ${description} \n ${url}`;
    const urlWhatsapp = `https://wa.me/?text=${encodeURIComponent(message)}`;

    window.open(urlWhatsapp, '_blank');
  }

  shareByEmail() {
    const url = window.location.href;
    const title = this.ticket.title;
    const description = '1 x ' + this.ticket.ticketCategoryName + ' - ' + this.ticket.ticketSectorName;

    const affair = encodeURIComponent(title);
    const body = encodeURIComponent(`${description} \n ${url}`);
    const urlEmail = `mailto:?subject=${affair}&body=${body}`;

    window.location.href = urlEmail;
  }

  showButtons(event: Event) {
    event.preventDefault();
    this.buttons = !this.buttons;
  }

  getCodeUrl(): string {
    let url = environment.apis.default.url + '/ticket/' + this.purchaseCode + '/' + this.ticketId + '?timestamp=' + Date.now();
    return url;
  }

  startCounter(): void {
    this.counter = 15;
    if (this.subscriptionCounter) {
      this.subscriptionCounter.unsubscribe();
    }
    this.subscriptionCounter = interval(1000).subscribe(() => {
      this.counter--;
      if (this.counter === 0) {
        this.subscriptionCounter.unsubscribe();
      }
    });
  }

  onImageLoad(): void {
    this.startCounter();
  }
}
