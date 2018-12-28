import { Injectable, Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { List, IResponse, buildQueryParams } from '../app.service';

export class GenerateModelRequest {
  actionName: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiGeneratorService {
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  generateModel(request: GenerateModelRequest) {
    return this.http.get<IResponse>(this.baseUrl + 'api/ApiGenerator/GenerateModel' + buildQueryParams(request));
  }
}
