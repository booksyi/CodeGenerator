import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConstantsService, Constant } from '../constants.service';

@Component({
  selector: 'app-constants-edit',
  templateUrl: './constants-edit.component.html',
  styleUrls: ['./constants-edit.component.scss']
})
export class ConstantsEditComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private service: ConstantsService) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.id > 0) {
      this.get();
    }
  }

  id: number;
  constant: Constant = new Constant();

  get() {
    this.service.get(this.id).subscribe(constant => this.constant = constant);
  }

  create() {
    this.service.create(this.constant).subscribe(() => this.back());
  }

  update() {
    this.service.update(this.id, this.constant).subscribe(() => this.back());
  }

  back() {
    this.service.redirectToList();
  }
}
