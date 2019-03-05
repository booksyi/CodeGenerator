import { Component, OnInit, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-constants-list',
  templateUrl: './constants-list.component.html',
  styleUrls: ['./constants-list.component.scss']
})
export class ConstantsListComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  constants: Constant[];
  confirmItem: Constant;

  list() {
    this.http.get<Constant[]>(
      '/api/constants'
    ).subscribe(constants => {
      this.constants = constants;
    });
  }

  create() {
    this.router.navigate(['constants/create']);
  }

  edit(id: number) {
    this.router.navigate(['constants/edit/' + id]);
  }

  confirm(id: number, content) {
    this.confirmItem = this.constants.filter(e => e.id === id)[0];
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

export class Constant {
  id: number;
  result: string;
}
