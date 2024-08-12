import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventsComponent } from './events/events.component';
import { LayoutComponent } from './layout/layout.component';
import { EventComponent } from './event/event.component';
import { EventResponseComponent } from './event/event-response/event-response.component';
import { EventTicketComponent } from './event-ticket/event-ticket.component';
import { EventCodeGuard } from './event/event-code-guard/event-code-guard';
import { TermsAndConditionsComponent } from './legal-terms-screens/terms-and-conditions/terms-and-conditions.component';
import { PrivacyPolicyComponent } from './legal-terms-screens/privacy-policy/privacy-policy.component';
import { ReturnPolicyComponent } from './legal-terms-screens/return-policy/return-policy.component';
import { ComingSoonComponent } from './coming-soon/coming-soon.component';
import { ticketeraConst } from '../shared/shared.const';

const routes: Routes = [
  {
    path: '',
    component: ComingSoonComponent,
    pathMatch: 'full',
  },
  {
    path: '',
    component: LayoutComponent,
    children: [
      {
        path: 'events',
        component: EventsComponent,
        data: { titleKey: 'Events'},
      },
      {
        path: 'event/:eventcode/purchase/:purchasecode/:ticketid',
        component: EventTicketComponent
      },
      {
        path: 'event/:code',
        component: EventComponent,
        canActivate: [EventCodeGuard],
      },
      {
        path: 'event/:code/response',
        component: EventResponseComponent,
      },
      {
        path: 'terms-and-conditions',
        component: TermsAndConditionsComponent,
        data: { titleKey: 'TermsAndConditions' },
      },
      {
        path: 'privacy-policy',
        component: PrivacyPolicyComponent,
        data: { titleKey: 'PrivacyPolicy' },
      },
      {
        path: 'return-policy',
        component: ReturnPolicyComponent,
        data: { titleKey: 'ReturnPolicy' },
      },
    ],
  },
  {
    path: 'tixgo.mx',
    component: ComingSoonComponent,
  },
  {
    path: '**',
    component: ComingSoonComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LandingRoutingModule {}
