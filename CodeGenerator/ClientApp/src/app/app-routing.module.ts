import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConstantsListComponent } from './code-generator/constants/list/list.component';
import { ConstantsCreateComponent } from './code-generator/constants/create/create.component';
import { ConstantsEditComponent } from './code-generator/constants/edit/edit.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';
import { GeneratorsListComponent } from './code-generator/generators/list/list.component';
import { GeneratorsEditComponent } from './code-generator/generators/edit/edit.component';
import { GeneratorsGenerateComponent } from './code-generator/generators/generate/generate.component';

const routes: Routes = [
  { path: 'constants', redirectTo: '/constants/list', pathMatch: 'full' },
  { path: 'constants/list', component: ConstantsListComponent },
  { path: 'constants/create', component: ConstantsCreateComponent },
  { path: 'constants/edit/:id', component: ConstantsEditComponent },
  { path: 'templates', redirectTo: '/templates/list', pathMatch: 'full' },
  { path: 'templates/list', component: TemplatesListComponent },
  { path: 'templates/create', component: TemplatesCreateComponent },
  { path: 'templates/edit/:id', component: TemplatesEditComponent },
  { path: 'generators', redirectTo: '/generators/list', pathMatch: 'full' },
  { path: 'generators/list', component: GeneratorsListComponent },
  { path: 'generators/edit', component: GeneratorsEditComponent },
  { path: 'generators/edit/:id', component: GeneratorsEditComponent },
  { path: 'generators/generate/:id', component: GeneratorsGenerateComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
