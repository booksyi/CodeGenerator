import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ApiGeneratorComponent } from './api-generator/api-generator.component';

@NgModule({
  declarations: [
    AppComponent,
    ApiGeneratorComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent, ApiGeneratorComponent]
})
export class AppModule { }
