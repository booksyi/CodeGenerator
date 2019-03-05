import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-constants-edit',
  templateUrl: './constants-edit.component.html',
  styleUrls: ['./constants-edit.component.scss']
})
export class ConstantsEditComponent implements OnInit {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.id > 0) {
      this.get(this.id);
    }
  }

  public id: number;
  constant: Constant = new Constant();

  get(id: number) {
    this.http.get<Constant>(
      '/api/constants/' + id
    ).subscribe(constant => {
      Object.assign(this.constant, constant);
    });
  }

  create() {
    this.http.post<Constant>(
      '/api/constants', this.constant
    ).subscribe(constant => {
      this.constant = constant;
      this.back();
    });
  }

  edit() {
    this.http.put<Constant>(
      '/api/constants/' + this.id, this.constant
    ).subscribe(constant => {
      this.constant = constant;
      this.back();
    });
  }

  back() {
    this.router.navigate(['constants/list']);
  }
}

export class Constant {
  result: string;
}
