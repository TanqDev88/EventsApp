import { Component } from '@angular/core';
import { LayoutService } from '@services/layout.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {

  isLightLocal: boolean = false;
  bgColorLocal: string = '';
  logoSrcLocal: string = '';

  constructor(private layoutService: LayoutService) {

    let theme = this.layoutService.theme;
    this.bgColorLocal = theme.bgColor;
    this.logoSrcLocal = theme.logo;
    this.isLightLocal = theme.isLight;

    this.layoutService.themeChange.subscribe(() => {
      let theme = this.layoutService.theme;
      this.bgColorLocal = theme.bgColor;
      this.logoSrcLocal = theme.logo;
      this.isLightLocal = theme.isLight;
    });
  }
}
