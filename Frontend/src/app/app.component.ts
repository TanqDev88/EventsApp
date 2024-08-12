import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription, filter, fromEvent, map, merge, mergeMap, of } from 'rxjs';
import { LayoutService } from './services/layout.service';
import { SplashScreenService } from './services/splash-screen.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { LocalizationService } from '@abp/ng.core';
import { ticketeraConst } from './shared/shared.const';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar></abp-loader-bar>
    <app-update></app-update>
    <router-outlet #outlet></router-outlet>
  `
})
export class AppComponent implements OnInit, OnDestroy{
  networkStatus: boolean = false;
  networkStatus$: Subscription = Subscription.EMPTY;

  constructor(
    private layoutService: LayoutService, 
    private splashScreenService : SplashScreenService, 
    private router: Router, 
    private activatedRoute: ActivatedRoute, 
    private titleService: Title, 
    private localizationService: LocalizationService)
  {
    if (!this.layoutService.existTheme)
      this.layoutService.changeTheme(false);

  }

  ngOnInit(): void {
    this.checkNetworkStatus()
    setTimeout(()=>{
      this.splashScreenService.hide();
    },1000)

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.activatedRoute),
      map(route => {
        while (route.firstChild) route = route.firstChild;
        return route;
      }),
      filter(route => route.outlet === 'primary'),
      mergeMap(route => route.data)
    ).subscribe(data => {
      const titleKey = data['titleKey'];
      const defaultTitle = 'TixGo';

      let title: string;

      if (data['title']) {
        title = data['title'];
      } else if (titleKey) {
        title = this.localizationService.instant("::" + titleKey) + ticketeraConst.tixgoTitle;
      } else {
        title = defaultTitle;
      }
      this.titleService.setTitle(title);
    });
  }

  ngOnDestroy(): void {
    this.networkStatus$.unsubscribe();
  }

  checkNetworkStatus() {
    this.networkStatus = navigator.onLine;
    this.networkStatus$ = merge(
      of(null),
      fromEvent(window, 'online'),
      fromEvent(window, 'offline')
    )
      .pipe(map(() => navigator.onLine))
      .subscribe(status => {
        this.networkStatus = status;
      });
  }
  isEnable(): boolean {
    return location.pathname.includes('account');
  }
}

