/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { TempApiCreateComponent } from './create.component';

let component: TempApiCreateComponent;
let fixture: ComponentFixture<TempApiCreateComponent>;

describe('create component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
          declarations: [ TempApiCreateComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
      fixture = TestBed.createComponent(TempApiCreateComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
