import { animate, query, style, transition, trigger } from '@angular/animations';
import { AfterViewInit, ChangeDetectorRef, Component } from '@angular/core';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
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
export class LayoutComponent implements AfterViewInit {
  constructor(private cd: ChangeDetectorRef
     ) {


      }


  ngAfterViewInit(): void{
    this.cd.detectChanges();
  }

}
