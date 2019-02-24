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

  codeTemplate: CodeTemplate = {
    inputs: [],
    templateNodes: []
  };

  addInput() {
    this.codeTemplate.inputs.push(new Input());
  }

  removeInput(index: number) {
    this.codeTemplate.inputs.splice(index, 1);
  }

  addProperty(input: Input) {
    if (input.children == null) {
      input.children = [];
    }
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
    if (template.requestNodes == null) {
      template.requestNodes = [];
    }
    let requestNode = new RequestNode();
    requestNode.from = "value";
    template.requestNodes.push(requestNode);
  }

  removeTemplateRequest(template: TemplateNode, index: number) {
    template.requestNodes.splice(index, 1);
  }

  addAdapterRequest(adapter: AdapterNode) {
    if (adapter.requestNodes == null) {
      adapter.requestNodes = [];
    }
    let requestNode = new RequestNode();
    requestNode.from = "value";
    adapter.requestNodes.push(requestNode);
  }

  removeAdapterRequest(adapter: AdapterNode, index: number) {
    adapter.requestNodes.splice(index, 1);
  }

  addAdapter(template: TemplateNode) {
    if (template.adapterNodes == null) {
      template.adapterNodes = [];
    }
    let adapterNode = new AdapterNode();
    adapterNode.httpMethod = "get";
    template.adapterNodes.push(adapterNode);
  }

  removeAdapter(template: TemplateNode, index: number) {
    template.adapterNodes.splice(index, 1);
  }

  addParameter(template: TemplateNode) {
    if (template.parameterNodes == null) {
      template.parameterNodes = [];
    }
    let parameterNode = new ParameterNode();
    parameterNode.from = "value";
    parameterNode.templateNode = new TemplateNode();
    template.parameterNodes.push(parameterNode);
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
      console.log(err)
    });
  }

  back() {
    this.router.navigate(['generators/list']);
  }
}

export class CodeTemplate {
  public inputs: Input[];
  public templateNodes: TemplateNode[];
}

export class Input {
  public name: string;
  public description: string;
  public isMultiple: boolean;
  public isSplit: boolean;
  public defaultValues: string[];
  public children: Input[];
}

export class RequestNode {
  public name: string;
  public from: string; // enum
  public value: string;
  public inputName: string;
  public inputProperty: string;
  public adapterName: string;
  public adapterProperty: string;
}

export class ParameterNode {
  public name: string;
  public from: string; // enum
  public value: string;
  public inputName: string;
  public inputProperty: string;
  public adapterName: string;
  public adapterProperty: string;
  public templateNode: TemplateNode;
}

export class AdapterNode {
  public name: string;
  public httpMethod: string; // enum
  public url: string;
  public requestNodes: RequestNode[];
  public responseConfine: string;
  public isSplit: boolean;
}

export class TemplateNode {
  public url: string;
  public requestNodes: RequestNode[];
  public adapterNodes: AdapterNode[];
  public parameterNodes: ParameterNode[];
}
