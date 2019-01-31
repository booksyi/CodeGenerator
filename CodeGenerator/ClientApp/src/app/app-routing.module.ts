import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TempApiListComponent } from './code-generator/temp-api/list/list.component';
import { TempApiCreateComponent } from './code-generator/temp-api/create/create.component';
import { TempApiEditComponent } from './code-generator/temp-api/edit/edit.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';

const routes: Routes = [
  { path: 'temp-api/list', component: TempApiListComponent },
  { path: 'temp-api/create', component: TempApiCreateComponent },
  { path: 'temp-api/edit/:id', component: TempApiEditComponent },
  { path: 'templates/list', component: TemplatesListComponent },
  { path: 'templates/create', component: TemplatesCreateComponent },
  { path: 'templates/edit/:id', component: TemplatesEditComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
