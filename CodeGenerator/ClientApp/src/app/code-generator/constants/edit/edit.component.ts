import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-constants-edit',
    templateUrl: './edit.component.html',
    styleUrls: ['./edit.component.scss']
})
/** edit component*/
export class ConstantsEditComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.get(this.id);
  }

  request: ConstantsEditRequest = {
    result: null
  };
  resources: ConstantsEditResource;

  get(id: number) {
    this.http.get<ConstantsGetResource>(
      '/api/constants/' + id
    ).subscribe(res => {
      this.request.result = res.result;
    });
  }

  edit() {
    this.http.put<ConstantsEditResource>(
      '/api/constants/' + this.id, this.request
    ).subscribe(res => {
      this.resources = res;
      this.back();
    });
  }

  back() {
    this.router.navigate(['constants/list']);
  }
}

export class ConstantsGetResource {
  result: string;
}

export class ConstantsEditRequest {
  result: string;
}

export class ConstantsEditResource {
}
