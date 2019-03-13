import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Guid } from '@app/shared/guid';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GeneratorsService {
  constructor(
    @Inject(HttpClient)
    private http: HttpClient,
    private router: Router) { }

  list(): Observable<Generator[]> {
    return this.http.get<Generator[]>(
      '/api/generators'
    ).pipe(map(generators => {
      for (let generator of generators) {
        this.getTemplates(generator.id)
          .subscribe(templates => generator.templates = templates);
      }
      return generators;
    }));
  }

  get(id: number): Observable<Generator> {
    return this.http.get<Generator>(
      '/api/generators/' + id
    ).pipe(map(generator => {
      this.getCodeTemplate(generator.id)
        .subscribe(codeTemplate => generator.codeTemplate = codeTemplate);
      return generator;
    }));
  }

  getCodeTemplate(id: number): Observable<CodeTemplate> {
    return this.http.get<CodeTemplate>(
      '/api/generators/' + id + '/codeTemplate'
    );
  }

  getTemplates(id: number): Observable<string[]> {
    return this.http.get<string[]>(
      '/api/generators/' + id + '/templates'
    );
  }

  getInputs(id: number): Observable<Input[]> {
    return this.http.get<Input[]>(
      '/api/generators/' + id + '/inputs'
    );
  }

  create(generator: Generator): Observable<Generator> {
    return this.http.post<Generator>(
      '/api/generators', generator
    );
  }

  update(id: number, generator: Generator): Observable<Generator> {
    return this.http.put<any>(
      '/api/generators/' + id, generator
    );
  }

  delete(id: number) {
    return this.http.delete(
      '/api/generators/' + id
    );
  }

  generate(id: number, jObject: any) {
    return this.http.post<GenerateResource[]>(
      '/api/generators/' + id + '/generate', jObject
    );
  }

  redirectToList() {
    this.router.navigate(['generators/list']);
  }

  redirectToCreate() {
    this.router.navigate(['generators/create']);
  }

  redirectToEdit(id: number) {
    this.router.navigate(['generators/edit/' + id]);
  }

  redirectToGenerate(id: number) {
    this.router.navigate(['generators/generate/' + id]);
  }
}

export class Generator {
  id: number;
  name: string;
  json: string;
  codeTemplate: CodeTemplate = new CodeTemplate();
  templates: string[];
}

export class CodeTemplate {
  inputs: Input[] = [];
  templateNodes: TemplateNode[] = [];
}

export class Input {
  name: string;
  description: string;
  isRequired: boolean;
  isMultiple: boolean;
  isSplit: boolean;
  defaultValues: string[];
  children: Input[] = [];
}

export class RequestNode {
  guid: string = Guid.newGuid();
  name: string;
  from: "value" | "input" | "adapter" = "value";
  value: string;
  inputName: string;
  inputProperty: string;
  adapterName: string;
  adapterProperty: string;
}

export class ParameterNode {
  guid: string = Guid.newGuid();
  name: string;
  from: "value" | "input" | "adapter" | "template" = "value";
  value: string;
  inputName: string;
  inputProperty: string;
  adapterName: string;
  adapterProperty: string;
  templateNode: TemplateNode = new TemplateNode();
}

export class AdapterNode {
  guid: string = Guid.newGuid();
  name: string;
  httpMethod: "get" | "post" = "get";
  url: string;
  requestNodes: RequestNode[] = [];
  responseConfine: string;
  isSplit: boolean;
}

export class TemplateNode {
  name: string;
  url: string;
  requestNodes: RequestNode[] = [];
  adapterNodes: AdapterNode[] = [];
  parameterNodes: ParameterNode[] = [];
}

export class Template {
  id: number;
  name: string;
  get url(): string {
    return "/api/templates/" + this.id + "/content";
  }
}

export class GenerateResource {
  name: string;
  text: string;
}
