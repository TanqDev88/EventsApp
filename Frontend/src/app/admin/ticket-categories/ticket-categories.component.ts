import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { GetTicketCategoryListDto, TicketCategoryDto, TicketCategoryService } from '@proxy/ticket-categories';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateEditTicketCategoriesComponent } from './create-edit-ticket-categories/create-edit-ticket-categories.component';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { LayoutService } from '@services/layout.service';

@Component({
  selector: 'app-ticket-categories',
  templateUrl: './ticket-categories.component.html',
  providers: [ListService]
})
export class TicketCategoriesComponent implements OnInit{
  categories = { items: [], totalCount: 0 } as PagedResultDto<TicketCategoryDto>;
  searchCategories = {} as GetTicketCategoryListDto;

  constructor(
    public readonly list: ListService<GetTicketCategoryListDto>,
    private ticketCateogryService: TicketCategoryService,
    private modalService: NgbModal,
    private confirmation: ConfirmationService,
    private layoutService: LayoutService
    )
    {
      this.layoutService.validateLogin();
    }


  ngOnInit(): void {
    this.viewCategories();
  }

  viewCategories(){
    const listCategories = (query) => this.ticketCateogryService.getList({ ...query, ...this.searchCategories });
    this.list.hookToQuery(listCategories).subscribe((response) => {
      this.categories = response;
    });
  }

  createOrEditCategory(id? : number):void{
    const modalRef = this.modalService.open(CreateEditTicketCategoriesComponent);
    modalRef.componentInstance.id = id;

    modalRef.closed.subscribe(res => {
      this.list.get();
    });
  }

  delete(id: number) {
    this.confirmation.warn('::TheTicketCategoryWillBeRemoved', '::AreYouSure')
        .subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
            this.ticketCateogryService.delete(id).subscribe(() => this.list.get());
          }
	    });
  }

}
