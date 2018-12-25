import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-api-generator',
  templateUrl: './api-generator.component.html'
})
export class ApiGeneratorComponent {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  public generateResponse: GenerateResponse;
  generate(request: GenerateRequest) {
    this.http.get<GenerateResponse>(this.baseUrl + 'api/ApiGenerator/Generate' + '?actionName=' + request.actionName).subscribe(result => {
      this.generateResponse = result;
    }, error => console.error(error));
  }
}

class GenerateRequest {
  actionName: string;
  constructor(actionName: string) {
    this.actionName = actionName;
  }
}

interface GenerateResponse {
  result: string;
}
