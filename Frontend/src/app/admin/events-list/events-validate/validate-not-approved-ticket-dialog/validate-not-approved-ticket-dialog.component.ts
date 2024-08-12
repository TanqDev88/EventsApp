import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-validate-not-approved-ticket-dialog',
  templateUrl: './validate-not-approved-ticket-dialog.component.html',
  styleUrls: ['./validate-not-approved-ticket-dialog.component.scss'],
})
export class ValidateNotApprovedTicketDialogComponent {

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
