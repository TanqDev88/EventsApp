import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-validate-already-validated-ticket-dialog',
  templateUrl: './validate-already-validated-ticket-dialog.component.html',
  styleUrls: ['./validate-already-validated-ticket-dialog.component.scss']
})
export class ValidateAlreadyValidatedTicketDialogComponent {
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

