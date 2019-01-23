import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GenerateCsApiComponent } from './code-generator/generate-cs-api/generate-cs-api.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';

const routes: Routes = [
  { path: 'generate-cs-api', component: GenerateCsApiComponent },
  { path: 'templates/list', component: TemplatesListComponent },
  { path: 'templates/create', component: TemplatesCreateComponent },
  { path: 'templates/edit/:id', component: TemplatesEditComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
