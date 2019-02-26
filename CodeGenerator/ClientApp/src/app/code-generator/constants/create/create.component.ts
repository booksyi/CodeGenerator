import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-constants-create',
    templateUrl: './create.component.html',
    styleUrls: ['./create.component.scss']
})
/** create component*/
export class ConstantsCreateComponent {
  constructor(
    @Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  ngOnInit() {
  }

  request: ConstantsCreateRequest = {
    result: null
  };
  resources: ConstantsCreateResource;

  create() {
    this.http.post<ConstantsCreateResource>(
      '/api/constants', this.request
    ).subscribe(res => {
      this.resources = res;
      this.back();
    });
  }

  back() {
    this.router.navigate(['constants/list']);
  }
}

export class ConstantsCreateRequest {
  result: string;
}

export class ConstantsCreateResource {
}
