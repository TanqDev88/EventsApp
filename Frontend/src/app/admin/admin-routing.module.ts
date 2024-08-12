import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthGuard, PermissionGuard } from '@abp/ng.core';
import { LayoutAdminComponent } from './layout-admin/layout-admin.component';
import { TicketCategoriesComponent } from './ticket-categories/ticket-categories.component';
import { TicketSectorsComponent } from './ticket-sectors/ticket-sectors.component';
import { ProviderPaymentsComponent } from './provider-payments/provider-payments.component';
import { CreateEditEventComponent } from './events-list/create-edit-event/create-edit-event.component';
import { EventsValidateComponent } from './events-list/events-validate/events-validate.component';
import { EventSalesComponent } from './events-list/event-sales/event-sales.component';
import { ticketeraConst } from '../shared/shared.const';

const routes: Routes = [
  {
    path: '',
    component: LayoutAdminComponent,
    children: [
      {
        path: 'dashboard',
        pathMatch: 'full',
        canActivate: [AuthGuard],
        component: HomeComponent,
        data: { title: 'Admin' + ticketeraConst.tixgoTitle }
      },
      {
        path: 'identity',
        loadChildren: () => import('@abp/ng.identity').then(m => m.IdentityModule.forLazy()),
      },
      {
        path: 'tenant-management',
        loadChildren: () =>
          import('@abp/ng.tenant-management').then(m => m.TenantManagementModule.forLazy()),
      },
      {
        path: 'setting-management',
        loadChildren: () =>
          import('@abp/ng.setting-management').then(m => m.SettingManagementModule.forLazy()),
      },
      {
        path: 'ticket-category',
        component: TicketCategoriesComponent,
        canActivate: [AuthGuard, PermissionGuard],
        data: { permission: "Pages.Categories" },
      },
      {
        path: 'events/create',
        component: CreateEditEventComponent,
        canActivate: [AuthGuard],
        data: { },
      },
      {
        path: 'events/create/:id',
        component: CreateEditEventComponent,
        canActivate: [AuthGuard],
        data: { },
      },
      {
        path: 'events/:id/validate',
        component: EventsValidateComponent,
        canActivate: [AuthGuard],
        data: { },
      },
      {
        path: 'events/:id/sales',
        component: EventSalesComponent,
        canActivate: [AuthGuard],
        data: { },
      },
      {
        path: 'ticket-sector',
        component: TicketSectorsComponent,
        canActivate: [AuthGuard, PermissionGuard],
        data: { permission: "Pages.Sectors" },
      },
      {
        path: 'provider-payment',
        component: ProviderPaymentsComponent,
        canActivate: [AuthGuard],
        data: {},
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: '**',
        redirectTo: 'dashboard'
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
