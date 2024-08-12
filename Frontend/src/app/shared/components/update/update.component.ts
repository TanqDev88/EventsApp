import { Component, OnInit } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { Router } from '@angular/router';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.scss']
})
export class UpdateComponent implements OnInit {

  showUpdate: boolean = false;

  constructor(private swUpdate: SwUpdate, private router: Router) {}

  ngOnInit(): void {
    if (this.swUpdate.isEnabled) {
      this.swUpdate.versionUpdates.subscribe(event => {
        switch (event.type) {
          case 'VERSION_DETECTED':
            console.log(`New version detected: ${event.version.hash}`);
            break;
          case 'VERSION_READY':
            console.log(`New version ready for use: ${event.latestVersion.hash}`);
            this.promptUser();
            break;
          case 'VERSION_INSTALLATION_FAILED':
            console.log(`Failed to install new version: ${event.version.hash}`);
            break;
        }
      });
    }
  }

  promptUser() {
    this.showUpdate = true;
  }

  updateVersion() {
    this.router.navigateByUrl('/').then(() => {
        window.location.reload(); // Asegura que todos los archivos sean recargados desde el servidor
    });
  }
}
