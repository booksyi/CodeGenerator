import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ConstantsListComponent } from './constants-list/constants-list.component';
import { ConstantsEditComponent } from './constants-edit/constants-edit.component';

const routes: Routes = [
  {
    path: 'constants', children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: 'list', component: ConstantsListComponent },
      { path: 'create', component: ConstantsEditComponent },
      { path: 'edit/:id', component: ConstantsEditComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConstantsRoutingModule { }
