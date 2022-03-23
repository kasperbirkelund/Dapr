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
    apiRootUrl: ''
  };

  constructor(private readonly httpHandler: HttpBackend) {
    this.httpClient = new HttpClient(this.httpHandler);
  }

  public loadConfig(): Promise<boolean> {
    const jsonFile = 'assets/config/app-config.json';
    console.log(`Loading config from ${jsonFile}`);

    return new Promise<boolean>((resolve, reject) => {
      this.httpClient.get<Config>(jsonFile)
        .pipe(map(res => res))
        .subscribe(value => {
          console.log(`Config apiRootUrl: ${value.apiRootUrl}`);
          this.config = value;
          if (this.config.apiRootUrl.length === 0) {
            console.log('No RESOURCES_API_ROOT_URL env var set. Defaulting to docker desktop setup.');
            this.config.apiRootUrl = 'http://localhost:5221'
          }
          console.log(this.config);
          resolve(true);
        },
          (error) => {
            reject(error);
          });
    });
  }
}
