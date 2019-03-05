import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TemplatesService, Template } from '../templates.service';

@Component({
  selector: 'app-templates-list',
  templateUrl: './templates-list.component.html',
  styleUrls: ['./templates-list.component.scss']
})
export class TemplatesListComponent implements OnInit {
  constructor(
    private modalService: NgbModal,
    private service: TemplatesService) { }

  ngOnInit() {
    this.list();
  }

  templates: Template[];
  confirmIndex: number = 0;

  list() {
    this.service.list().subscribe(templates => this.templates = templates);
  }

  create() {
    this.service.redirectToCreate();
  }

  edit(id: number) {
    this.service.redirectToEdit(id);
  }

  confirm(id: number, content) {
    this.confirmIndex = this.templates.findIndex(e => e.id === id);
    this.modalService.open(content);
  }

  delete(id: number) {
    this.service.delete(id).subscribe(() => this.list());
  }
}
