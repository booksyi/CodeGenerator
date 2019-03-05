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
