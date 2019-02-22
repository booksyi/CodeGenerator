import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-api-constants-edit',
    templateUrl: './edit.component.html',
    styleUrls: ['./edit.component.scss']
})
/** edit component*/
export class ApiConstantsEditComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.getApi(this.id);
  }

  request: ApiConstantsEditRequest = {
    result: null
  };

  resources: ApiConstantsEditResource;

  getApi(id: number) {
    this.http.get<ApiConstantsGetResource>(
      '/api/apiConstants/' + id
    ).subscribe(res => {
      this.request.result = res.result;
    });
  }

  edit() {
    this.editApi(this.request);
  }

  editApi(query: ApiConstantsEditRequest) {
    this.http.put<ApiConstantsEditResource>(
      '/api/apiConstants/' + this.id, query
    ).subscribe(res => {
      this.resources = res;
      this.back();
    });
  }

  back() {
    this.router.navigate(['api-constants/list']);
  }
}

export class ApiConstantsGetResource {
  result: string;
}

export class ApiConstantsEditRequest {
  result: string;
}

export class ApiConstantsEditResource {
}
