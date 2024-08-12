import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { AdminRoutingModule } from './admin-routing.module';
import { HomeComponent } from './home/home.component';
import { CommonModule } from '@angular/common';
import { TicketCategoriesComponent } from './ticket-categories/ticket-categories.component';
import { TicketSectorsComponent } from './ticket-sectors/ticket-sectors.component';
import { ProviderPaymentsComponent } from './provider-payments/provider-payments.component';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from './layout-admin/sidebar/sidebar.component';
import { LayoutAdminComponent } from './layout-admin/layout-admin.component';
import { CreateEditTicketCategoriesComponent } from './ticket-categories/create-edit-ticket-categories/create-edit-ticket-categories.component';
import { CreateEditTicketSectorsComponent } from './ticket-sectors/create-edit-ticket-sectors/create-edit-ticket-sectors.component';
import { CreateEditProviderPaymentsComponent } from './provider-payments/create-edit-provider-payments/create-edit-provider-payments.component';
import { CreateEditEventComponent } from './events-list/create-edit-event/create-edit-event.component';
import { HeaderAdminComponent } from './layout-admin/header/header-admin/header-admin.component';
import { EventsValidateComponent } from './events-list/events-validate/events-validate.component';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { ValidateApprovedTicketDialogComponent } from './events-list/events-validate/validate-approved-ticket-dialog/validate-approved-ticket-dialog.component';
import { ValidateNotApprovedTicketDialogComponent } from './events-list/events-validate/validate-not-approved-ticket-dialog/validate-not-approved-ticket-dialog.component';
import { ValidateAlreadyValidatedTicketDialogComponent } from './events-list/events-validate/validate-already-validated-ticket-dialog/validate-already-validated-ticket-dialog.component';
import { CreateUserComponent } from './events-list/create-edit-event/create-user/create-user/create-user.component';
import { EventSalesComponent } from './events-list/event-sales/event-sales.component';
import { EventSalesTotalTicketsComponent } from './events-list/event-sales/event-sales-total-tickets/event-sales-total-tickets.component';
import { EventSalesTotalEntriesComponent } from './events-list/event-sales/event-sales-total-entries/event-sales-total-entries.component';
import { EventSalesTotalAvailableComponent } from './events-list/event-sales/event-sales-total-available/event-sales-total-available.component';
import { EventSalesTotalSalesComponent } from './events-list/event-sales/event-sales-total-sales/event-sales-total-sales.component';
import { EventSalesDaysToSaleComponent } from './events-list/event-sales/event-sales-days-to-sale/event-sales-days-to-sale.component';
import { EventSalesDaysToRemainingComponent } from './events-list/event-sales/event-sales-days-to-remaining/event-sales-days-to-remaining.component';
import { EventSalesGraphTicketSalesComponent } from './events-list/event-sales/event-sales-graph-ticket-sales/event-sales-graph-ticket-sales.component';
import { NgChartsModule } from 'ng2-charts';
import { CountUpModule } from 'ngx-countup';
import { EventSalesGraphTicketSaleComponent } from './events-list/event-sales/event-sales-graph-ticket-sale/event-sales-graph-ticket-sale.component';

@NgModule({
  declarations: [
    LayoutAdminComponent,
    SidebarComponent,
    HomeComponent,
    TicketCategoriesComponent,
    CreateEditTicketCategoriesComponent,
    TicketSectorsComponent,
    CreateEditTicketSectorsComponent,
    ProviderPaymentsComponent,
    CreateEditProviderPaymentsComponent,
    CreateEditEventComponent,
    HeaderAdminComponent,
    EventsValidateComponent,
    ValidateApprovedTicketDialogComponent,
    ValidateNotApprovedTicketDialogComponent,
    ValidateAlreadyValidatedTicketDialogComponent,
    EventsValidateComponent,
    CreateUserComponent,
    EventSalesComponent,
    EventSalesTotalTicketsComponent,
    EventSalesTotalEntriesComponent,
    EventSalesTotalAvailableComponent,
    EventSalesTotalSalesComponent,
    EventSalesDaysToSaleComponent,
    EventSalesDaysToRemainingComponent,
    EventSalesGraphTicketSalesComponent,
    EventSalesGraphTicketSaleComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    AdminRoutingModule,
    SharedModule,
    ZXingScannerModule,
    NgChartsModule,
    CountUpModule
  ],
  schemas:[ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AdminModule {}
