import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-templates-edit',
  templateUrl: './templates-edit.component.html',
  styleUrls: ['./templates-edit.component.scss']
})
export class TemplatesEditComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.get(this.id);
  }

  public id: number;
  template: Template = new Template();

  get(id: number) {
    this.http.get<Template>(
      '/api/templates/' + id
    ).subscribe(template => {
      Object.assign(this.template, template);
    });
  }

  create() {
    this.http.post<Template>(
      '/api/templates', this.template
    ).subscribe(template => {
      this.back();
    }, err => {
      console.log(err)
    });
  }

  edit() {
    this.http.put<Template>(
      '/api/templates/' + this.id, this.template
    ).subscribe(template => {
      this.back();
    });
  }

  back() {
    this.router.navigate(['templates/list']);
  }
}

export class Template {
  name: string;
  content: string;
}
