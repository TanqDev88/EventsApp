import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { GetProviderPaymentListDto, ProviderPaymentDto, ProviderPaymentService } from '@proxy/provider-payments';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { CreateEditProviderPaymentsComponent } from './create-edit-provider-payments/create-edit-provider-payments.component';

@Component({
  selector: 'app-provider-payments',
  templateUrl: './provider-payments.component.html',
  providers: [ListService]

})
export class ProviderPaymentsComponent implements OnInit {

  providerPayments = { items: [], totalCount: 0 } as PagedResultDto<ProviderPaymentDto>;
  searchProviderPayments = {} as GetProviderPaymentListDto;

  constructor(
    public readonly list: ListService<GetProviderPaymentListDto>,
    private providerPaymentService: ProviderPaymentService,
    private modalService: NgbModal,
    private confirmation: ConfirmationService)
  { }

  ngOnInit(): void {
    this.viewProviderPayments();
  }

  viewProviderPayments(){
    const listProviderPayment = (query) => this.providerPaymentService.getList({ ...query, ...this.searchProviderPayments });
    this.list.hookToQuery(listProviderPayment).subscribe((response) => {
      this.providerPayments = response;
    });
  }

  createOrEditProviderPayment(id? : number):void{
    const modalRef = this.modalService.open(CreateEditProviderPaymentsComponent);
    modalRef.componentInstance.id = id;

    modalRef.closed.subscribe(res => {
      this.list.get();
    });
  }

  delete(id: number) {
    this.confirmation.warn('::TheProviderPaymentWillBeRemoved', '::AreYouSure')
        .subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
            this.providerPaymentService.delete(id).subscribe(() => this.list.get());
          }
	    });
  }
}
