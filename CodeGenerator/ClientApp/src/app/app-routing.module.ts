import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GenerateCsApiComponent } from './code-generator/generate-cs-api/generate-cs-api.component';

const routes: Routes = [
  { path: 'generate-cs-api', component: GenerateCsApiComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
