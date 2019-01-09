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
    tableNames: ["Article", "Epaper", "Member"],
  };

  identify(index, item) {
    return item.name;
  }

  resources: GenerateCsApiResource[];
  jsonResources: string;

  addTableNameClick() {
    this.request.tableNames.push("");
  }

  generateCsApiClick() {
    this.generateCsApi(this.request);
  }

  generateCsApi(query: GenerateCsApiRequest) {
    this.http.get<GenerateCsApiResource[]>(
      '/api/generators/generateCsApi' + buildQueryParams(query)
    ).subscribe(res =>
      {
      this.resources = res;
      this.jsonResources = JSON.stringify(res);
      });
  }

  openResource(path: string) {
    this.resources.filter(function (e, i) {
      return e.savePath === path;
    })[0].open = true;
  }
}

export class GenerateCsApiRequest {
  projectName: string;
  connectionString: string;
  tableNames: string[];
}

export class GenerateCsApiResource {
  savePath: string;
  tree: any;
  open: boolean = false;
}
