import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GeneratorsRoutingModule } from './generators-routing.module';
import { GeneratorsListComponent } from './generators-list/generators-list.component';
import { GeneratorsEditComponent } from './generators-edit/generators-edit.component';
import { GeneratorsGenerateComponent } from './generators-generate/generators-generate.component';

@NgModule({
  declarations: [
    GeneratorsEditComponent,
    GeneratorsListComponent,
    GeneratorsGenerateComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    NgbModule.forRoot(),
    GeneratorsRoutingModule
  ]
})
export class GeneratorsModule { }
