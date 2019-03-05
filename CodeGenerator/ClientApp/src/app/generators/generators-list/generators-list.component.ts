import { Component, OnInit, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-generators-list',
  templateUrl: './generators-list.component.html',
  styleUrls: ['./generators-list.component.scss']
})
export class GeneratorsListComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  ngOnInit() {
    this.list();
  }

  generators: Generator[];
  confirmItem: Generator;

  list() {
    this.http.get<Generator[]>(
      '/api/generators'
    ).subscribe(generators => {
      this.generators = generators;
      for (let generator of this.generators) {
        this.http.get<string[]>(
          '/api/generators/' + generator.id + '/templates'
        ).subscribe(templates => {
          generator.templates = templates;
        });
      }
    });
  }

  create() {
    this.router.navigate(['generators/create']);
  }

  edit(id: number) {
    this.router.navigate(['generators/edit/' + id]);
  }

  confirm(id: number, content) {
    this.confirmItem = this.generators.filter(e => e.id === id)[0];
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

  generate(id: number) {
    this.router.navigate(['generators/generate/' + id]);
  }
}

export class Generator {
  id: number;
  name: string;
  templates: string[];
}
