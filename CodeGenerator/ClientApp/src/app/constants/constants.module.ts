import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ConstantsRoutingModule } from './constants-routing.module';
import { ConstantsListComponent } from './constants-list/constants-list.component';
import { ConstantsEditComponent } from './constants-edit/constants-edit.component';

@NgModule({
  declarations: [ConstantsListComponent, ConstantsEditComponent],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ConstantsRoutingModule
  ]
})
export class ConstantsModule { }
