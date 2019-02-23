import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-generate',
    templateUrl: './generate.component.html',
    styleUrls: ['./generate.component.scss']
})
/** generate component*/
export class GeneratorsGenerateComponent {
  /** generate ctor */
  constructor(@Inject(HttpClient) private http: HttpClient,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.get(this.id);
  }

  inputs: Input[];
  resources: GenerateResource[];

  get(id: number) {
    this.http.get<Input[]>(
      '/api/codeTemplates/' + id + '/inputs'
    ).subscribe(res => {
      this.inputs = res;
      for (let input of this.inputs) {
        this.setDefaultValues(input);
      }
    });
  }

  setDefaultValues(input: Input) {
    if (input.children && input.children.length) {
      for (let child of input.children) {
        this.setDefaultValues(child);
      }
    }
    else {
      input.values = [''];
    }
  }

  trackByFn(index: any, item: any) {
    return index;
  }

  add(input: Input) {
    input.values.push('');
  }

  remove(input: Input, index: number) {
    input.values.splice(index, 1);
  }

  submit() {
    let query = new Dictionary();
    for (let input of this.inputs) {
      query[input.name] = input.values;
    }
    this.http.post<GenerateResource[]>(
      '/api/generators/' + this.id + '/generate', query
    ).subscribe(res => {
      this.resources = res;
    });
  }

  rollback() {
    this.resources = null;
  }

  back() {
    this.router.navigate(['generators/list']);
  }
}

class Dictionary {
  [index: string]: string[];
}

class Input {
  public name: string;
  public description: string;
  public isMultiple: boolean;
  public children: Input[];
  public values: string[];
  //public type: string;
  //public regex: string;
}

class GenerateResource {
  public path: string;
  public text: string;
}
