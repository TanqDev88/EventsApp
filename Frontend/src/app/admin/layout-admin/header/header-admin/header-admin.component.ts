import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-header-admin',
  templateUrl: './header-admin.component.html',
  styleUrls: ['./header-admin.component.scss']
})
export class HeaderAdminComponent {
  @Output() sidebarVisibilityChange = new EventEmitter<boolean>();
  sidebarVisible: boolean = false;
  themeIsDark = false;

  get themeColor():string {
    return this.themeIsDark ? 'text-white bg-dark':'text-white bg-dark';
  }

  toggleSidebar() {
    this.sidebarVisible = !this.sidebarVisible;
    this.sidebarVisibilityChange.emit(this.sidebarVisible);
  }
}
