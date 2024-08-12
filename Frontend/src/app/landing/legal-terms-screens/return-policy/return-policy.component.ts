import { Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import { LocalizationService } from '@abp/ng.core';

@Component({
  selector: 'app-return-policy',
  templateUrl: './return-policy.component.html',
  styleUrls: ['./return-policy.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReturnPolicyComponent implements OnInit {
  public language: string;

  constructor(private localizationService: LocalizationService) {}

  ngOnInit(): void {
    this.language = this.localizationService.currentLang;
  }
}
