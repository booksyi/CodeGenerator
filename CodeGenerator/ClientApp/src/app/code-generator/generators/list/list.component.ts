import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';
import { async } from '@angular/core/testing';

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
    this.list();
  }

  request: GetListRequest = {
  };

  resources: GetListResource[];

  list() {
    this.http.get<GetListResource[]>(
      '/api/codeTemplates' + buildQueryParams(this.request)
    ).subscribe(result => {
      this.resources = result;
      for (let resource of this.resources) {
        this.http.get<string[]>(
          '/api/codeTemplates/' + resource.id + '/templates'
        ).subscribe(templates => {
          resource.templates = templates;
        });
      }
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
