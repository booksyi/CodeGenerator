import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-templates-edit',
  templateUrl: './edit.component.html',
  //styleUrls: ['./edit.component.scss']
})
export class TemplatesEditComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.get(this.id);
  }

  request: TemplatesEditRequest = {
    name: "",
    description: null,
    context: null
  };
  resources: TemplatesEditResource;

  get(id: number) {
    this.http.get<TemplatesGetResource>(
      '/api/templates/' + id
    ).subscribe(res => {
      this.request.name = res.name;
      this.request.description = res.description;
      this.request.context = res.context;
    });
  }

  edit() {
    this.http.put<TemplatesEditResource>(
      '/api/templates/' + this.id, this.request
    ).subscribe(res => {
      this.resources = res;
      this.back();
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
