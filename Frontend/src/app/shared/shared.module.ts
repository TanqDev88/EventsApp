import { CoreModule } from '@abp/ng.core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { CommonModule } from '@angular/common';
import { EventGalleryComponent } from './components/event-gallery/event-gallery.component';
import { DateFromToComponent } from './components/date-from-to/date-from-to.component';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { CardBlockComponent } from './components/card-block/card-block.component';

@NgModule({
  imports: [
    CommonModule,
    CoreModule.forChild(),
    NgbModule,
    ThemeSharedModule,
    NgxValidateCoreModule,
    NgxSkeletonLoaderModule
  ],
  exports: [
    CommonModule,
    CoreModule,
    NgbModule,
    ThemeSharedModule,
    NgxValidateCoreModule,
    NgxSkeletonLoaderModule,
    EventGalleryComponent,
    DateFromToComponent,
    CardBlockComponent
  ],
  providers: [],
  declarations: [
    EventGalleryComponent,
    DateFromToComponent,
    CardBlockComponent
  ],
  schemas:[ CUSTOM_ELEMENTS_SCHEMA ]
})
export class SharedModule {}
