import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app-button.component.html',
  styleUrls: ['./app-button.component.scss']
})
export class BasicButtonComponent {
  @Input() label: string = '';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() class: string = '';
  @Input() disabled: boolean = false;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() backgroundImage: string = '';

  @Output() clickEvent: EventEmitter<void> = new EventEmitter<void>();

  onClick(): void {
    if (!this.disabled) {
      this.clickEvent.emit();
    }
  }

  get getClass(): string {
    let baseClass = 'btn';
    if (this.size === 'sm') baseClass += ' btn-sm';
    if (this.size === 'md') baseClass += ' btn-md';
    if (this.size === 'lg') baseClass += ' btn-lg';
  
    return `${baseClass} ${this.class}`;
  }
 
  getStyles(): { [key: string]: string } {
    return this.backgroundImage
      ? { 'background-image': `url(${this.backgroundImage})`, 'background-size': 'cover' }
      : {};
  }
}
