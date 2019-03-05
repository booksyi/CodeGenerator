import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TemplatesListComponent } from './templates-list/templates-list.component';
import { TemplatesEditComponent } from './templates-edit/templates-edit.component';

const routes: Routes = [
  {
    path: 'templates', children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: 'list', component: TemplatesListComponent },
      { path: 'create', component: TemplatesEditComponent },
      { path: 'edit/:id', component: TemplatesEditComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TemplatesRoutingModule { }
