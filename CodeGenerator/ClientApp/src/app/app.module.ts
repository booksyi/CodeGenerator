import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TempApiListComponent } from './code-generator/temp-api/list/list.component';
import { TempApiCreateComponent } from './code-generator/temp-api/create/create.component';
import { TempApiEditComponent } from './code-generator/temp-api/edit/edit.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';

@NgModule({
  declarations: [
    AppComponent,
    TempApiListComponent,
    TempApiCreateComponent,
    TempApiEditComponent,
    TemplatesListComponent,
    TemplatesCreateComponent,
    TemplatesEditComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    NgbModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
