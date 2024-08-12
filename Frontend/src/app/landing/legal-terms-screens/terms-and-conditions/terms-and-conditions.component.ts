import { Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import { LocalizationService } from '@abp/ng.core';

@Component({
  selector: 'app-terms-and-conditions',
  templateUrl: './terms-and-conditions.component.html',
  styleUrls: ['./terms-and-conditions.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TermsAndConditionsComponent implements OnInit {
  public language: string;

  constructor(private localizationService: LocalizationService) {}

  ngOnInit(): void {
    this.language = this.localizationService.currentLang;
  }
}
