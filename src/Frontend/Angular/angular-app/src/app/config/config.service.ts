import { HttpBackend, HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { Config } from './config';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  private httpClient: HttpClient;
  public config: Config = {
    apiRootUrl: 'http://localhost:5221'
  };

  constructor(private readonly httpHandler: HttpBackend) {
    this.httpClient = new HttpClient(this.httpHandler);
  }

  public loadConfig(): Promise<boolean> {
    const jsonFile = 'assets/config/app-config.json';
    return new Promise<boolean>((resolve, reject) => {
      this.httpClient.get<Config>(jsonFile)
        .pipe(map(res => res))
        .subscribe(value => {
          this.config = value;
          console.log(this.config);
          resolve(true);
        },
          (error) => {
            reject(error);
          });
    });
  }
}
