import { ListService, PagedResultDto } from '@abp/ng.core';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { GetTicketSectorListDto, TicketSectorDto, TicketSectorService } from '@proxy/ticket-sectors';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateEditTicketSectorsComponent } from './create-edit-ticket-sectors/create-edit-ticket-sectors.component';
import { LayoutService } from '@services/layout.service';


@Component({
  selector: 'app-ticket-sectors',
  templateUrl: './ticket-sectors.component.html',
  providers: [ListService]
})
export class TicketSectorsComponent implements OnInit{
  sectors = { items: [], totalCount: 0 } as PagedResultDto<TicketSectorDto>;
  searchSectors = {} as GetTicketSectorListDto;

  constructor(
    public readonly list: ListService<GetTicketSectorListDto>,
    private ticketSectorService: TicketSectorService,
    private modalService: NgbModal,
    private confirmation: ConfirmationService,
    private layoutService: LayoutService
    )
    {
      this.layoutService.validateLogin();
    }

  ngOnInit(): void {
    this.viewSectors();
  }

  viewSectors(){
    const listSectors = (query) => this.ticketSectorService.getList({ ...query, ...this.searchSectors });
    this.list.hookToQuery(listSectors).subscribe((response) => {
      this.sectors = response;
    });
  }

  createOrEditSector(id? : number):void{
    const modalRef = this.modalService.open(CreateEditTicketSectorsComponent);
    modalRef.componentInstance.id = id;

    modalRef.closed.subscribe(res => {
      this.list.get();
    });
  }

  delete(id: number) {
    let sectorName = "";
    this.ticketSectorService.get(id).subscribe(res =>{sectorName = res.name})
    this.confirmation.warn('::TheTicketSectorWillBeRemoved', '::AreYouSure')
        .subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
            this.ticketSectorService.delete(id).subscribe(() => this.list.get());
          }
	    });
  }

}
