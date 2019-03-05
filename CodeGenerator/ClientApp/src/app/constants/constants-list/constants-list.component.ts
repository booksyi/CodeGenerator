import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConstantsService, Constant } from '../constants.service';

@Component({
  selector: 'app-constants-list',
  templateUrl: './constants-list.component.html',
  styleUrls: ['./constants-list.component.scss']
})
export class ConstantsListComponent implements OnInit {
  constructor(
    private modalService: NgbModal,
    private service: ConstantsService) { }

  ngOnInit() {
    this.list();
  }

  constants: Constant[];
  confirmIndex: number = 0;

  list() {
    this.service.list().subscribe(constants => this.constants = constants);
  }

  create() {
    this.service.redirectToCreate();
  }
  
  edit(id: number) {
    this.service.redirectToEdit(id);
  }

  confirm(id: number, content) {
    this.confirmIndex = this.constants.findIndex(e => e.id === id);
    this.modalService.open(content);
  }

  delete(id: number) {
    this.service.delete(id).subscribe(() => this.list());
  }
}
