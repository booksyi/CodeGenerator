import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ApiConstantsListComponent } from './code-generator/api-constants/list/list.component';
import { ApiConstantsCreateComponent } from './code-generator/api-constants/create/create.component';
import { ApiConstantsEditComponent } from './code-generator/api-constants/edit/edit.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';
import { GeneratorsListComponent } from './code-generator/generators/list/list.component';
import { GeneratorsGenerateComponent } from './code-generator/generators/generate/generate.component';

const routes: Routes = [
  { path: 'api-constants', redirectTo: '/api-constants/list', pathMatch: 'full' },
  { path: 'api-constants/list', component: ApiConstantsListComponent },
  { path: 'api-constants/create', component: ApiConstantsCreateComponent },
  { path: 'api-constants/edit/:id', component: ApiConstantsEditComponent },
  { path: 'templates', redirectTo: '/templates/list', pathMatch: 'full' },
  { path: 'templates/list', component: TemplatesListComponent },
  { path: 'templates/create', component: TemplatesCreateComponent },
  { path: 'templates/edit/:id', component: TemplatesEditComponent },
  { path: 'generators', redirectTo: '/generators/list', pathMatch: 'full' },
  { path: 'generators/list', component: GeneratorsListComponent },
  { path: 'generators/generate/:id', component: GeneratorsGenerateComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
