import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProviderPaymentDto, ProviderPaymentService } from '@proxy/provider-payments';

@Component({
  selector: 'app-create-edit-provider-payments',
  templateUrl: './create-edit-provider-payments.component.html',
})
export class CreateEditProviderPaymentsComponent implements OnInit {

  @Input() id: number;
  fgProviderPayment: FormGroup;
  isSaveButtonDisabled = false;

  constructor(private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private providerPaymentService: ProviderPaymentService) {
      this.createproviderPaymentForm();
  }

  ngOnInit(): void {
    if (this.id) {
      this.providerPaymentService.get(this.id).subscribe(res => {
        this.setProviderPaymentForm(res);
      });
    }  }

  createproviderPaymentForm() {
    this.fgProviderPayment = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });
  }

  setProviderPaymentForm(providerPayment: ProviderPaymentDto) {
    this.fgProviderPayment.patchValue({
      name: providerPayment.name,
      description: providerPayment.description,
    })
  }

  save() {
    if (this.isSaveButtonDisabled) {
      return;
    }
    this.isSaveButtonDisabled = true;
    const providerPayment = this.fgProviderPayment.value as ProviderPaymentDto;
    const saveProviderPayment = this.id
      ? this.providerPaymentService.update(this.id, providerPayment)
      : this.providerPaymentService.create(providerPayment);

    saveProviderPayment.subscribe(
      res => {
        this.activeModal.close(res);
      },
      err => {
        this.isSaveButtonDisabled = false;
      }
    );
  }
}
