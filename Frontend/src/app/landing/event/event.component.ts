import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { EventDateDto, EventDto, EventService } from '@proxy/events';
import { LayoutService } from '@services/layout.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EventTicketModalComponent } from './event-ticket-modal/event-ticket-modal.component';
import { ticketeraConst } from 'src/app/shared/shared.const';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.scss']
})
export class EventComponent implements OnInit, OnDestroy {
  private connection: HubConnection;
  public event: EventDto = null;
  saleForPerson: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private layoutService: LayoutService,
    private modalService: NgbModal,
    private titleService: Title,
  ) { }

  ngOnDestroy(): void {
    this.layoutService.setThemeClient(null);
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
  }

  ngOnInit() {
    let code = this.route.snapshot.paramMap.get('code');

    this.eventService.getByCodeByCode(code).subscribe(event => {
      this.titleService.setTitle(event.name + ticketeraConst.tixgoTitle);
      event.eventDates.forEach(eventDate => {
        eventDate.startDate = this.getFormattedDate(new Date(eventDate.startDate));
      });
      this.event = event;
      this.saleForPerson = event.saleForPerson;

      this.layoutService.setThemeClient(
        this.event.bgColor !== null && this.event.photoLogo !== null ?
          {
            isLight: false,
            bgColor: this.event.bgColor,
            logo: this.event.photoLogo
          } : null
      );
    });

    // -- Set Hub
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.apis.default.url + '/signalr-hubs/event')
      .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
      .build();

    // -- Receive Message NotificationTicket
    this.connection.on('ReceiveEventUpdate', data => {
      this.event.isActive = data;
    });

    // -- Receive Message NotificationTicket
    this.connection.on('EventAvailabilityUpdate', data => {
      this.event.eventStatus = data;
    });

    // -- Receive Message NotificationTicket
    this.connection.on('EventStatus', data => {
      this.event.eventStatus = data;
    });

    // -- Init connection
    const startConnection = () => {
      this.connection
        .start()
        .then(_ => {
          console.log('Connection Started');
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

  navegarAEditar() {
    this.router.navigate(['./event', this.event.code]);
  }

  isDateInPast(startDate: string): boolean {
    const currentDate = new Date();
    const eventDate = new Date(startDate);
    return eventDate.getTime() < currentDate.getTime();
  }

  buy(selectedEventDate: EventDateDto): void {
    const currentDate = new Date();
    const eventDate = new Date(selectedEventDate.startDate);

    if (eventDate.getTime() < currentDate.getTime()) {
      alert('No puedes comprar entradas para fechas pasadas.');
      return;
    }
    const modalRef = this.modalService.open(EventTicketModalComponent, {
      size: 'lg',
      backdrop: 'static'
    });

    modalRef.componentInstance.eventId = this.event.id;
    modalRef.componentInstance.eventDateId = selectedEventDate.id;
    modalRef.componentInstance.eventDateName = selectedEventDate.startDate;
    modalRef.componentInstance.saleForPerson = this.saleForPerson;
  }

  getFormattedDate(date: Date): string {
    return date.toISOString().substring(0, date.toISOString().length - 1);
  }
}
