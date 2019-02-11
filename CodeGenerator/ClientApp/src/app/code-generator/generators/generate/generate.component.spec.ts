/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { GeneratorsGenerateComponent } from './generate.component';

let component: GeneratorsGenerateComponent;
let fixture: ComponentFixture<GeneratorsGenerateComponent>;

describe('generate component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
          declarations: [GeneratorsGenerateComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
      fixture = TestBed.createComponent(GeneratorsGenerateComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
