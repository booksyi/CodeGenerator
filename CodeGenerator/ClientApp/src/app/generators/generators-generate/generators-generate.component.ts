import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModalStack } from '@app/shared/ng-bootstrap-custom.service';
import { GeneratorsService, GenerateResource } from '../generators.service';
import { Guid } from '@app/shared/guid';
import { trackByFn } from '@app/shared/functions';

@Component({
  selector: 'app-generators-generate',
  templateUrl: './generators-generate.component.html',
  styleUrls: ['./generators-generate.component.scss']
})
export class GeneratorsGenerateComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private modalStackService: NgbModalStack,
    private service: GeneratorsService) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.get();
  }

  id: number;
  inputs: Input[];
  resources: GenerateResource[];
  get currentModal(): any {
    return this.modalStackService.data;
  }

  trackByFn = trackByFn;

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

  toJObject(inputs: Input[]): JObject {
    let jObject = new JObject();
    for (let input of inputs) {
      if (input.children && input.children.length) {
        var objectValues: JObject[] = [];
        for (let valueInputs of input.values) {
          let args: Input[] = JSON.parse(JSON.stringify(valueInputs));
          objectValues.push(this.toJObject(args));
        }
        if (input.isMultiple) {
          jObject[input.name] = objectValues;
        }
        else if (objectValues.length) {
          jObject[input.name] = objectValues[0];
        }
      }
      else {
        var stringValues: string[] = [];
        for (let valueInputs of input.values) {
          stringValues.push(valueInputs.toString());
        }
        if (input.isMultiple) {
          jObject[input.name] = stringValues;
        }
        else if (stringValues.length) {
          jObject[input.name] = stringValues[0];
        }
      }
    }
    return jObject;
  }

  toJson(inputs: Input[]): string {
    return JSON.stringify(this.toJObject(inputs));
  }

  add(input: Input) {
    let values: InputObject[] | string[] = this.defaultValues(input);
    Array.prototype.push.apply(input.values, values);
  }

  remove(input: Input, index: number) {
    input.values.splice(index, 1);
  }

  get() {
    this.service.getInputs(this.id).subscribe(inputs => {
      if (inputs) {
        this.inputs = inputs.map(input => Object.assign(new Input(), input));
        for (let input of this.inputs) {
          input.values = this.defaultValues(input);
        }
      }
      else {
        this.submit();
      }
    });
  }

  open(content, data) {
    this.modalStackService.open(content, data);
  }

  submit() {
    let jObject = this.inputs ? this.toJObject(this.inputs) : {};
    this.service.generate(this.id, jObject).subscribe(resources => {
      this.resources = resources;
    });
  }

  rollback() {
    this.resources = null;
    if (!this.inputs) {
      this.submit();
    }
  }

  back() {
    this.service.redirectToList();
  }
}

class JObject {
  [index: string]: JObject[] | JObject | string[] | string;
}
type InputObject = Input[]

class Input {
  guid: string = Guid.newGuid();
  name: string;
  displayName: string;
  inputType: "textbox" | "textarea" | "truefalse" = "textbox";
  isRequired: boolean;
  isMultiple: boolean;
  children: Input[];
  values: InputObject[] | string[];
}
