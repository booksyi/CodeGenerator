import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-templates-create',
  templateUrl: './create.component.html',
  //styleUrls: ['./create.component.scss']
})
export class TemplatesCreateComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
  }

  request: TemplatesCreateRequest = {
    name: "",
    description: null,
    context: null
  };

  resources: TemplatesCreateResource;

  templatesCreate() {
    this.templatesCreateApi(this.request);
  }

  templatesCreateApi(query: TemplatesCreateRequest) {
    this.http.post<TemplatesCreateResource>(
      '/api/templates', query
    ).subscribe(res => {
      this.resources = res;
      this.back();
    }, err => {
      console.log(err)
    });
  }

  back() {
    this.router.navigate(['templates/list']);
  }
}

export class TemplatesCreateRequest {
  name: string;
  description: string;
  context: string;
}

export class TemplatesCreateResource {
}
