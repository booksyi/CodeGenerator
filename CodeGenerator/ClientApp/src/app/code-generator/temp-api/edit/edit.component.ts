import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-temp-api-edit',
    templateUrl: './edit.component.html',
    styleUrls: ['./edit.component.scss']
})
/** edit component*/
export class TempApiEditComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.getApi(this.id);
  }

  request: TempApiEditRequest = {
    result: null
  };

  resources: TempApiEditResource;

  getApi(id: number) {
    this.http.get<TempApiGetResource>(
      '/api/temp/' + id
    ).subscribe(res => {
      this.request.result = res.result;
    });
  }

  edit() {
    this.editApi(this.request);
  }

  editApi(query: TempApiEditRequest) {
    this.http.put<TempApiEditResource>(
      '/api/temp/' + this.id, query
    ).subscribe(res => {
      this.resources = res;
      this.back();
    });
  }

  back() {
    this.router.navigate(['temp-api/list']);
  }
}

export class TempApiGetResource {
  result: string;
}

export class TempApiEditRequest {
  result: string;
}

export class TempApiEditResource {
}
