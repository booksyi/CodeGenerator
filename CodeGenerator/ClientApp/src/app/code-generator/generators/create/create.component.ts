import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-generators-create',
    templateUrl: './create.component.html',
    styleUrls: ['./create.component.scss']
})
/** create component*/
export class GeneratorsCreateComponent {
  /** create ctor */
  constructor(@Inject(HttpClient) private http: HttpClient,
    public router: Router) { }

  codeTemplate: CodeTemplate = new CodeTemplate();

  addInput() {
    this.codeTemplate.inputs.push(new Input());
  }

  removeInput(index: number) {
    this.codeTemplate.inputs.splice(index, 1);
  }

  addProperty(input: Input) {
    input.children.push(new Input());
  }

  removeProperty(input: Input, index: number) {
    input.children.splice(index, 1);
  }

  addTemplate() {
    this.codeTemplate.templateNodes.push(new TemplateNode());
  }

  removeTemplate(index: number) {
    this.codeTemplate.templateNodes.splice(index, 1);
  }

  addTemplateRequest(template: TemplateNode) {
    template.requestNodes.push(new RequestNode());
  }

  removeTemplateRequest(template: TemplateNode, index: number) {
    template.requestNodes.splice(index, 1);
  }

  addAdapterRequest(adapter: AdapterNode) {
    adapter.requestNodes.push(new RequestNode());
  }

  removeAdapterRequest(adapter: AdapterNode, index: number) {
    adapter.requestNodes.splice(index, 1);
  }

  addAdapter(template: TemplateNode) {
    template.adapterNodes.push(new AdapterNode());
  }

  removeAdapter(template: TemplateNode, index: number) {
    template.adapterNodes.splice(index, 1);
  }

  addParameter(template: TemplateNode) {
    template.parameterNodes.push(new ParameterNode());
  }

  removeParameter(template: TemplateNode, index: number) {
    template.parameterNodes.splice(index, 1);
  }

  create() {
    this.http.post<any>(
      '/api/codeTemplates', this.codeTemplate
    ).subscribe(res => {
      this.back();
    }, err => {
      console.log(err);
    });
  }

  back() {
    this.router.navigate(['generators/list']);
  }
}

export class CodeTemplate {
  public inputs: Input[] = [];
  public templateNodes: TemplateNode[] = [];
}

export class Input {
  public name: string;
  public description: string;
  public isRequired: boolean;
  public isMultiple: boolean;
  public isSplit: boolean;
  public defaultValues: string[];
  public children: Input[] = [];
}

export class RequestNode {
  public guid: string = Guid.newGuid();
  public name: string;
  public from: "value" | "input" | "adapter" = "value";
  public value: string;
  public inputName: string;
  public inputProperty: string;
  public adapterName: string;
  public adapterProperty: string;
}

export class ParameterNode {
  public guid: string = Guid.newGuid();
  public name: string;
  public from: "value" | "input" | "adapter" | "template" = "value";
  public value: string;
  public inputName: string;
  public inputProperty: string;
  public adapterName: string;
  public adapterProperty: string;
  public templateNode: TemplateNode = new TemplateNode();
}

export class AdapterNode {
  public guid: string = Guid.newGuid();
  public name: string;
  public httpMethod: "get" | "post" = "get";
  public url: string;
  public requestNodes: RequestNode[] = [];
  public responseConfine: string;
  public isSplit: boolean;
}

export class TemplateNode {
  public url: string;
  public requestNodes: RequestNode[] = [];
  public adapterNodes: AdapterNode[] = [];
  public parameterNodes: ParameterNode[] = [];
}

class Guid {
  static newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
