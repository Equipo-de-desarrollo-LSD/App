import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BuscarCiudades } from './buscar-ciudades';

describe('BuscarCiudades', () => {
  let component: BuscarCiudades;
  let fixture: ComponentFixture<BuscarCiudades>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BuscarCiudades]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BuscarCiudades);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
