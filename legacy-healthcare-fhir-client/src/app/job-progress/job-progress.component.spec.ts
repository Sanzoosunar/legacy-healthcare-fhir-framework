import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobProgressComponent } from './job-progress.component';

describe('JobProgressComponent', () => {
  let component: JobProgressComponent;
  let fixture: ComponentFixture<JobProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobProgressComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
