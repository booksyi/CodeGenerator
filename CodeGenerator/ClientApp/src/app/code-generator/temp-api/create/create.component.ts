import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    selector: 'app-temp-api-create',
    templateUrl: './create.component.html',
    styleUrls: ['./create.component.scss']
})
/** create component*/
export class TempApiCreateComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
  }

  request: TempApiCreateRequest = {
    result: null
  };

  resources: TempApiCreateResource;

  create() {
    this.createApi(this.request);
  }

  createApi(query: TempApiCreateRequest) {
    this.http.post<TempApiCreateResource>(
      '/api/temp', query
    ).subscribe(res => {
      this.resources = res;
      this.back();
    });
  }

  back() {
    this.router.navigate(['temp-api/list']);
  }
}

export class TempApiCreateRequest {
  result: string;
}

export class TempApiCreateResource {
}
