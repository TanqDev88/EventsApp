import { animate, query, style, transition, trigger } from '@angular/animations';
import { AfterViewInit, ChangeDetectorRef, Component } from '@angular/core';
import { LayoutService } from '@services/layout.service';

@Component({
  selector: 'app-layout-admin',
  templateUrl: './layout-admin.component.html',
  styleUrls: ['./layout-admin.component.scss'],
  animations: [
    trigger('inOutPage', [
      transition('* => *', [
        query(
          ':enter',
          [style({ opacity: 0 })],
          { optional: true }
        ),
        query(
          ':leave',
          [style({ opacity: 1 }), animate('0.2s', style({ opacity: 0 }))],
          { optional: true }
        ),
        query(
          ':enter',
          [style({ opacity: 0 }), animate('0.1s', style({ opacity: 1 }))],
          { optional: true }
        )
      ])
    ])
  ]
})
export class LayoutAdminComponent implements AfterViewInit {
  sidebarVisible: boolean = false;

  constructor(public layoutService: LayoutService,
     private cd: ChangeDetectorRef
     ) {

      }


  ngAfterViewInit(): void{
    this.cd.detectChanges();
  }

  toggleSidebar(sidebarVisible: boolean) {
    this.sidebarVisible = sidebarVisible;
  }
}
