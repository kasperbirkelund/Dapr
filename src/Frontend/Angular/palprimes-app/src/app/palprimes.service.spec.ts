import { TestBed } from '@angular/core/testing';

import { PalprimesService } from './palprimes.service';

describe('PalprimesService', () => {
  let service: PalprimesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PalprimesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
