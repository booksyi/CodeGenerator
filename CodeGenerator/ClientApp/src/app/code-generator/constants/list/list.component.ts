import { Component, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-constants-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
/** list component*/
export class ConstantsListComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  request: ConstantsListRequest = { };
  resources: ConstantsListResource[];
  confirmItem: ConstantsListResource;

  list() {
    this.http.get<ConstantsListResource[]>(
      '/api/constants' + buildQueryParams(this.request)
    ).subscribe(res => {
      this.resources = res;
    });
  }

  create() {
    this.router.navigate(['constants/create']);
  }

  edit(id: number) {
    this.router.navigate(['constants/edit/' + id]);
  }

  confirm(id: number, content) {
    this.confirmItem = this.resources.filter(e => e.id === id)[0];
    this.modalService.open(content);
  }

  delete(id: number) {
    this.http.delete(
      '/api/constants/' + id
    ).subscribe(() => {
      this.confirmItem = null;
      this.list();
    });
  }
}

export class ConstantsListRequest {
}

export class ConstantsListResource {
  id: number;
  result: string;
}
