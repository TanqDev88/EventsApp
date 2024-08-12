import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TicketSectorDto, TicketSectorService } from '@proxy/ticket-sectors';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-create-edit-ticket-sectors',
  templateUrl: './create-edit-ticket-sectors.component.html',
})
export class CreateEditTicketSectorsComponent implements OnInit {
  @Input() id: number;
  fgTicketSector: FormGroup;
  saveClicked: boolean = false;

  constructor(private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private ticketSectorService: TicketSectorService) {
      this.createTicketSectorForm();
  }

  ngOnInit(): void {
    if (this.id) {
      this.ticketSectorService.get(this.id).subscribe(res => {
        this.setTicketSectorForm(res);
      });
    }
  }

  createTicketSectorForm() {
    this.fgTicketSector = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });
  }

  setTicketSectorForm(Sector: TicketSectorDto) {
    this.fgTicketSector.patchValue({
      name: Sector.name,
      description: Sector.description,
    });
  }

  onSave() {
    if (this.saveClicked) {
      return;
    }
    this.saveClicked = true;
    const ticketSector = this.fgTicketSector.value as TicketSectorDto;
    const saveTicketSector = this.id
      ? this.ticketSectorService.update(this.id, ticketSector)
      : this.ticketSectorService.create(ticketSector);

    saveTicketSector.subscribe(res => {
      this.activeModal.close(res);
    }, err => {
      this.saveClicked = false;
    });
  }
}
