import { ToasterService } from '@abp/ng.theme.shared';
import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { EventService, TicketAvailableDto } from '@proxy/events';
import { PurchaseInputDto, PurchaseTicketDto, PurchaseTicketInputDto } from '@proxy/tickets';
import { LayoutService } from '@services/layout.service';
import { Observable } from 'rxjs';
import { ticketeraConst } from 'src/app/shared/shared.const';
import { environment } from 'src/environments/environment';

interface PurchaseTicketQuantityDto extends PurchaseTicketDto {
  quantity: number;
}

@Component({
  selector: 'app-event-ticket-modal',
  styleUrls: ['./event-ticket-modal.component.scss'],
  templateUrl: './event-ticket-modal.component.html',
})
export class EventTicketModalComponent implements OnInit, OnDestroy {
  private connection: HubConnection;
  isMobile: boolean = false;
  validateSaleForPerson: boolean;
  disabledIncrement: boolean;
  termsAccepted: boolean = false;
  loading: boolean = false;

  @Input() eventId: number;
  @Input() eventDateId: number;
  @Input() eventDateName: string;
  @Input() saleForPerson: number;

  code: string;
  countdownInterval: any;
  countdownMinutes: number;
  countdownTimeout: any;

  init: boolean = true;
  selectTickets: PurchaseTicketQuantityDto[] = [];
  ticketLoading: boolean = false;
  purchaseForm: FormGroup = this.fb.group({
    step1: this.fb.group({
      purchaseTickets: this.fb.array([]),
    }),
    step2: this.fb.group({
      name: ['', [Validators.required, Validators.minLength(4), Validators.pattern(/^[^\d]*$/)]],
      surname: ['', [Validators.required, Validators.minLength(4), Validators.pattern(/^[^\d]*$/)]],
      email: ['', [Validators.required, Validators.email]],
      confirmEmail: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.minLength(4), Validators.pattern(/^\d+$/)]],
      address: ['', [Validators.required, Validators.minLength(4)]],
    }),
  });

  step: number = 1;
  commission: number = 0;

  emailsMatchValidator(control: AbstractControl): ValidationErrors | null {
    const email = control.get('email').value;
    const confirmEmail = control.get('confirmEmail').value;

    console.log(email, confirmEmail);
    console.log(email !== confirmEmail);

    if (email !== confirmEmail) {
      return { emailsNotMatch: true };
    }

    return null;
  }

  constructor(
    private eventService: EventService,
    private fb: FormBuilder,
    private toaster: ToasterService,
    private layoutService: LayoutService,
    @Inject(NgbActiveModal) private activeModal: NgbActiveModal
  ) {
    // -- Init time regresive
    this.countdownMinutes = 10 * 60;

    // -- Check is mobile
    this.layoutService.isMobile.subscribe(result => {
      this.isMobile = result;
    });
  }

  ngOnDestroy(): void {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }

    // -- Clear time
    clearInterval(this.countdownInterval);
    localStorage.removeItem('countdownEndTime');
    document.removeEventListener('visibilitychange', this.handleVisibilityChange.bind(this));
  }

  ngOnInit(): void {
    if (localStorage.getItem('countdownEndTime')) {
      localStorage.removeItem('countdownEndTime');
    }

    document.addEventListener('visibilitychange', this.handleVisibilityChange.bind(this));
    console.log(this.saleForPerson);
    console.log(this.eventId);
    console.log(this.eventDateId);

    // -- Set init validation
    this.purchaseForm.get('step2').setValidators(this.emailsMatchValidator);

    // -- Subscribe changes to confirmEmail
    this.purchaseForm.get('step2.confirmEmail').valueChanges.subscribe(() => {
      this.purchaseForm.get('step2').updateValueAndValidity();
    });

    // -- Get total tickets
    this.getAvailableTickets(() => {
      this.eventService.purchaseCreate().subscribe(res => {
        this.code = res;
        console.log(this.code);
        this.startCountdown();
      });
    });

    // -- Set Hub
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.apis.default.url + '/signalr-hubs/event')
      .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
      .build();

    // -- Receive Message NotificationTicket
    this.connection.on('NotificationTicket', data => {
      if (data == this.eventId) {
        this.getAvailableTickets(() => {
          console.log('Refresh count tickets');
        });
      }
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

  // -- Buy tickets
  buy() {
    if (this.purchaseForm.invalid || !this.termsAccepted) return;

    this.ticketLoading = true;
    let purchaseInput = this.purchaseForm.get('step2').value as PurchaseInputDto;
    purchaseInput.code = this.code;
    purchaseInput.eventId = this.eventId;
    purchaseInput.eventDateId = this.eventDateId;

    console.log(purchaseInput);
    this.eventService
      .purchaseProcessByInputAndInputCommissionAndSeconds(purchaseInput, this.commission, this.countdownMinutes)
      .subscribe(
        result => {
          console.log(result);
          this.ticketLoading = false;
          location.href = result;
        },
        error => {
          console.log(error);
          this.toaster.error('No se pudo completar la operación, vuelva a intentarlo');
          this.cancel();
        }
      );
  }

  cancel() {
    this.activeModal.dismiss('Cross click');
    if (this.code) {
      this.eventService.purchaseCancelByCode(this.code).subscribe(() => {
        console.log('Cancel Purchase');
      });
    }

    // -- Clear time
    clearInterval(this.countdownInterval);
    localStorage.removeItem('countdownEndTime');
    document.removeEventListener('visibilitychange', this.handleVisibilityChange.bind(this));
  }

  // -- Got Step
  goStep(val: number) {
    this.step = val;
    if (this.step === 3) {
      this.termsAccepted = false;
      this.purchaseForm.updateValueAndValidity();
    }
  }

  // -- Get Available Tickets
  getAvailableTickets(func: Function) {
    let input = {
      eventId: this.eventId,
      eventDateId: this.eventDateId,
    } as TicketAvailableDto;

    this.eventService.getTicketAvailableByInput(input).subscribe(data => {
      if (this.init) {
        this.selectTickets = data.map(ticket => {
          const quantityDto = ticket as PurchaseTicketQuantityDto;
          quantityDto.quantity = 0;
          return quantityDto;
        });
      } else {
        data.forEach(ticket => {
          var index = this.selectTickets.findIndex(
            x =>
              x.ticketCategoryId == ticket.ticketCategoryId &&
              x.ticketSectorId == ticket.ticketSectorId
          );
          if (index != -1) {
            this.selectTickets[index].count = ticket.count;
          }
        });
      }

      if (func) {
        func();
      }

      this.init = false;
    });
  }

decrementQuantity(index: number) {
  if (this.loading) return;

  if (this.selectTickets[index].quantity > 0) {
    this.selectTickets[index].quantity--;
    this.disabledIncrement = false;
  }


  this.loading = true;
  this.updateCommission();
  this.addOrUpdateTicket(this.selectTickets[index]).subscribe(
    () => {
      this.ticketLoading = false;
      this.loading = false;
    },
    error => {
      this.selectTickets[index].quantity++;
      console.log(error);
      this.loading = false;
    }
  );
}


incrementQuantity(index: number) {
  if (this.loading || this.disabledIncrement) return;

  const totalQuantity = this.selectTickets.reduce((total, ticket) => total + ticket.quantity, 0);

  if (this.saleForPerson > 0 && totalQuantity >= this.saleForPerson) {
    this.disabledIncrement = true;
    return;
  }

  this.loading = true;
  this.eventService.validateTicketPurchase(this.eventId, totalQuantity).subscribe(
    result => {
      this.validateSaleForPerson = result;
      if (this.validateSaleForPerson || this.saleForPerson === 0) {
        this.selectTickets[index].quantity++;
        this.updateCommission();
        this.addOrUpdateTicket(this.selectTickets[index]).subscribe(
          () => {
            this.ticketLoading = false;
            this.loading = false;
            this.disabledIncrement = false;
          },
          error => {
            this.selectTickets[index].quantity--;
            console.log(error);
            this.loading = false;
          }
        );
      } else {
        this.disabledIncrement = true;
        this.loading = false;
      }
    },
    error => {
      console.log(error);
      this.loading = false;
    }
  );
}

  updateCommission() {
    let totalTicketPrice = 0;

    this.selectTickets.forEach(ticket => {
      totalTicketPrice += ticket.price * ticket.quantity;
    });


    this.commission = +(totalTicketPrice * ticketeraConst.mexicoCommissionRate).toFixed(2);
  }

  addOrUpdateTicket(ticket: PurchaseTicketQuantityDto): Observable<any> {
    this.ticketLoading = true;
    let input = {
      count: ticket.quantity,
      code: this.code,
      eventId: this.eventId,
      eventDateId: this.eventDateId,
      ticketCategoryId: ticket.ticketCategoryId,
      ticketSectorId: ticket.ticketSectorId,
    } as PurchaseTicketInputDto;

    return this.eventService.puchaseCreateOrUpdateTicketByInput(input);
  }

  get totalQuantity(): number {
    let totalQuantity = 0;
    this.selectTickets.forEach(ticket => {
      if (ticket.quantity > 0) {
        totalQuantity += ticket.price * ticket.quantity;
      }
    });

    const totalWithCommission = totalQuantity + (totalQuantity > 0 ? this.commission : 0);
    return +totalWithCommission.toFixed(2);
  }

  get purchaseTickets(): PurchaseTicketQuantityDto[] {
    return this.selectTickets.filter(x => x.quantity > 0);
  }

  startCountdown(): void {
    const initialCountdownMinutes = 10 * 60;

    const savedTime = localStorage.getItem('countdownEndTime');
    if (savedTime) {
      const endTime = new Date(savedTime).getTime();
      const currentTime = new Date().getTime();
      this.countdownMinutes = Math.max(Math.floor((endTime - currentTime) / 1000), 0);

      if (this.countdownMinutes <= 0) {
        clearInterval(this.countdownInterval);
        localStorage.removeItem('countdownEndTime');
        this.cancel();
        return;
      }
    } else {
      this.countdownMinutes = initialCountdownMinutes;
      const endTime = new Date().getTime() + this.countdownMinutes * 1000;
      localStorage.setItem('countdownEndTime', new Date(endTime).toISOString());
    }

    this.updateCountdown();
  }

  updateCountdown(): void {
    clearInterval(this.countdownInterval);
    clearTimeout(this.countdownTimeout);

    this.countdownInterval = setInterval(() => {
      this.countdownMinutes--;
      if (this.countdownMinutes <= 0) {
        clearInterval(this.countdownInterval);
        clearTimeout(this.countdownTimeout);
        localStorage.removeItem('countdownEndTime');
        this.cancel();
      }
    }, 1000);

    const endTime = new Date(localStorage.getItem('countdownEndTime')).getTime();
    const currentTime = new Date().getTime();
    const remainingTime = Math.max(endTime - currentTime, 0);

    this.countdownTimeout = setTimeout(() => {
      clearInterval(this.countdownInterval);
      localStorage.removeItem('countdownEndTime');
      this.cancel();
    }, remainingTime);
  }

  formatTime(seconds: number): string {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  closeModal() {
    this.activeModal.close();
  }

  isBuyButtonEnabled(): boolean {
    return (
      this.termsAccepted &&
      !this.purchaseForm.invalid &&
      !this.purchaseForm.get('step2').invalid &&
      !this.purchaseForm.get('step1').invalid
    );
  }

  onTermsCheckboxChange(checked: boolean) {
    this.termsAccepted = checked;
  }

  handleEnterKey(event: KeyboardEvent) {
    if (this.purchaseForm.get('step2').valid) {
      this.goStep(3);
    }
    event.preventDefault();
  }

  handleVisibilityChange(): void {
    if (document.visibilityState === 'visible') {
      this.startCountdown();
    } else {
      clearInterval(this.countdownInterval);
    }
  }
}
