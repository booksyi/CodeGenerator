import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { buildQueryParams } from '@app/lib';

@Component({
  selector: 'app-generate-cs-api',
  templateUrl: './generate-cs-api.component.html',
  //styleUrls: ['./.component.scss']
})
export class GenerateCsApiComponent implements OnInit {
  constructor(@Inject(HttpClient) private http: HttpClient) { }

  ngOnInit() {
  }

  request: GenerateCsApiRequest = {
    projectName: "CoreWebFuntions",
    connectionString: "Server=LAPTOP-RD9P71LP\\SQLEXPRESS;Database=TIP_TEST;UID=sa;PWD=1234;",
    tableNames: ["Article", "Epaper", "Member"]
  };

  response: any[];
  jsonResponse: string;

  addTableNameClick() {
    this.request.tableNames.push("");
  }

  generateCsApiClick() {
    this.generateCsApi(this.request);
  }

  generateCsApi(query: GenerateCsApiRequest) {
    this.http.get<any[]>(
      '/api/generators/generateCsApi' + buildQueryParams(query)
    ).subscribe(res =>
      {
        this.response = res;
        this.jsonResponse = JSON.stringify(res);
      });
  }
}

export class GenerateCsApiRequest {
  projectName: string;
  connectionString: string;
  tableNames: string[];
}

export class GeneratorsResource {
  name: string;
  text: string;
  children: GeneratorsResource[];
}
