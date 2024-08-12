import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BarcodeFormat } from '@zxing/library';
import { ValidateApprovedTicketDialogComponent } from './validate-approved-ticket-dialog/validate-approved-ticket-dialog.component';
import { ValidateNotApprovedTicketDialogComponent } from './validate-not-approved-ticket-dialog/validate-not-approved-ticket-dialog.component';
import { ValidateAlreadyValidatedTicketDialogComponent } from './validate-already-validated-ticket-dialog/validate-already-validated-ticket-dialog.component';
import { EventService, TicketValidateInputDto } from '@proxy/events';
import { TicketValidateType } from '@proxy/enum';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-events-validate',
  templateUrl: './events-validate.component.html',
  styleUrls: ['./events-validate.component.scss']
})
export class EventsValidateComponent {

  eventId: number;
  loadValidation: boolean = false;
  availableDevices: MediaDeviceInfo[];
  currentDevice: MediaDeviceInfo = null;
  formatsEnabled: BarcodeFormat[] = [ BarcodeFormat.QR_CODE ];
  hasDevices: boolean;
  hasPermission: boolean;
  qrResultString: string;

  constructor(private _dialog: NgbModal,
              private eventService: EventService,
              private route: ActivatedRoute)
  {
    this.eventId = parseInt(this.route.snapshot.paramMap.get('id'));
  }

  clearResult(): void {
    this.qrResultString = null;
  }

  onCamerasFound(devices: MediaDeviceInfo[]): void {
    this.availableDevices = devices;
    this.hasDevices = Boolean(devices && devices.length);
  }

  onCodeResult(resultString: string) {
    if (!this.loadValidation){

      this.loadValidation = true;

      // -- Set code QR
      this.qrResultString = resultString;

      // -- Set code and eventid
      let input = {
        eventId: this.eventId,
        ticketCode: this.qrResultString
      } as TicketValidateInputDto;

      this.eventService.ticketValidateByInput(input).subscribe(result =>{

        // -- Check ok
        if (result.status == TicketValidateType.Success){
          let dialogApro = this._dialog.open(ValidateApprovedTicketDialogComponent,{fullscreen: true });
          dialogApro.componentInstance.data = result;
          dialogApro.closed.subscribe(()=>{
            this.loadValidation = false;
          });
        }

        // -- Check fail
        if (result.status == TicketValidateType.Fail){
          let dialogFail = this._dialog.open(ValidateNotApprovedTicketDialogComponent,{fullscreen: true });
          dialogFail.closed.subscribe(()=>{
            this.loadValidation = false;
          });
        }

        // -- Check already
        if (result.status == TicketValidateType.Already){
          let dialogAlready = this._dialog.open(ValidateAlreadyValidatedTicketDialogComponent,{fullscreen: true });
          dialogAlready.closed.subscribe(()=>{
            this.loadValidation = false;
          });
        }

      });

    }
  }

  onDeviceSelectChange(deviceSelected: MediaDeviceInfo) {
    const device = this.availableDevices.find(x => x.deviceId === deviceSelected.deviceId);
    this.currentDevice = device || null;
  }

  onHasPermission(has: boolean) {
    this.hasPermission = has;
  }

  reload():void{
    location.reload();
  }
}
