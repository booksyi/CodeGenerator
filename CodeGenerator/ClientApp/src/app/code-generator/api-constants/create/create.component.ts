import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-api-constants-create',
    templateUrl: './create.component.html',
    styleUrls: ['./create.component.scss']
})
/** create component*/
export class ApiConstantsCreateComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
  }

  request: ApiConstantsCreateRequest = {
    result: null
  };
  resources: ApiConstantsCreateResource;

  create() {
    this.http.post<ApiConstantsCreateResource>(
      '/api/apiConstants', this.request
    ).subscribe(res => {
      this.resources = res;
      this.back();
    });
  }

  back() {
    this.router.navigate(['api-constants/list']);
  }
}

export class ApiConstantsCreateRequest {
  result: string;
}

export class ApiConstantsCreateResource {
}
