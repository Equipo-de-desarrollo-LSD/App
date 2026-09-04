import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PerfilPublicoComponent } from './perfil-publico';

describe('PerfilPublico', () => {
  let component: PerfilPublicoComponent;
  let fixture: ComponentFixture<PerfilPublicoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PerfilPublicoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PerfilPublicoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
