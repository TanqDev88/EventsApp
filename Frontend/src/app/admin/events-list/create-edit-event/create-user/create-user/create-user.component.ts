import { IdentityUserCreateDto, IdentityUserService } from '@abp/ng.identity/proxy';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { EventService } from '@proxy/events/event.service';

@Component({
  selector: 'app-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.scss']
})
export class CreateUserComponent {
  public userForm: FormGroup;

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder, private eventService: EventService, private identityService: IdentityUserService) {
    this.userForm = this.fb.group({
      name: [''],
      surname: [''],
      email: ['', [Validators.required, Validators.email]],
      userName: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(1), Validators.pattern(/^(?=.*\d)(?=.*[A-Z])(?=.*\W).*$/)]],
      phone: ['', [Validators.pattern('^\\+?\\d{10,15}$')]],
      isActive: [true],
    });
  }

  ngOnInit(): void {}

  save() {
    if (this.userForm.get('userName').valid || this.userForm.get('password').valid || this.userForm.get('email').valid) {
      let user = this.userForm.value as IdentityUserCreateDto;
      this.identityService.create(user).subscribe(result =>{
        console.log(result);
      })
      this.activeModal.close(this.userForm.value);
    }
  }

  close() {
    this.activeModal.dismiss();
  }
}
