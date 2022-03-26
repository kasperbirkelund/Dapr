import { Component, OnDestroy, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { PalprimesService } from './palprimes.service';
import { Guid } from 'guid-typescript';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { Palprime } from './palprime';
import { trigger, transition, style, animate, query, stagger } from '@angular/animations';
import { ConfigService } from './config/config.service';

const listFadeAnimation = trigger('listFadeAnimation', [
  transition('* <=> *', [
    query(':enter',
      [style({ opacity: 0 }), stagger('200ms', animate('600ms ease-out', style({ opacity: 1 })))],
      { optional: true }
    ),
    query(':leave',
      animate('200ms', style({ opacity: 0 })),
      { optional: true }
    )
  ])
]);

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  animations: [listFadeAnimation]
})
export class AppComponent implements OnInit, OnDestroy {

  public range: number = 10;

  public progressValue: number = 0;
  public progressIncrement: number = 0;

  public palprimes: BehaviorSubject<Palprime[]> = new BehaviorSubject<Palprime[]>([]);

  private hubConnection: HubConnection;
  private hubConnectionUri = `${this.configService.config.apiRootUrl}/notifications`;
  private clientId = Guid.create().toString();

  constructor(private service: PalprimesService, private configService: ConfigService) {
    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug)
      .withUrl(this.hubConnectionUri, {
        skipNegotiation: true, //skip negotiation as we use websockets. Not supported by old browsers.
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();

  }

  ngOnInit(): void {
    this.hubConnection.on("result", (palprime: Palprime) => {
      console.log(palprime);
      this.progressValue = this.progressValue + this.progressIncrement;
      this.range = this.range - 1;
      if (palprime.isPal && palprime.isPrime) {
        const current = this.palprimes.value;
        const updated = [...current, palprime];
        updated.sort((n1, n2) => n1.number - n2.number);
        this.palprimes.next(updated);
      }
      if (this.range == 0) this.progressValue = 0;
    });

    this.hubConnection.start()
      .then(() => {
        this.hubConnection
          .send("Subscribe", this.clientId);
      })
      .catch(err => {
        console.log('Error while establishing connection :(')
        console.error(err);
      });
  }

  load(): void {
    this.palprimes.next([]);
    this.progressIncrement = 100 / this.range;
    this.progressValue = 0;

    this.service.get(this.clientId, this.range)
      .subscribe({
        next: () => {
          console.log("Range is: " + this.range);
        },
        error: (errorResponse) => {
          console.log("Error: " + errorResponse.message);
        }
      });
  }

  ngOnDestroy(): void {
    this.hubConnection
      .send("unsubscribe", this.clientId);
  }

}
