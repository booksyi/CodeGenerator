import { Component, OnInit } from '@angular/core';
import { ApiGeneratorService } from './api-generator.service';
@Component({
  selector: 'app-api-generator',
  templateUrl: './api-generator.component.html'
})
export class ApiGeneratorComponent implements OnInit {
  constructor(
    private apiGeneratorService: ApiGeneratorService
  ) { }

  public result: string;
  generateModel(actionName: string) {
    this.apiGeneratorService.generateModel({ actionName }).subscribe(
      result => { this.result = result.value }
    );
  }
}
