import { Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import { LocalizationService } from '@abp/ng.core';

@Component({
  selector: 'app-privacy-policy',
  templateUrl: './privacy-policy.component.html',
  styleUrls: ['./privacy-policy.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PrivacyPolicyComponent implements OnInit {
  public language: string;

  constructor(private localizationService: LocalizationService) {}

  ngOnInit(): void {
    this.language = this.localizationService.currentLang;
  }
}
