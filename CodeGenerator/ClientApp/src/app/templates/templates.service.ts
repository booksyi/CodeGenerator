import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TemplatesService {
  constructor(
    @Inject(HttpClient)
    private http: HttpClient,
    private router: Router) { }

  list(): Observable<Template[]> {
    return this.http.get<Template[]>(
      '/api/templates'
    );
  }

  get(id: number): Observable<Template> {
    return this.http.get<Template>(
      '/api/templates/' + id
    );
  }

  create(template: Template): Observable<Template> {
    return this.http.post<Template>(
      '/api/templates', template
    );
  }

  update(id: number, template: Template): Observable<Template> {
    return this.http.put<Template>(
      '/api/templates/' + id, template
    );
  }

  delete(id: number): Observable<any> {
    return this.http.delete(
      '/api/templates/' + id
    );
  }

  redirectToList() {
    this.router.navigate(['templates/list']);
  }

  redirectToCreate() {
    this.router.navigate(['templates/create']);
  }

  redirectToEdit(id: number) {
    this.router.navigate(['templates/edit/' + id]);
  }
}

export class Template {
  id: number;
  name: string;
  content: string;
}
