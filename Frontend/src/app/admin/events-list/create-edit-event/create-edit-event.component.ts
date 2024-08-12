import { ListService, LocalizationService, PagedResultDto, mapEnumToOptions } from '@abp/ng.core';
import { ChangeDetectorRef, Component, HostListener, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbDate, NgbModal, NgbNavChangeEvent, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { EventInputDto, UserOutDto } from '@proxy/events';
import { GetProviderPaymentListDto,ProviderPaymentDto,ProviderPaymentService,} from '@proxy/provider-payments';
import { EventService } from '@proxy/events/event.service';
import {GetTicketCategoryListDto,TicketCategoryDto,TicketCategoryService,} from '@proxy/ticket-categories';
import { CreateEditTicketCategoriesComponent } from '../../ticket-categories/create-edit-ticket-categories/create-edit-ticket-categories.component';
import { TicketSectorService } from '@proxy/ticket-sectors/ticket-sector.service';
import { GetTicketSectorListDto, TicketSectorDto } from '@proxy/ticket-sectors';
import { CreateEditTicketSectorsComponent } from '../../ticket-sectors/create-edit-ticket-sectors/create-edit-ticket-sectors.component';
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { StorageService } from '@proxy/storages/storage.service';
import { Observable, Subscription, debounceTime, distinctUntilChanged, map } from 'rxjs';
import { TypeUserEvent } from '@proxy/enum/type-user-event.enum';
import { CreateUserComponent } from './create-user/create-user/create-user.component';
import { IdentityUserDto } from '@abp/ng.identity/proxy';
import { ticketeraConst } from 'src/app/shared/shared.const';
import { EventStatus } from '@proxy/enum';

@Component({
  selector: 'app-create-edit-event',
  templateUrl: './create-edit-event.component.html',
  styleUrls: ['./create-edit-event.component.scss'],
  providers: [ListService],
})
export class CreateEditEventComponent implements OnInit {
  userDto : IdentityUserDto
  typeUserEventOptions = mapEnumToOptions(TypeUserEvent);
  eventId: number;
  active;
  disabled = true;
  providerPayments = { items: [], totalCount: 0 } as PagedResultDto<ProviderPaymentDto>;
  searchProviderPayments = {} as GetProviderPaymentListDto;
  public tickets: any[] = [];
  public events: any;
  public function: any[] = [];
  selectedCategoryId: number;
  selectedSectorId: number;
  selectedCategory: string | undefined;
  selectedSector: string | undefined;
  fgPrices: FormGroup;
  searchCategories = {} as GetTicketCategoryListDto;
  categories = { items: [], totalCount: 0 } as PagedResultDto<TicketCategoryDto>;
  sectors = { items: [], totalCount: 0 } as PagedResultDto<TicketSectorDto>;
  searchSectors = {} as GetTicketSectorListDto;
  errorMessageDate: boolean = false;
  errorMessageDateHour: boolean = false;
  minStartDate: NgbDate | null;
  logoSrcLocal: string = '/assets/images/logo/logonike.png';
  fileUploadedGallery: boolean = false;
  fileUploadedDetail: boolean = false;
  fileUploadedLogo: boolean = false;
  formDataGallery: FormData;
  formDataDetail: FormData;
  formDataLogo: FormData;
  confirmNavigation = false;
  beforeUnload = true;
  private navigationSubscription: Subscription;
  showAlert: boolean = true;
  private finalizarClicked: boolean = false;
  userInput: FormControl = new FormControl();
  filteresUserOptions: (UserOutDto | { addNew: boolean })[] = [];
  selectedUsers: any[] = [];
  selectedType: string | undefined;
  userTypesDictionary: { [key: string]: number } = {};
  userTypes: { [key: string]: string } = {};
  minDate: NgbDate;

  public createEvent: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(5)]],
    description: ['', [Validators.required, Validators.minLength(5)]],
    eventType: [1],
    code: [''],
    bgColor: [''],
    photoGallery: [null],
    photoDetail: [null],
    photoLogo: [0, Validators.required],
    idPhotoLogo: [null],
    isMain: [false],
    isActive: [true],
    eventStatus: [EventStatus.Available],
    place: ['', [Validators.required, Validators.minLength(5)]],
    eventDatesInput: this.fb.array([]),
    prices: this.fb.array([]),
    idProviderPayment: [[],Validators.required],
    saleForPerson: [''],
    validators: this.fb.group({})
  });

  public pricesEvent: FormGroup = this.fb.group({
    count: [null, [Validators.required, Validators.min(1), Validators.pattern(/^[0-9]+$/)]],
    price: [null, [Validators.required, Validators.min(1), Validators.pattern(/^[0-9]+$/)]],
    ticketCategoryId: [],
    ticketCategoryName: ['', Validators.required],
    ticketSectorId: [],
    ticketSectorName: ['', Validators.required],
  });

  public datesEvents: FormGroup = this.fb.group({
    startDate: [],
    endDate: [],
  });

  public createPartThree: FormGroup;

  public createPartTFour: FormGroup = this.fb.group({
    paymentMethods: [false, Validators.requiredTrue],
  });

  public createPartTFive: FormGroup = this.fb.group({
    customizeColor: [false],
    chosenColor: [ticketeraConst.theme.dark.bgColor],
    photoLogo: [null],
      saleForPerson: [0, [Validators.required, Validators.min(0)]],
  });

  constructor(
    private providerPaymentService: ProviderPaymentService,
    private eventService: EventService,
    public readonly list: ListService<GetProviderPaymentListDto>,
    private fb: FormBuilder,
    private ticketCategoryService: TicketCategoryService,
    private modalService: NgbModal,
    private ticketSectorService: TicketSectorService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private route: ActivatedRoute,
    private storageService: StorageService,
    private localization: LocalizationService,
  ) {
    const today = new Date();
    let day = today.getDate() + 1;
    let month = today.getMonth() + 1; // getMonth() is zero-based
    let year = today.getFullYear();

    const daysInMonth = new Date(year, month, 0).getDate();

    if (day > daysInMonth) {
      day = 1;
      month += 1;
      if (month > 12) {
        month = 1;
        year += 1;
      }
    }

    this.minStartDate = new NgbDate(year, month, day);

    this.createPartThree = this.fb.group({
      startDate: [null, [Validators.required]],
      startHour: ['', [Validators.required]],
      endDate: [null],
      endHour: [null],
    });

    this.navigationSubscription = this.router.events.subscribe(event => {
      if (event instanceof NavigationStart && this.showAlert && !this.finalizarClicked) {
        const confirmationMessage = '¿Seguro que deseas abandonar la página?';
        if (!confirm(confirmationMessage)) {
          this.showAlert = false; // Desactivar la alerta temporalmente
          setTimeout(() => this.showAlert = true, 1000); // Reactivar la alerta después de 1 segundo
          this.router.navigateByUrl(this.router.url); // Restaurar la URL actual
        }
      }
    });
  }

  ngOnInit(): void {
    this.viewProviderPayments();
    this.viewCategories();
    this.viewSectors();
    this.route.params.subscribe(params => {
      this.eventId = +params['id'];
      if (this.eventId) {
        this.loadEventData(this.eventId);
      }
    });

    this.userInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(value => {
        this.search(value);
      });
  }

  search(text: string): void {
    let key = text ? text : "";
    this.eventService.getUsersEventByInput(key).subscribe(items => {
      this.filteresUserOptions = [...items, { addNew: true }];
    });
  }

  filteredUser = (text$: Observable<string>): Observable<(UserOutDto | { addNew: boolean })[]> => {
    return text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      map(term => term.length < 2 ? []
        : this.filteresUserOptions.filter(v => {
            if ('addNew' in v) return true;
            return v.email
          }))
    );
  };

  resultFormatter = (result: UserOutDto | { addNew: boolean }) => {
    if ('addNew' in result) {
        var addUser;
        this.localization.get('::AddUser').subscribe(result =>{
        addUser = result;
      }
      );
      return addUser;
    }else{
      let displayName = '';
      if (result.email) {
        displayName += result.email
      }
      return displayName;
    }
  };

  inputFormatter = (result: UserOutDto | { addNew: boolean }) => {
    if (typeof result === 'object' && 'addNew' in result) {
      return '';
    } else {
      let displayName = '';
      if (result.email) {
        displayName += result.email;
      }
      return displayName;
    }
  };

  openUserModal() {
    const modalRef = this.modalService.open(CreateUserComponent, { size: 'lg' });
    modalRef.result.then((newUser) => {
      if (newUser) {
        console.log('Nuevo usuario:', newUser);
      }
    }).catch((error) => {
      console.log('Modal dismissed', error);
    });
  }

  onSelect(event: any): void {
    const item = event.item;
    if ('addNew' in item) {
      this.openUserModal();
    } else {
      const selectedUser = event.item;
      const userAlreadyAdded = this.selectedUsers.some(user => user.id === selectedUser.id);
      if (!userAlreadyAdded) {
        this.selectedUsers.push(selectedUser);
      }
      this.userInput.setValue("");
    }
  }

  ngOnDestroy() {
    if (this.navigationSubscription) {
      this.navigationSubscription.unsubscribe();
    }
  }

  toggleDisabled() {
    this.disabled = !this.disabled;
    if (this.disabled) {
      this.active = 1;
    }
  }

  selectCategory(item: string, id: number): void {
    this.selectedCategoryId = id;
    this.selectedCategory = item;
    this.pricesEvent.get('ticketCategoryName').setValue(this.selectedCategory);
    this.pricesEvent.get('ticketCategoryId').setValue(this.selectedCategoryId);
  }

  selectSector(item: string, id: number): void {
    this.selectedSectorId = id;
    this.selectedSector = item;
    this.pricesEvent.get('ticketSectorName').setValue(this.selectedSector);
    this.pricesEvent.get('ticketSectorId').setValue(this.selectedSectorId);
  }

  createOrEditCategory(): void {
    const modalRef = this.modalService.open(CreateEditTicketCategoriesComponent);
    modalRef.closed.subscribe(res => {
      this.list.get();
    });
  }

  createOrEditSector(): void {
    const modalRef = this.modalService.open(CreateEditTicketSectorsComponent);
    modalRef.closed.subscribe(res => {
      this.list.get();
    });
  }

  viewCategories() {
    this.searchCategories.maxResultCount = 1000;
    const listCategories = query =>
      this.ticketCategoryService.getList({ ...query, ...this.searchCategories });
    this.list.hookToQuery(listCategories).subscribe(response => {
      this.categories = response;
    });
  }

  viewSectors() {
    this.searchSectors.maxResultCount = 1000;
    const listSectors = query =>
      this.ticketSectorService.getList({ ...query, ...this.searchSectors });
    this.list.hookToQuery(listSectors).subscribe(response => {
      this.sectors = response;
    });
  }

  public saveFormPartTwo(): void {
    if (!this.createEvent.get('name').invalid) {
      const pricesArray = this.createEvent.get('prices') as FormArray;
      const newPriceValues = this.pricesEvent.value;
      const newPriceGroup = this.fb.group(newPriceValues);
      pricesArray.push(newPriceGroup);
      this.tickets.push(this.pricesEvent.value);
      this.pricesEvent.reset();
    }
  }

  public saveFormPartThree(): void {
    this.errorMessageDate = false;
    this.errorMessageDateHour = false;

    const startDateValue = this.createPartThree.get('startDate')?.value;
    const startHourValue = this.createPartThree.get('startHour')?.value;
    const startDateTime = new Date(
      startDateValue.year,
      startDateValue.month - 1,
      startDateValue.day,
      startHourValue.hour,
      startHourValue.minute
    );

    const year = startDateTime.getFullYear();
    const month = (startDateTime.getMonth() + 1).toString().padStart(2, '0');
    const day = startDateTime.getDate().toString().padStart(2, '0');
    const hours = startDateTime.getHours().toString().padStart(2, '0');
    const minutes = startDateTime.getMinutes().toString().padStart(2, '0');

    const formattedDateStart = `${year}-${month}-${day}T${hours}:${minutes}`;

    let endDateTime: Date;
    let formattedDateEnd;

    const endDateControl = this.createPartThree.get('endDate');
    const endHourControl = this.createPartThree.get('endHour');

    if (endDateControl?.value && endHourControl?.value) {
      const endDateValue = endDateControl.value;
      const endHourValue = endHourControl.value;

      endDateTime = new Date(
        endDateValue.year,
        endDateValue.month - 1,
        endDateValue.day,
        endHourValue.hour,
        endHourValue.minute
      );

      const yearEnd = endDateTime.getFullYear();
      const monthEnd = (endDateTime.getMonth() + 1).toString().padStart(2, '0');
      const dayEnd = endDateTime.getDate().toString().padStart(2, '0');
      const hoursEnd = endDateTime.getHours().toString().padStart(2, '0');
      const minutesEnd = endDateTime.getMinutes().toString().padStart(2, '0');

      formattedDateEnd = `${yearEnd}-${monthEnd}-${dayEnd}T${hoursEnd}:${minutesEnd}`;
    }

    if (endDateTime <= startDateTime) {
      this.errorMessageDateHour = true;
      return;
    }

    if (!this.isPastDate(startDateValue)) {
      const datesArray = this.createEvent.get('eventDatesInput') as FormArray;
      const formattedStartDate = startDateTime.toLocaleString('es-ES', {
        day: 'numeric',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      });
      const newDatesGroup = this.fb.group({
        startDate: formattedDateStart,
        endDate: formattedDateEnd,
      });

      datesArray.push(newDatesGroup);
      this.function.push(newDatesGroup.value);

      this.createPartThree.reset();
    } else {
      console.log(
        'La fecha de inicio no es válida o es una fecha pasada. No se puede agregar la función.'
      );
    }
  }

  isPastDate(date: NgbDateStruct): boolean {
    const currentDate = new Date();
    const selectedDate = new Date(date.year, date.month - 1, date.day);
    return selectedDate < currentDate;
  }

  public DeleteEventTable(index: number): void {
    if (index >= 0 && index < this.tickets.length) {
      this.tickets.splice(index, 1);
      const pricesArray = this.createEvent.get('prices') as FormArray;
      if (index < pricesArray.length) {
        pricesArray.removeAt(index);
      }
    }
    this.cdr.detectChanges();
  }

  public DeleteFunctionTable(index: number): void {
    if (index >= 0 && index < this.function.length) {
      this.function.splice(index, 1);
      const datesArray = this.createEvent.get('eventDatesInput') as FormArray;
      datesArray.removeAt(index);
    }
  }

  viewProviderPayments() {
    const listProviderPayment = query =>
      this.providerPaymentService.getList({ ...query, ...this.searchProviderPayments });
    this.list.hookToQuery(listProviderPayment).subscribe(response => {
      this.providerPayments = response;
    });
  }

  async onSave(): Promise<void> {
    await this.loadImages();
    const customizeColor = this.createPartTFive.get('customizeColor').value;

    if (customizeColor) {
      const chosenColorValue = this.createPartTFive.get('chosenColor').value;
      this.createEvent.get('bgColor').setValue(chosenColorValue);
    } else {
      this.createEvent.get('bgColor').setValue(ticketeraConst.theme.dark.bgColor);

      if (this.fileUploadedLogo) {
        this.storageService
          .removeFileByFileId(this.createEvent.get('photoLogo').value)
          .subscribe(() => {});
      }

      if(this.createEvent.get('idPhotoLogo').value !== null){
      this.storageService
        .removeFileByFileId(this.createEvent.get('idPhotoLogo').value)
        .subscribe(() => {});
      }

      this.createEvent.get('photoLogo').setValue(0);

    }
    if (this.createPartTFive.get('saleForPerson').valid) {
      this.createEvent.patchValue({
        saleForPerson: this.createPartTFive.get('saleForPerson').value,
      });
    }

    let event = this.createEvent.value as EventInputDto;
    const eventName = event.name.toLowerCase();
    const eventCode = `${eventName}`;
    event.code = eventCode;
    const saveEvent = this.eventId
      ? this.eventService.update(this.eventId, event)
      : this.eventService.create(event);
    saveEvent.subscribe(() => {
      this.router.navigate(['admin/events']);
    });
  }

  public elementsInTablePartTwo(): boolean {
    return this.tickets.length == 0;
  }

  public elementsInTablePartThree(): boolean {
    return this.function.length == 0;
  }

  onFileSelected(event: any, type: string): void {
  const file = event.target.files[0];
  const maxSize = 20 * 1024 * 1024;

  if (file) {
    if (file.size > maxSize) {
      if (type === 'photoGallery') {
        this.createEvent.get('photoGallery').setErrors({ size: true });
      } else if (type === 'photoDetail') {
        this.createEvent.get('photoDetail').setErrors({ size: true });
      } else if (type === 'photoLogo') {
        this.createEvent.get('photoLogo').setErrors({ size: true });
      }
    } else {
      const fileReader = new FileReader();
      fileReader.readAsDataURL(file);
      fileReader.onload = (event: any) => {
        if (type === 'photoGallery') {
          this.createEvent.get('photoGallery').setErrors(null);
          this.formDataGallery = new FormData();
          this.formDataGallery.append('file', file);
          this.fileUploadedGallery = true;
        } else if (type === 'photoDetail') {
          this.createEvent.get('photoDetail').setErrors(null);
          this.formDataDetail = new FormData();
          this.formDataDetail.append('file', file);
          this.fileUploadedDetail = true;
        } else if (type === 'photoLogo') {
          this.createEvent.get('photoLogo').setErrors(null);
          this.formDataLogo = new FormData();
          this.formDataLogo.append('file', file);
          this.fileUploadedLogo = true;
        }
      };
    }
  }
}


  loadEventData(eventId: number): void {
    this.eventService.get(eventId).subscribe(eventData => {
      eventData.eventDates.forEach(eventDate => {
        eventDate.startDate = this.getFormattedDate(new Date(eventDate.startDate));
      });
      const functionArray = eventData.eventDates;
      const pricesArray = eventData.prices;

      if (Array.isArray(pricesArray)) {
        this.tickets = pricesArray;
        const pricesFormArray = this.createEvent.get('prices') as FormArray;
        pricesFormArray.clear();
        pricesArray.forEach(priceData => {
          const priceFormGroup = this.fb.group(priceData);
          pricesFormArray.push(priceFormGroup);
        });
      }

      if (Array.isArray(functionArray)) {
        this.function = functionArray;
        const datesFormArray = this.createEvent.get('eventDatesInput') as FormArray;
        datesFormArray.clear();
        functionArray.forEach(priceData => {
          const datesFormGroup = this.fb.group(priceData);
          datesFormArray.push(datesFormGroup);
        });
      }

      if (Array.isArray(eventData.userEvents)) {
        this.selectedUsers = eventData.userEvents.map(userEvent => ({
            email: userEvent.identityUserEmail,
            id: userEvent.identityUserId,
            type: this.getTypeUserTextValue(userEvent.typeUserEvent)
        }));

        const validatorsFormGroup = this.createEvent.get('validators') as FormGroup;
        validatorsFormGroup.reset();
        eventData.userEvents.forEach(userEvent => {
            validatorsFormGroup.addControl(userEvent.identityUserId, new FormControl(this.getTypeUserTextValue(userEvent.typeUserEvent)));
            this.selectedTypeUser(userEvent.identityUserId, this.getTypeUserTextValue(userEvent.typeUserEvent));
        });
      }

      this.createEvent.patchValue(eventData);

      if(eventData.photoLogo == null)
        {
          this.createEvent.get('photoLogo').setValue(0);
        }

      this.createPartTFive.patchValue({
        chosenColor: eventData.bgColor || ticketeraConst.theme.dark.bgColor,
        customizeColor: eventData.bgColor !== ticketeraConst.theme.dark.bgColor || eventData.photoLogo !== null,
        uploadLogo: eventData.photoLogo !== null,
        saleForPerson:eventData.saleForPerson,
      });
    });
  }

  isString(value: any): boolean {
    return typeof value === 'string';
  }

  isFinishButtonEnabled(): boolean {
    const customizeColor = this.createPartTFive.get('customizeColor').value;
    const chosenColorValid = this.createPartTFive.get('chosenColor').valid;
    const photoLogoValid = this.fileUploadedLogo === true;
    const uploadLogoValid = photoLogoValid || this.createEvent.get('photoLogo').value !== 0;
    const saleForPersonValid = this.createPartTFive.get('saleForPerson').valid;
    return saleForPersonValid && (!customizeColor || (customizeColor && chosenColorValid && uploadLogoValid));
  }

  isPaymentMethodSelected(id: number): boolean {
    const selectedPaymentMethods = this.createEvent.get('idProviderPayment').value;
    return selectedPaymentMethods.includes(id);
  }

  updateSelectedPaymentMethods(id: number, isChecked: boolean): void {
    const selectedPaymentMethods: number[] = this.createEvent.get('idProviderPayment').value;

    if (isChecked) {
        if (!selectedPaymentMethods.includes(Number(id))) {
            selectedPaymentMethods.push(Number(id));
        }
    } else {
        const index = selectedPaymentMethods.indexOf(Number(id));
        if (index !== -1) {
            selectedPaymentMethods.splice(index, 1);
        }
    }
    this.createEvent.get('idProviderPayment').patchValue(selectedPaymentMethods);
  }

  async loadImages(): Promise<void> {
    const observables = [];

    if (this.fileUploadedGallery) {
        observables.push(
            new Promise<void>((resolve) => {
                this.storageService.postImageByFile(this.formDataGallery).subscribe(result => {
                    this.createEvent.get('photoGallery').setValue(result);
                    resolve();
                });
            })
        );
    } else {
        this.createEvent.patchValue({
            photoGallery: 0,
        });
    }

    if (this.fileUploadedDetail) {
        observables.push(
            new Promise<void>((resolve) => {
                this.storageService.postImageByFile(this.formDataDetail).subscribe(result => {
                    this.createEvent.get('photoDetail').setValue(result);
                    resolve();
                });
            })
        );
    } else {
        this.createEvent.patchValue({
            photoDetail: 0,
        });
    }

    if (this.fileUploadedLogo) {
        observables.push(
            new Promise<void>((resolve) => {
                this.storageService.postImageByFile(this.formDataLogo).subscribe(result => {
                    this.createEvent.get('photoLogo').setValue(result);
                    resolve();
                });
            })
        );
    } else {
        this.createEvent.patchValue({
            photoLogo: 0,
        });
    }

    await Promise.all(observables);
  }

  @HostListener("window:beforeunload", ["$event"])
  onWindowClose(event: any): void {
    if(this.beforeUnload){
      event.preventDefault();
      event.returnValue = false;
    }
  }

  onFinalizarClick() {
    if (!this.finalizarClicked) {
      this.finalizarClicked = true;
      this.onSave();
    }
  }

  selectedTypeUser(guid: string, type: string): void {
    this.userTypes[guid] = type;
    const validatorsFormGroup = this.createEvent.get('validators') as FormGroup;
    if (validatorsFormGroup.controls[guid]) {
      validatorsFormGroup.controls[guid].setValue(type);
    } else {
      validatorsFormGroup.addControl(guid, new FormControl(type));
    }
  }

  getTypeUserText(guid: string): string {
    const typeId = this.userTypes[guid];
    const typeOption = this.typeUserEventOptions.find(option => {
      return option.key === typeId;
    });
    if (typeOption) {
      if (typeOption.key === 'Readonly') {
          return '::Readonly';
      } else if (typeOption.key === 'Validator') {
          return '::Validator';
      } else {
          return typeOption.key;
      }
    } else {
      return '::Options';
    }
  }

  deleteTypeTable(index: number): void {
    const userId = this.selectedUsers[index].id;
    this.selectedUsers.splice(index, 1);
    delete this.userTypes[userId];
    (this.createEvent.get('validators') as FormGroup).removeControl(userId);
  }

  getTranslatedOption(option: string): string {
    if (option === 'Readonly' || option === 'Validator') {
      var optionTranslate = '::' + option + '';
      return optionTranslate;
    } else {
        return option
    }
  }

  getTypeUserTextValue(type: number): string {
    return TypeUserEvent[type];
  }

  isFinalizeDisabled(): boolean {
    return this.selectedUsers.some(user => !this.userTypes[user.id]);
  }

  getFormattedDate(date: Date): string {
    return date.toISOString().substring(0, date.toISOString().length - 1);
  }

  getSaleForPersonLabel(): string {
    const saleForPersonValue = this.createPartTFive.get('saleForPerson').value;
    return saleForPersonValue === 0 ? 'Limitless' : saleForPersonValue.toString();
  }

  onStartDateChange(date: NgbDate) {
    this.minDate = date;
  }
}
