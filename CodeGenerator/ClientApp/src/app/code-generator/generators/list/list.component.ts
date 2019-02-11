import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-generators-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
/** list component*/
export class GeneratorsListComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
    this.getList();
  }

  request: GetListRequest = {
  };

  resources: GetListResource[];

  getList() {
    this.getListApi(this.request);
  }

  getListApi(query: GetListRequest) {
    this.http.get<GetListResource[]>(
      '/api/requestNodes' + buildQueryParams(query)
    ).subscribe(res => {
      this.resources = res;
    });
  }

  create() {
    this.router.navigate(['generators/create']);
  }

  edit(id: number) {
    this.router.navigate(['generators/edit/' + id]);
  }

  generate(id: number) {
    this.router.navigate(['generators/generate/' + id]);
  }
}

export class GetListRequest {
}

export class GetListResource {
  id: number;
  node: string;
  templates: string[];
}
