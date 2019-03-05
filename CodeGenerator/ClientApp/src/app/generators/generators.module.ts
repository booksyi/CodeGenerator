import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { GeneratorsRoutingModule } from './generators-routing.module';
import { GeneratorsListComponent } from './generators-list/generators-list.component';
import { GeneratorsEditComponent } from './generators-edit/generators-edit.component';
import { GeneratorsGenerateComponent } from './generators-generate/generators-generate.component';

@NgModule({
  declarations: [GeneratorsEditComponent, GeneratorsListComponent, GeneratorsGenerateComponent],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    GeneratorsRoutingModule
  ]
})
export class GeneratorsModule { }
