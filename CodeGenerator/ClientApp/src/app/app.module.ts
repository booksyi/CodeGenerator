import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ConstantsListComponent } from './code-generator/constants/list/list.component';
import { ConstantsCreateComponent } from './code-generator/constants/create/create.component';
import { ConstantsEditComponent } from './code-generator/constants/edit/edit.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';
import { GeneratorsListComponent } from './code-generator/generators/list/list.component';
import { GeneratorsEditComponent } from './code-generator/generators/edit/edit.component';
import { GeneratorsGenerateComponent } from './code-generator/generators/generate/generate.component';

@NgModule({
  declarations: [
    AppComponent,
    ConstantsListComponent,
    ConstantsCreateComponent,
    ConstantsEditComponent,
    TemplatesListComponent,
    TemplatesCreateComponent,
    TemplatesEditComponent,
    GeneratorsListComponent,
    GeneratorsEditComponent,
    GeneratorsGenerateComponent
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
