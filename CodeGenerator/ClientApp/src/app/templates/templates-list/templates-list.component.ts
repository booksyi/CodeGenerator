import { Component, OnInit, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-templates-list',
  templateUrl: './templates-list.component.html',
  styleUrls: ['./templates-list.component.scss']
})
export class TemplatesListComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  templates: Template[];
  confirmItem: Template;

  list() {
    this.http.get<Template[]>(
      '/api/templates'
    ).subscribe(templates => {
      this.templates = templates;
    });
  }

  create() {
    this.router.navigate(['templates/create']);
  }

  edit(id: number) {
    this.router.navigate(['templates/edit/' + id]);
  }

  confirm(id: number, content) {
    this.confirmItem = this.templates.filter(e => e.id === id)[0];
    this.modalService.open(content);
  }

  delete(id: number) {
    this.http.delete(
      '/api/templates/' + id
    ).subscribe(() => {
      this.confirmItem = null;
      this.list();
    });
  }
}

export class Template {
  id: number;
  name: string;
}
