import { NgModule } from '@angular/core';
import { EventComponent } from './event/event.component';
import { EventsComponent } from './events/events.component';
import { LayoutComponent } from './layout/layout.component';
import { LandingRoutingModule } from './landing-routing.module';
import { HeaderComponent } from './layout/header/header.component';
import { FooterComponent } from './layout/footer/footer.component';
import { SharedModule } from '../shared/shared.module';
import { EventTicketModalComponent } from './event/event-ticket-modal/event-ticket-modal.component';
import { EventResponseComponent } from './event/event-response/event-response.component';
import { EventResponseApprovedComponent } from './event/event-response/event-response-approved/event-response-approved.component';
import { EventResponsePendingComponent } from './event/event-response/event-response-pending/event-response-pending.component';
import { EventResponseErrorComponent } from './event/event-response/event-response-error/event-response-error.component';
import { EventTicketComponent } from './event-ticket/event-ticket.component';
import { TermsAndConditionsComponent } from './legal-terms-screens/terms-and-conditions/terms-and-conditions.component';
import { PrivacyPolicyComponent } from './legal-terms-screens/privacy-policy/privacy-policy.component';
import { ReturnPolicyComponent } from './legal-terms-screens/return-policy/return-policy.component';
import { ComingSoonComponent } from './coming-soon/coming-soon.component';

@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    LayoutComponent,
    EventsComponent,
    EventComponent,
    EventTicketModalComponent,
    EventResponseComponent,
    EventResponseApprovedComponent,
    EventResponsePendingComponent,
    EventResponseErrorComponent,
    EventTicketComponent,
    TermsAndConditionsComponent,
    PrivacyPolicyComponent,
    ReturnPolicyComponent,
    ComingSoonComponent
  ],
  imports: [
    LandingRoutingModule,
    SharedModule
  ]
})
export class LandingModule { }
