import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ApiGeneratorComponent } from './api-generator/api-generator.component';

const routes: Routes = [
  { path: 'api-generator', component: ApiGeneratorComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
