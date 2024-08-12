import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TicketCategoryDto, TicketCategoryService } from '@proxy/ticket-categories';


@Component({
  selector: 'app-create-edit-ticket-categories',
  templateUrl: './create-edit-ticket-categories.component.html',
})
export class CreateEditTicketCategoriesComponent implements OnInit{
  @Input() id: number;
  fgTicketCategory: FormGroup;
  isSaveButtonDisabled = false;

  constructor(private fb: FormBuilder,
              public activeModal: NgbActiveModal,
              private ticketCategoryService: TicketCategoryService) {
    this.createTicketCategoryForm();
  }

  ngOnInit(): void {
    if (this.id) {
      this.ticketCategoryService.get(this.id).subscribe(res => {
        this.setTicketCategoryForm(res);
      });
    }  }

  createTicketCategoryForm() {
    this.fgTicketCategory = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });
  }

  setTicketCategoryForm(category: TicketCategoryDto) {
    this.fgTicketCategory.patchValue({
      name: category.name,
      description: category.description,
    })
  }

  save() {
    if (this.isSaveButtonDisabled) {
      return;
    }
    this.isSaveButtonDisabled = true;

    const ticketCategory = this.fgTicketCategory.value as TicketCategoryDto;
    const saveTicketCategory = this.id
      ? this.ticketCategoryService.update(this.id, ticketCategory)
      : this.ticketCategoryService.create(ticketCategory);

    saveTicketCategory.subscribe(
      res => {
        this.activeModal.close(res);
      },
      err => {
        this.isSaveButtonDisabled = false;
      }
    );
  }

}
