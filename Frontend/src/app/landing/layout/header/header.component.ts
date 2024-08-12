import { Component } from '@angular/core';
import { LayoutService } from '@services/layout.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {

  isLightLocal: boolean = false;
  bgColorLocal: string = '';
  logoSrcLocal: string = '';
  isThemeClient: boolean;
  isMobile: boolean;

  constructor(private layoutService: LayoutService, private router: Router) {

    let theme = this.layoutService.theme;
    this.bgColorLocal = theme.bgColor;
    this.logoSrcLocal = theme.logo;
    this.isLightLocal = theme.isLight;

    this.layoutService.themeChange.subscribe(() => {
      let theme = this.layoutService.theme;
      this.isLightLocal = theme.isLight;
      this.bgColorLocal = theme.bgColor;
      this.logoSrcLocal = theme.logo;
    });

    this.layoutService.isMobile.subscribe(result =>{
      this.isMobile = result;
    });
  }

  isThemeClientfunc():boolean{
    return this.isThemeClient = !this.layoutService.getThemeClient();
  }

  get hasLoggedIn(): boolean {
    return this.layoutService.isAuthenticated;
  }

  go(uri: string): void {
    this.router.navigateByUrl(uri);
  }

  logout() {
    this.layoutService.logout();
  }

  changeLight() {
    this.layoutService.changeTheme(!this.isLightLocal);
  }

}
