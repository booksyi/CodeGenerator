import { Component, OnInit, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-api-constants-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
/** list component*/
export class ApiConstantsListComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  request: ApiConstantsListRequest = {
  };

  resources: ApiConstantsListResource[];

  list() {
    this.listApi(this.request);
  }

  listApi(query: ApiConstantsListRequest) {
    this.http.get<ApiConstantsListResource[]>(
      '/api/apiConstants' + buildQueryParams(query)
    ).subscribe(res => {
      this.resources = res;
    });
  }

  create() {
    this.router.navigate(['api-constants/create']);
  }

  edit(id: number) {
    this.router.navigate(['api-constants/edit/' + id]);
  }

  confirmItem: ApiConstantsListResource;

  deleteConfirm(id: number, content) {
    this.confirmItem = this.resources.filter(e => e.id === id)[0];
    this.modalService.open(content);
  }

  delete(id: number) {
    this.http.delete(
      '/api/apiConstants/' + id
    ).subscribe(() => {
      this.list();
    });
  }
}

export class ApiConstantsListRequest {
}

export class ApiConstantsListResource {
  id: number;
  result: string;
}
