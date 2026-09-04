import { TestBed } from '@angular/core/testing';

import { CiudadService } from './ciudades'; 

describe('Ciudades', () => {
  let service: CiudadService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CiudadService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
