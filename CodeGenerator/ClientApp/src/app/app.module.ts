import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ApiConstantsListComponent } from './code-generator/api-constants/list/list.component';
import { ApiConstantsCreateComponent } from './code-generator/api-constants/create/create.component';
import { ApiConstantsEditComponent } from './code-generator/api-constants/edit/edit.component';
import { TemplatesListComponent } from './code-generator/templates/list/list.component';
import { TemplatesCreateComponent } from './code-generator/templates/create/create.component';
import { TemplatesEditComponent } from './code-generator/templates/edit/edit.component';
import { GeneratorsListComponent } from './code-generator/generators/list/list.component';
import { GeneratorsCreateComponent } from './code-generator/generators/create/create.component';
import { GeneratorsEditComponent } from './code-generator/generators/edit/edit.component';
import { GeneratorsGenerateComponent } from './code-generator/generators/generate/generate.component';

@NgModule({
  declarations: [
    AppComponent,
    ApiConstantsListComponent,
    ApiConstantsCreateComponent,
    ApiConstantsEditComponent,
    TemplatesListComponent,
    TemplatesCreateComponent,
    TemplatesEditComponent,
    GeneratorsListComponent,
    GeneratorsCreateComponent,
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
