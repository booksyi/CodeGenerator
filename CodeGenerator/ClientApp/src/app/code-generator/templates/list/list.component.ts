import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-templates-list',
  templateUrl: './list.component.html',
  //styleUrls: ['./list.component.scss']
})
export class TemplatesListComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
    this.templatesList();
  }

  request: TemplatesListRequest = {
  };

  resources: TemplatesListResource[];

  templatesList() {
    this.templatesListApi(this.request);
  }

  templatesListApi(query: TemplatesListRequest) {
    this.http.get<TemplatesListResource[]>(
      '/api/templates' + buildQueryParams(query)
    ).subscribe(res => {
      this.resources = res;
    });
  }

  create() {
    this.router.navigate(['templates/create']);
  }

  edit(id: number) {
    this.router.navigate(['templates/edit/' + id]);
  }
}

export class TemplatesListRequest {
}

export class TemplatesListResource {
  id: number;
  name: string;
}
