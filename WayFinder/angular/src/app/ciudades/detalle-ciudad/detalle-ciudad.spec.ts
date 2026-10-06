import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetalleCiudad } from './detalle-ciudad';

describe('DetalleCiudad', () => {
  let component: DetalleCiudad;
  let fixture: ComponentFixture<DetalleCiudad>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetalleCiudad]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetalleCiudad);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
