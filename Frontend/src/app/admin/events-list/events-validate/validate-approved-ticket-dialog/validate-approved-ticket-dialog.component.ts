import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TicketValidateOutDto } from '@proxy/events';

@Component({
  selector: 'app-validate-approved-ticket-dialog',
  templateUrl: './validate-approved-ticket-dialog.component.html',
  styleUrls: ['./validate-approved-ticket-dialog.component.scss']
})
export class ValidateApprovedTicketDialogComponent {
  @Input() public data: TicketValidateOutDto;

  constructor(private activeModal: NgbActiveModal) {
    setTimeout(() => {
      if (this.activeModal) {
        this.activeModal.close();
      }
    }, 5000);
  }

  closeModalAfterDelay() {
    if (this.activeModal) {
      this.activeModal.close();
    }
  }
}
