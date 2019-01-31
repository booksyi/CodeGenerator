import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
    selector: 'app-temp-api-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
/** list component*/
export class TempApiListComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
    this.tempApiList();
  }

  request: TempApiListRequest = {
  };

  resources: TempApiListResource[];

  tempApiList() {
    this.tempApiListApi(this.request);
  }

  tempApiListApi(query: TempApiListRequest) {
    this.http.get<TempApiListResource[]>(
      '/api/temp' + buildQueryParams(query)
    ).subscribe(res => {
      this.resources = res;
    });
  }

  create() {
    this.router.navigate(['temp-api/create']);
  }

  edit(id: number) {
    this.router.navigate(['temp-api/edit/' + id]);
  }
}

export class TempApiListRequest {
}

export class TempApiListResource {
  id: number;
  result: string;
}
