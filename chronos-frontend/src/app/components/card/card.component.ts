import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css']
})
export class CardComponent {
  @Input() icone: string = '';
  @Input() titulo?: string = '';
  @Input() descricao?: string = '';
  @Input() isSelected: boolean = false;
  @Output() cardClick = new EventEmitter<string>();
  @Output() selected = new EventEmitter<boolean>();
  @Input() isCompleteDay = true;
  

  onClick() {
    this.cardClick.emit(this.titulo);
  }
}
