import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SplashScreenService {
  private _isShow: BehaviorSubject<boolean> = new BehaviorSubject(true);
  isShow = this._isShow.asObservable();
  private renderer: Renderer2;

  constructor(private rendererFactory: RendererFactory2) {
    this.renderer = this.rendererFactory.createRenderer(null, null);
  }

  show() {
    this._isShow.next(true);
    let appLoading = this.renderer.selectRootElement('#app-loading');
    this.renderer.removeClass(appLoading, 'hide');
  }

  hide(): void {
    this._isShow.next(false);
    let appLoading = this.renderer.selectRootElement('#app-loading');
    if (appLoading) {
      this.renderer.addClass(appLoading, 'hide');
    }
  }
}
