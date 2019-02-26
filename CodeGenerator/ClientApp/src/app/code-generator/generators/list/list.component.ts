import { Component, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  request: GetListRequest = {
  };

  resources: GetListResource[];

  list() {
    this.http.get<GetListResource[]>(
      '/api/generators' + buildQueryParams(this.request)
    ).subscribe(result => {
      this.resources = result;
      for (let resource of this.resources) {
        this.http.get<string[]>(
          '/api/generators/' + resource.id + '/templates'
        ).subscribe(templates => {
          resource.templates = templates;
        });
      }
    });
  }

  create() {
    this.router.navigate(['generators/edit']);
  }

  edit(id: number) {
    this.router.navigate(['generators/edit/' + id]);
  }

  generate(id: number) {
    this.router.navigate(['generators/generate/' + id]);
  }

  confirmItem: GetListResource;

  confirm(id: number, content) {
    this.confirmItem = this.resources.filter(e => e.id === id)[0];
    this.modalService.open(content);
  }

  delete(id: number) {
    this.http.delete(
      '/api/generators/' + id
    ).subscribe(() => {
      this.confirmItem = null;
      this.list();
    });
  }
}

export class GetListRequest {
}

export class GetListResource {
  id: number;
  node: string;
  templates: string[];
}
