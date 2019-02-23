import { Component, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-templates-list',
  templateUrl: './list.component.html',
  //styleUrls: ['./list.component.scss']
})
export class TemplatesListComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  request: TemplatesListRequest = {
  };

  resources: TemplatesListResource[];

  list() {
    this.http.get<TemplatesListResource[]>(
      '/api/templates' + buildQueryParams(this.request)
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

  confirmItem: TemplatesListResource;

  confirm(id: number, content) {
    this.confirmItem = this.resources.filter(e => e.id === id)[0];
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

export class TemplatesListRequest {
}

export class TemplatesListResource {
  id: number;
  name: string;
}
