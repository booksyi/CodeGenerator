import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TemplatesRoutingModule } from './templates-routing.module';
import { TemplatesListComponent } from './templates-list/templates-list.component';
import { TemplatesEditComponent } from './templates-edit/templates-edit.component';

@NgModule({
  declarations: [
    TemplatesListComponent,
    TemplatesEditComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    NgbModule.forRoot(),
    TemplatesRoutingModule
  ]
})
export class TemplatesModule { }
