import { Component, Inject, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-generators-create',
    templateUrl: './create.component.html',
    styleUrls: ['./create.component.scss']
})
/** create component*/
export class GeneratorsCreateComponent {
  /** create ctor */
  constructor(@Inject(HttpClient) private http: HttpClient,
    private modalService: NgbModal,
    public router: Router) { }

  codeTemplate: CodeTemplate = new CodeTemplate();

  modalData: any;
  modalStack: ModalItem[] = [];
  modalTab: boolean = false;
  openModal(content, data) {
    let component = this;
    if (this.modalService.hasOpenModals()) {
      this.modalTab = true;
      this.modalService.dismissAll();
    }
    this.modalService.open(content).result.finally(function () {
      if (component.modalTab) {
        component.modalTab = false;
      }
      else {
        component.modalStack.splice(-1, 1);
        component.modalData = null;
        if (component.modalStack.length) {
          let lastModal = component.modalStack.pop();
          component.openModal(lastModal.content, lastModal.data);
        }
      }
    });
    this.modalData = data;
    this.modalStack.push({ content: content, data: data });
  }

  addInput() {
    this.codeTemplate.inputs.push(new Input());
  }

  removeInput(input: Input) {
    let index = this.codeTemplate.inputs.indexOf(input, 0);
    this.codeTemplate.inputs.splice(index, 1);
  }

  addProperty(input: Input) {
    input.children.push(new Input());
  }

  removeProperty(parent: Input, input: Input) {
    let index = parent.children.indexOf(input, 0);
    parent.children.splice(index, 1);
  }

  addTemplate() {
    this.codeTemplate.templateNodes.push(new TemplateNode());
  }

  removeTemplate(template: TemplateNode) {
    let index = this.codeTemplate.templateNodes.indexOf(template, 0);
    this.codeTemplate.templateNodes.splice(index, 1);
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

class ModalItem {
  public content: any;
  public data: any;
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
