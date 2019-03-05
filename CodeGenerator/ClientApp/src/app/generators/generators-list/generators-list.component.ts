import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GeneratorsService } from '../generators.service';

@Component({
  selector: 'app-generators-list',
  templateUrl: './generators-list.component.html',
  styleUrls: ['./generators-list.component.scss']
})
export class GeneratorsListComponent implements OnInit {
  constructor(
    private modalService: NgbModal,
    private service: GeneratorsService) { }

  ngOnInit() {
    this.list();
  }

  generators: Generator[];
  confirmIndex: number = 0;

  list() {
    this.service.list().subscribe(generators => this.generators = generators);
  }

  create() {
    this.service.redirectToCreate();
  }

  edit(id: number) {
    this.service.redirectToEdit(id);
  }

  generate(id: number) {
    this.service.redirectToGenerate(id);
  }

  confirm(id: number, content) {
    this.confirmIndex = this.generators.findIndex(e => e.id === id);
    this.modalService.open(content);
  }

  delete(id: number) {
    this.service.delete(id).subscribe(() => this.list());
  }
}

export class Generator {
  id: number;
  name: string;
  templates: string[];
}
