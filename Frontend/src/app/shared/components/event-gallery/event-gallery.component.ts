import { AuthService, ListService, PagedResultDto } from '@abp/ng.core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { EventDto, EventResultRequestDto, EventService } from '@proxy/events';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-event-gallery',
  templateUrl: './event-gallery.component.html',
  styleUrls: ['./event-gallery.component.scss'],
  providers: [ListService]
})
export class EventGalleryComponent implements OnInit {

  @Input() pageSize: number = 6;
  @Input() isCarrousel: boolean = false;
  @Input() isMain: boolean = false;
  @Input() disableCard: boolean = false;
  @Input() showOptions: boolean = false;
  @Input() showNewEvent: boolean = false;
  @Input() order: string = 'asc';

  eventsLoaded: boolean = false;
  eventContext = { items: [], totalCount: 0 } as PagedResultDto<EventDto>;
  searchEvent = {} as EventResultRequestDto;
  events: EventDto[] = [];
  infoEvents: any = {};
  isCreator: boolean = false;
  private connection: HubConnection;

  get page(): number {
    return Math.ceil(this.events.length / this.pageSize);
  }

  get pages(): number {
    return Math.ceil(this.eventContext.totalCount / this.pageSize);
  }

  get existMoreEvent(): boolean {
    return this.page < this.pages;
  }

  constructor(
    public readonly list: ListService<EventResultRequestDto>,
    private eventService: EventService,
    private confirmation: ConfirmationService,
    private router: Router,
    private authService: AuthService,) { }

  ngOnInit(): void {
    this.getEvents();

    // -- Set Hub
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.apis.default.url + '/signalr-hubs/event')
      .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
      .build();

    // -- Receive Message NotificationTicket
    this.connection.on('EventStatusCode', data => {
      this.eventService.getByCodeByCode(data).subscribe(result => {
        const eventToUpdate = this.events.find(event => event.code === result.code);

        if (eventToUpdate) {
          eventToUpdate.eventStatus = result.eventStatus;
        }
      });
    });

    // -- Receive Message NotificationTicket
    this.connection.on('StatisticsEvents', data => {
      this.infoEvents = data;
    });

    // -- Init connection
    const startConnection = () => {
      this.connection
        .start()
        .then(_ => {
          console.log('Connection Started');
          if (this.authService.isAuthenticated && this.showOptions == true) {
            this.getInfoEvents();
          }
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

  private getEvents(): void {
    if (!this.eventsLoaded) {

      this.eventsLoaded = true;

      this.searchEvent.isMain = this.isMain;
      this.searchEvent.isOwner = this.showOptions;
      this.searchEvent.isAdmin = this.showOptions;
      this.searchEvent.isEditor = this.showOptions;
      this.searchEvent.isValidator = this.showOptions;
      this.searchEvent.maxResultCount = this.pageSize;
      this.searchEvent.order = this.order;

      const listEvent = (query) => this.eventService.getList({ ...query, ...this.searchEvent });
      this.list.hookToQuery(listEvent).subscribe((response) => {
        this.eventContext = response;
        this.eventContext.items.forEach(ev => {
          if(ev.isOwner){
            this.isCreator = true;
          }
          ev.eventDates.forEach(eventDate => {
            eventDate.startDate = this.getFormattedDate(new Date(eventDate.startDate));
          });
          this.events.push(ev);
        });
        this.eventsLoaded = false;
      });
    }
  }

  // -- On Search events
  search(): void {
    this.searchEvent.skipCount = 0;
    this.searchEvent.maxResultCount = this.pageSize;
    this.events = [];
    this.getEvents();
  }


  // -- Load more events
  onLoadMore(): void {
    if (this.existMoreEvent) {
      this.searchEvent.skipCount = this.pageSize * this.page;
      this.getEvents();
    }
  }

  // -- Get more events
  goTo(code: string): void {
    if (code) {
      this.router.navigateByUrl(`/event/${code}`);
    }
  }

  // -- Get more events
  goToValidate(ev: EventDto): void {
    if (ev) {
      this.router.navigateByUrl(`admin/events/${ev.id}/validate`);
    }
  }

  // -- Get more events
  goToSale(ev: EventDto): void {
    if (ev) {
      this.router.navigateByUrl(`admin/events/${ev.id}/sales`);
    }
  }

  selectedDates(ev: any) {
    this.searchEvent.dateFrom = ev.fromDate;
    this.searchEvent.dateTo = ev.toDate;
    this.search();
  }

  createOrEditEvent(id?: number): void {
    const routePath = id ? ['admin/events/create', id] : ['admin/events/create'];
    this.router.navigate(routePath);
  }

  delete(id: number) {
    this.confirmation.warn('::TheEventWillBeRemoved', '::AreYouSure',{id: id})
        .subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
            this.eventService.delete(id).subscribe(() => {
              this.events = this.events.filter(x => x.id !== id);
              this.eventContext.totalCount = this.eventContext.totalCount - 1;
            });
          }
      });
  }

  getFormattedDate(date: Date): string {
    return date.toISOString().substring(0, date.toISOString().length - 1);
  }

  toggleEventStatus(item: EventDto) {
    item.isActive = !item.isActive;
    this.eventService.updateEventStatusByIdAndIsActive(item.id, item.isActive).subscribe();
  }

  getInfoEvents(){
    this.eventService.getDashboardStatistics(0).subscribe();
  }
}
