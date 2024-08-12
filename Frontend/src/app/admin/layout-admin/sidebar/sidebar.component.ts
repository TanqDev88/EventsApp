import { ConfigStateService, CurrentUserDto } from '@abp/ng.core';
import { Component, TemplateRef } from '@angular/core';
import { Observable } from 'rxjs';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { LayoutService } from '@services/layout.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent  {
  currentUser$: Observable<CurrentUserDto> = this.configState.getOne$('currentUser');
  themeIsDark = false;

  constructor(
    private offcanvasService: NgbOffcanvas,
    public layoutService: LayoutService,
    private configState: ConfigStateService,
    private route:Router) {}


  get themeColor():string {
    return this.themeIsDark ? 'text-white bg-dark':'text-white bg-dark';
  }

  openBottom(content: TemplateRef<any>) {
		this.offcanvasService.open(content, { position: 'bottom' });
	}

  navigateTo(url: string){
    this.offcanvasService.dismiss('Cross click');
    this.route.navigateByUrl(url);
  }
}
