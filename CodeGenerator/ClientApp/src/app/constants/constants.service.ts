import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConstantsService {
  constructor(
    @Inject(HttpClient)
    private http: HttpClient,
    private router: Router) { }

  list(): Observable<Constant[]> {
    return this.http.get<Constant[]>(
      '/api/constants'
    );
  }

  get(id: number): Observable<Constant> {
    return this.http.get<Constant>(
      '/api/constants/' + id
    );
  }

  create(constant: Constant): Observable<Constant> {
    return this.http.post<Constant>(
      '/api/constants', constant
    );
  }

  update(id: number, constant: Constant): Observable<Constant> {
    return this.http.put<Constant>(
      '/api/constants/' + id, constant
    );
  }

  delete(id: number): Observable<any> {
    return this.http.delete(
      '/api/constants/' + id
    );
  }

  redirectToList() {
    this.router.navigate(['constants/list']);
  }

  redirectToCreate() {
    this.router.navigate(['constants/create']);
  }

  redirectToEdit(id: number) {
    this.router.navigate(['constants/edit/' + id]);
  }
}

export class Constant {
  id: number;
  result: string;
}
