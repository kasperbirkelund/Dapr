import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PalprimesService {

  private serviceUrl = `${environment.palprimesServiceUrl}/calculation/api/GetCalculations`;
  constructor(private httpClient: HttpClient) { }

  get(clientId: string, range: number): Observable<any> {
    return this.httpClient.get(`${this.serviceUrl}/${clientId}/${range}`);
  }
}
