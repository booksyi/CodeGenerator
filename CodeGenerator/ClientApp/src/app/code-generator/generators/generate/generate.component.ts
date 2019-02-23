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
        input.values = this.defaultValues(input);
      }
    });
  }

  defaultValues(input: Input): InputObject[] | string[] {
    if (input.children && input.children.length) {
      let valueInputs: InputObject = [];
      for (let child of input.children) {
        valueInputs.push(JSON.parse(JSON.stringify(child)));
        for (let valueInput of valueInputs) {
          valueInput.values = this.defaultValues(valueInput);
        }
      }
      return [valueInputs];
    }
    else {
      return [''];
    }
  }

  trackByFn(index: any, item: any) {
    return index;
  }
  
  add(input: Input) {
    let values: InputObject[] | string[] = this.defaultValues(input);
    Array.prototype.push.apply(input.values, values);
  }

  remove(input: Input, index: number) {
    input.values.splice(index, 1);
  }

  toJObject(inputs: Input[]): JObject {
    let jObject = new JObject();
    for (let input of inputs) {
      if (input.children) {
        var objectValues: JObject[] = [];
        for (let valueInputs of input.values) {
          let args: Input[] = JSON.parse(JSON.stringify(valueInputs));
          objectValues.push(this.toJObject(args));
        }
        jObject[input.name] = objectValues;
      }
      else {
        var stringValues: string[] = [];
        for (let valueInputs of input.values) {
          stringValues.push(valueInputs.toString());
        }
        jObject[input.name] = stringValues;
      }
    }
    return jObject;
  }

  submit() {
    let query = this.toJObject(this.inputs);
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

class JObject {
  [index: string]: JObject[] | JObject | string[] | string;
}
type InputObject = Input[]

class Input {
  public name: string;
  public description: string;
  public isMultiple: boolean;
  public children: Input[];
  public values: InputObject[] | string[];
  //public type: string;
  //public regex: string;
}

class GenerateResource {
  public path: string;
  public text: string;
}
