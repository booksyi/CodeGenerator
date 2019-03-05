import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TemplatesService, Template } from '../templates.service';

@Component({
  selector: 'app-templates-edit',
  templateUrl: './templates-edit.component.html',
  styleUrls: ['./templates-edit.component.scss']
})
export class TemplatesEditComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private service: TemplatesService) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.id > 0) {
      this.get(this.id);
    }
  }

  id: number;
  template: Template = new Template();

  get(id: number) {
    this.service.get(id).subscribe(template => this.template = template);
  }

  create() {
    this.service.create(this.template).subscribe(() => this.back());
  }

  update() {
    this.service.update(this.id, this.template).subscribe(() => this.back());
  }

  back() {
    this.service.redirectToList();
  }
}
