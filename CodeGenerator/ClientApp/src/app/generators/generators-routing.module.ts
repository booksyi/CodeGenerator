import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GeneratorsListComponent } from './generators-list/generators-list.component';
import { GeneratorsEditComponent } from './generators-edit/generators-edit.component';
import { GeneratorsGenerateComponent } from './generators-generate/generators-generate.component';

const routes: Routes = [
  {
    path: 'generators', children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: 'list', component: GeneratorsListComponent },
      { path: 'create', component: GeneratorsEditComponent },
      { path: 'edit/:id', component: GeneratorsEditComponent },
      { path: 'generate/:id', component: GeneratorsGenerateComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GeneratorsRoutingModule { }
