import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card-block',
  templateUrl: './card-block.component.html',
  styleUrls: ['./card-block.component.scss']
})
export class CardBlockComponent {
  @Input() load: boolean = false;
  @Input() height: string = '100%';
}
