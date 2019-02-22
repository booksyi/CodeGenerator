/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { TempApiListComponent } from './list.component';

let component: TempApiListComponent;
let fixture: ComponentFixture<TempApiListComponent>;

describe('list component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
          declarations: [ TempApiListComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(TempApiListComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
