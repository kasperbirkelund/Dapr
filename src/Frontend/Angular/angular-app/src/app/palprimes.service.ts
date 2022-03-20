import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConfigService } from './config/config.service';

@Injectable({
  providedIn: 'root'
})
export class PalprimesService {

  private serviceUrl = `${this.configService.config.apiRootUrl}/calculation/api/GetCalculations`;
  constructor(private httpClient: HttpClient, private configService: ConfigService) { }

  get(clientId: string, range: number): Observable<any> {
    return this.httpClient.get(`${this.serviceUrl}/${clientId}/${range}`);
  }
}
