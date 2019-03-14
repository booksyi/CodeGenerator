import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModalStack } from '@app/shared/ng-bootstrap-custom.service';
import { TemplatesService } from '@app/templates/templates.service'
import {
  GeneratorsService,
  Generator,
  Input,
  RequestNode,
  ParameterNode,
  AdapterNode,
  TemplateNode,
  Template
} from '../generators.service';

@Component({
  selector: 'app-generators-edit',
  templateUrl: './generators-edit.component.html',
  styleUrls: ['./generators-edit.component.scss']
})
export class GeneratorsEditComponent implements OnInit {
  constructor(
    private modalStackService: NgbModalStack,
    private route: ActivatedRoute,
    private service: GeneratorsService,
    private templatesService: TemplatesService) { }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    if (this.id > 0) {
      this.get();
    }
  }

  id: number;
  generator: Generator = new Generator();
  templates: Template[];
  get currentModal(): any {
    return this.modalStackService.data;
  }

  get() {
    this.service.get(this.id).subscribe(generator => this.generator = generator);
  }

  create() {
    this.service.create(this.generator).subscribe(() => this.back());
  }

  update() {
    this.service.update(this.id, this.generator).subscribe(() => this.back());
  }

  back() {
    this.service.redirectToList();
  }

  // 找出包含 RequestNode 一整串的 TemplateNode
  getRequestElders(request: RequestNode, elders: TemplateNode[] = null): TemplateNode[] {
    if (elders && elders.length) {
      let last = elders[elders.length - 1];
      if (last.requestNodes.concat(
        last.adapterNodes.some(x => x.requestNodes != null && x.requestNodes.length > 0) ?
          last.adapterNodes
            .map(x => x.requestNodes)
            .reduce((x, y) => x.concat(y)) : []).includes(request)) {
        return elders;
      }
      else if (last.parameterNodes.some(x => x.from === "template")) {
        return last.parameterNodes
          .map(x => this.getRequestElders(request, elders.concat(x.templateNode)))
          .reduce((x, y) => x.concat(y));
      }
      return [];
    }
    return this.generator.codeTemplate.templateNodes
      .map(x => this.getRequestElders(request, [x]))
      .reduce((x, y) => x.concat(y));
  }

  // 找出包含 ParameterNode 一整串的 TemplateNode
  getParameterElders(parameter: ParameterNode, elders: TemplateNode[] = null): TemplateNode[] {
    if (elders && elders.length) {
      let last = elders[elders.length - 1];
      if (last.parameterNodes.includes(parameter)) {
        return elders;
      }
      else if (last.parameterNodes.some(x => x.from === "template")) {
        return last.parameterNodes
          .map(x => this.getParameterElders(parameter, elders.concat(x.templateNode)))
          .reduce((x, y) => x.concat(y));
      }
      return [];
    }
    return this.generator.codeTemplate.templateNodes
      .map(x => this.getParameterElders(parameter, [x]))
      .reduce((x, y) => x.concat(y));
  }

  // 找出 RequestNode 或 ParameterNode 在執行階段可以取得資料的 AdapterNode
  getMergeAdapters(node: RequestNode | ParameterNode): AdapterNode[] {
    if (node instanceof RequestNode) {
      let elders = this.getRequestElders(node);
      if (elders.length) {
        let last = elders[elders.length - 1];
        let index = last.adapterNodes.findIndex(x => x.requestNodes.includes(node));
        if (index >= 0) {
          // 如果 RequestNode 是 AdapterNode 裡面的
          // 則只能看到同一個樣板在這個 AdapterNode 之前的 AdapterNode
          if (elders.length == 1) {
            return last.adapterNodes.filter((x, y) => y < index);
          }
          else {
            return elders.slice(0, -1).map(x => x.adapterNodes).concat(
              last.adapterNodes.filter((x, y) => y < index)
            ).reduce((x, y) => x.concat(y));
          }
        }
        return elders
          .map(x => x.adapterNodes)
          .reduce((x, y) => x.concat(y));
      }
    }
    else if (node instanceof ParameterNode) {
      return this.getParameterElders(node)
        .map(x => x.adapterNodes)
        .reduce((x, y) => x.concat(y));
    }
    return [];
  }

  getTemplates() {
    this.templatesService.list().subscribe(templates =>
      this.templates = templates.map(x => Object.assign(new Template(), x)));
  }

  open(content, data) {
    this.modalStackService.open(content, data);
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
}
