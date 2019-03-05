import { Component, OnInit, Inject } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-generators-edit',
  templateUrl: './generators-edit.component.html',
  styleUrls: ['./generators-edit.component.scss']
})
export class GeneratorsEditComponent implements OnInit {
  constructor(@Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public route: ActivatedRoute,
    public router: Router) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.id > 0) {
      this.get();
    }
  }

  public id: number;
  generator: Generator = new Generator();
  templates: Template[];

  get() {
    this.http.get<Generator>(
      '/api/generators/' + this.id
    ).subscribe(generator => {
      this.generator = generator;
      this.http.get<CodeTemplate>(
        '/api/generators/' + this.id + '/codeTemplate'
      ).subscribe(codeTemplate => {
        this.generator.codeTemplate = codeTemplate;
      });
    });
  }

  getTemplates() {
    this.http.get<Template[]>(
      '/api/templates'
    ).subscribe(templates => {
      this.templates = templates.map(x => Object.assign(new Template(), x));
    });
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

  addInput() {
    this.generator.codeTemplate.inputs.push(new Input());
  }

  removeInput(input: Input) {
    let index = this.generator.codeTemplate.inputs.indexOf(input, 0);
    this.generator.codeTemplate.inputs.splice(index, 1);
  }

  addProperty(input: Input) {
    if (input.children == null) {
      input.children = [];
    }
    input.children.push(new Input());
  }

  removeProperty(parent: Input, input: Input) {
    let index = parent.children.indexOf(input, 0);
    parent.children.splice(index, 1);
  }

  addTemplate() {
    this.generator.codeTemplate.templateNodes.push(new TemplateNode());
  }

  removeTemplate(template: TemplateNode) {
    let index = this.generator.codeTemplate.templateNodes.indexOf(template, 0);
    this.generator.codeTemplate.templateNodes.splice(index, 1);
  }

  addTemplateRequest(template: TemplateNode) {
    template.requestNodes.push(new RequestNode());
  }

  removeTemplateRequest(template: TemplateNode, request: RequestNode) {
    let index = template.requestNodes.indexOf(request, 0);
    template.requestNodes.splice(index, 1);
  }

  addAdapterRequest(adapter: AdapterNode) {
    adapter.requestNodes.push(new RequestNode());
  }

  removeAdapterRequest(adapter: AdapterNode, request: RequestNode) {
    let index = adapter.requestNodes.indexOf(request, 0);
    adapter.requestNodes.splice(index, 1);
  }

  addAdapter(template: TemplateNode) {
    template.adapterNodes.push(new AdapterNode());
  }

  removeAdapter(template: TemplateNode, adapter: AdapterNode) {
    let index = template.adapterNodes.indexOf(adapter, 0);
    template.adapterNodes.splice(index, 1);
  }

  addParameter(template: TemplateNode) {
    template.parameterNodes.push(new ParameterNode());
  }

  removeParameter(template: TemplateNode, parameter: ParameterNode) {
    let index = template.parameterNodes.indexOf(parameter, 0);
    template.parameterNodes.splice(index, 1);
  }

  create() {
    this.http.post<any>(
      '/api/generators', this.generator
    ).subscribe(() => {
      this.back();
    }, err => {
      console.log(err);
    });
  }

  edit() {
    this.http.put<any>(
      '/api/generators/' + this.id, this.generator
    ).subscribe(() => {
      this.back();
    }, err => {
      console.log(err);
    });
  }

  back() {
    this.router.navigate(['generators/list']);
  }

}

class BsModal {
  public content: any;
  public data: any;
}

export class Generator {
  public name: string;
  public json: string;
  public codeTemplate: CodeTemplate = new CodeTemplate();
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
  public name: string;
  public url: string;
  public requestNodes: RequestNode[] = [];
  public adapterNodes: AdapterNode[] = [];
  public parameterNodes: ParameterNode[] = [];
}

export class Template {
  public id: number;
  public name: string;
  public get url(): string {
    return "/api/templates/" + this.id + "/content";
  }
}

class Guid {
  static newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
