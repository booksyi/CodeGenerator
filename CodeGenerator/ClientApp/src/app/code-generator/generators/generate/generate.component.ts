import { Component, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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
    private modalService: NgbModal,
    public route: ActivatedRoute,
    public router: Router) { }

  public id: number;
  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.get(this.id);
  }

  bsModal: any;
  bsModalStack: BsModal[] = [];
  bsModalEventDismiss: boolean = false;
  openModal(content, data) {
    let component = this;
    component.bsModal = data;
    component.bsModalStack.push({ content: content, data: data });
    if (this.modalService.hasOpenModals()) {
      this.bsModalEventDismiss = true;
      this.modalService.dismissAll();
    }
    this.modalService.open(content, { size: "lg" }).result.finally(function () {
      if (component.bsModalEventDismiss) {
        component.bsModalEventDismiss = false;
      }
      else {
        component.bsModalStack.splice(-1, 1);
        component.bsModal = null;
        if (component.bsModalStack.length) {
          let lastModal = component.bsModalStack.pop();
          component.openModal(lastModal.content, lastModal.data);
        }
      }
    });
  }

  trackByFn(index: any, item: any) {
    return index;
  }

  inputs: Input[];
  resources: GenerateResource[];

  get(id: number) {
    this.http.get<Input[]>(
      '/api/generators/' + id + '/inputs'
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
      if (input.children && input.children.length) {
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

  toJson(inputs: Input[]): string {
    return JSON.stringify(this.toJObject(inputs));
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

class BsModal {
  public content: any;
  public data: any;
}

class JObject {
  [index: string]: JObject[] | JObject | string[] | string;
}
type InputObject = Input[]

class Input {
  public name: string;
  public description: string;
  public isRequired: boolean;
  public isMultiple: boolean;
  public children: Input[];
  public values: InputObject[] | string[];
  //public type: string;
  //public regex: string;
}

class GenerateResource {
  public name: string;
  public text: string;
}
