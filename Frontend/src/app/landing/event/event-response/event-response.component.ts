import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { EventService, PurchaseCheckDto, PurchaseDto } from '@proxy/events';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-event-response',
  templateUrl: './event-response.component.html',
  styleUrls: ['./event-response.component.scss']
})
export class EventResponseComponent implements OnInit, OnDestroy {
  purchaseCheckInput: PurchaseCheckDto;
  purchase: PurchaseDto;

  private connection: HubConnection;

  constructor(private route: ActivatedRoute,
    private eventService: EventService) {

  }

  ngOnInit(): void {

    let code = this.route.snapshot.paramMap.get('code');
    console.log(code);

    // Capturar todos los parámetros de la URL
    this.route.queryParams.subscribe(params => {

      this.purchaseCheckInput = params;
      console.log(!this.purchaseCheckInput.external_reference);

      if (!this.purchaseCheckInput.external_reference) return;

      let code = this.purchaseCheckInput.external_reference;
      this.eventService.purchaseGetByCode(code).subscribe(purchase => {
        this.purchase = purchase;

        this.eventService.purchaseCheckByInput(this.purchaseCheckInput).subscribe(result => {
          console.log(result);
          this.purchaseCheckInput.status = result;
        });
      });


      // -- Set Hub
      this.connection = new HubConnectionBuilder()
        .withUrl(environment.apis.default.url + '/signalr-hubs/event')
        .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
        .build();

      // -- Receive Message NotificationTicketSold
      this.connection.on("NotificationTicketSold", data => {
        console.log(data);
        if (data && data == this.purchaseCheckInput.external_reference) {
          this.eventService.purchaseCheckByInput(this.purchaseCheckInput).subscribe(result => {
            console.log(result);
            this.purchaseCheckInput.status = result;
          });
        }
      });

      // -- Init connection
      const startConnection = () => {
        this.connection.start().then(_ => {
          console.log('Connection Started');
        }).catch(error => {
          console.error('Error starting connection:', error);
          // Retrying connection after 5 seconds
          setTimeout(startConnection, 5000);
        });
      };

      startConnection();

      // -- Close connection
      this.connection.onclose(error => {
        console.log('Connection Closed:', error);
        // Reconnect
        startConnection();
      });


    });
  }

  ngOnDestroy(): void {
    if (this.connection) {
      this.connection.stop();
      this.connection = null;
    }
  }

}
