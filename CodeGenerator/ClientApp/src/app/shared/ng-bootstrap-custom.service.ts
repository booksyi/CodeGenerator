import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Injectable({
  providedIn: 'root'
})
export class NgbModalStack {
  constructor(
    private modalService: NgbModal) { }

  data: any;
  private stack: NgbModalStackItem[] = [];
  private dismiss: boolean = false;
  open(content, data) {
    let container = this;
    container.data = data;
    container.stack.push({ content: content, data: data });
    if (container.modalService.hasOpenModals()) {
      container.dismiss = true;
      container.modalService.dismissAll();
    }
    container.modalService.open(content, { size: "lg" }).result.finally(function () {
      if (container.dismiss) {
        container.dismiss = false;
      }
      else {
        container.stack.splice(-1, 1);
        container.data = null;
        if (container.stack.length) {
          let lastModal = container.stack.pop();
          container.open(lastModal.content, lastModal.data);
        }
      }
    });
  }
}

class NgbModalStackItem {
  public content: any;
  public data: any;
}
