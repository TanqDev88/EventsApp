import { Injectable } from '@angular/core';
import { AuthService, ConfigStateService, CurrentUserDto } from '@abp/ng.core';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { ticketeraConst } from '../shared/shared.const';

export interface Theme {
  isLight: boolean,
  bgColor: string,
  logo: string
}


@Injectable({
  providedIn: 'root'
})
export class LayoutService {

  public showMenu: boolean = false;
  public showFooter: boolean = false;
  public showHeader: boolean = false;
  public arrayLinks = null;
  private isMobileSubject = new BehaviorSubject<boolean>(false);

  // Define un único sujeto observable para todos los cambios relacionados con el tema
  public themeChange = new Subject<void>();

  private themeClient: Theme;

  constructor(private authService: AuthService, private config: ConfigStateService, private offcanvasService: NgbOffcanvas, private route: Router) {
    this.checkScreenSize();
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated;
  }

  public validateLogin(): void {
    if (!this.isAuthenticated) {
      this.showMenu = false;
      this.authService.navigateToLogin();
    }
    this.showMenu = true;
  }

  changeTheme(isLight: boolean): void {
    let theme: Theme
    if (isLight) {
      theme = {
        isLight: isLight,
        bgColor: ticketeraConst.theme.light.bgColor,
        logo: ticketeraConst.theme.light.logo
      };
    } else {
      theme = {
        isLight: isLight,
        bgColor: ticketeraConst.theme.dark.bgColor,
        logo: ticketeraConst.theme.dark.logo
      };
    }
    localStorage.removeItem("theme");
    localStorage.setItem("theme", JSON.stringify(theme));
    this.themeChange.next();
  }

  setThemeClient(theme: Theme): void {
    this.themeClient = (theme != null) ? theme : null;
    this.themeChange.next();
  }

  getThemeClient(): Theme {
    return this.themeClient;
  }

  get theme(): Theme {
    let themeString = localStorage.getItem("theme");
    let theme = this.themeClient != null ? this.themeClient : JSON.parse(themeString) as Theme;
    return theme;
  }

  get existTheme(): boolean {
    return this.theme != null;
  }

  logout() {
    this.offcanvasService.dismiss('Cross click');
    localStorage.clear();
    this.authService.logout().subscribe(() => {
      this.navigateTo('/events');
      this.changeTheme(false);
    });
  }

  navigateTo(url: string) {
    this.offcanvasService.dismiss('Cross click');
    this.route.navigateByUrl(url);
  }

  private checkScreenSize() {
    const screenWidth = window.innerWidth;
    const isMobile = screenWidth < 768; // Por ejemplo, aquí puedes definir el ancho límite para considerar como móvil
    this.isMobileSubject.next(isMobile);
    window.addEventListener('resize', () => {
      const newScreenWidth = window.innerWidth;
      const newIsMobile = newScreenWidth < 768;
      this.isMobileSubject.next(newIsMobile);
    });
  }

  public get isMobile(): Observable<boolean> {
    return this.isMobileSubject.asObservable();
  }

  public get currentUser(): Observable<CurrentUserDto> {
    return this.config.getOne$("currentUser");
  }
}
