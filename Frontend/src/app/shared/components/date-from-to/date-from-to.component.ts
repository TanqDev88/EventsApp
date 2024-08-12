import { Component, EventEmitter, Output } from '@angular/core';
import { NgbCalendar, NgbDate, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-date-from-to',
  templateUrl: './date-from-to.component.html',
  styleUrls: ['./date-from-to.component.scss']
})
export class DateFromToComponent {

  hoveredDate: NgbDate | null = null;
  fromDate: NgbDate | null;
  // fromDate: NgbDate | null = this.calendar.getToday();
  toDate: NgbDate | null;
  // toDate: NgbDate | null = this.calendar.getNext(this.calendar.getToday(), 'd', 10);

  @Output() selectedDate = new EventEmitter<{ fromDate: string | null, toDate: string | null }>();

  constructor(private calendar: NgbCalendar,
    public formatter: NgbDateParserFormatter) {

  }

  // Método para emitir las fechas seleccionadas
  emitDates() {

    let fd = '';
    // -- Date From
    if (this.fromDate && typeof this.fromDate === 'object' && 'day' in this.fromDate && 'month' in this.fromDate && 'year' in this.fromDate) {
      fd = `${this.fromDate.year}-${this.fromDate.month}-${this.fromDate.day}`;
    } else {
      fd = undefined;
    }

    let td = '';
    // -- Date To
    if (this.toDate && typeof this.toDate === 'object' && 'day' in this.toDate && 'month' in this.toDate && 'year' in this.toDate) {
      td = `${this.toDate.year}-${this.toDate.month}-${this.toDate.day}`;
    } else {
      td = undefined;
    }

    this.selectedDate.emit({ fromDate: fd, toDate: td });
  }

  onDateSelection(date: NgbDate) {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
    } else if (this.fromDate && !this.toDate && date && date.after(this.fromDate)) {
      this.toDate = date;
    } else {
      this.toDate = null;
      this.fromDate = date;
    }

    this.emitDates();
  }

  isHovered(date: NgbDate) {
    return (
      this.fromDate && !this.toDate && this.hoveredDate && date.after(this.fromDate) && date.before(this.hoveredDate)
    );
  }

  isInside(date: NgbDate) {
    return this.toDate && date.after(this.fromDate) && date.before(this.toDate);
  }

  isRange(date: NgbDate) {
    return (
      date.equals(this.fromDate) ||
      (this.toDate && date.equals(this.toDate)) ||
      this.isInside(date) ||
      this.isHovered(date)
    );
  }

  validateInput(currentValue: NgbDate | null, input: string): NgbDate | null {
    const parsed = this.formatter.parse(input);
    return parsed && this.calendar.isValid(NgbDate.from(parsed)) ? NgbDate.from(parsed) : currentValue;
  }

  clear():void{
    this.fromDate = null;
    this.toDate = null;
    this.emitDates();
  }
}
