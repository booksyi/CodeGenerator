import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-templates-edit',
  templateUrl: './edit.component.html',
  //styleUrls: ['./edit.component.scss']
})
export class TemplatesEditComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.templatesGetApi(this.id);
  }

  request: TemplatesEditRequest = {
    name: "",
    description: null,
    context: null
  };

  resources: TemplatesEditResource;

  templatesGetApi(id: number) {
    this.http.get<TemplatesGetResource>(
      '/api/templates/' + id
    ).subscribe(res => {
      this.request.name = res.name;
      this.request.description = res.description;
      this.request.context = res.context;
    });
  }

  templatesEdit() {
    this.templatesEditApi(this.request);
    this.back();
  }

  templatesEditApi(query: TemplatesEditRequest) {
    this.http.put<TemplatesEditResource>(
      '/api/templates/' + this.id, query
    ).subscribe(res => {
      this.resources = res;
    });
  }

  back() {
    this.router.navigate(['templates/list']);
  }
}

export class TemplatesGetResource {
  name: string;
  description: string;
  context: string;
}

export class TemplatesEditRequest {
  name: string;
  description: string;
  context: string;
}

export class TemplatesEditResource {
}
