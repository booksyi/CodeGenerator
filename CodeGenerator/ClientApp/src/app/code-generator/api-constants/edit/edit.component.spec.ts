/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { TempApiEditComponent } from './edit.component';

let component: TempApiEditComponent;
let fixture: ComponentFixture<TempApiEditComponent>;

describe('edit component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
          declarations: [ TempApiEditComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
      fixture = TestBed.createComponent(TempApiEditComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
