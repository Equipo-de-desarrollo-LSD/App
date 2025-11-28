import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CiudadService } from './ciudades';

describe('Ciudades', () => {
  let component: CiudadService;
  let fixture: ComponentFixture<CiudadService>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CiudadService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CiudadService);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
