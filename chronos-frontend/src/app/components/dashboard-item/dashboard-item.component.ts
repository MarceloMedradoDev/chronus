import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Card2Component } from '../card2/card2.component';

@Component({
  selector: 'app-dashboard-item',
  standalone: true,
  imports: [CommonModule, Card2Component],
  templateUrl: './dashboard-item.component.html',
  styleUrl: './dashboard-item.component.css'
})
export class DashboardItemComponent {
  
  @Input() item: any;
  @Input() cargoSelecionado = '';
  @Input() icone = '';
  @Input() titulo = '';
  @Input() descricao = '';
  @Input() isSelected = false;
  @Input() nome = ''
  @Input() isAdmin = false;
  
  @Output() cardClick = new EventEmitter<string>();

  // dadosAPI = [
  //   { data: { cargo: 'Junior', horas: 120 } },
  //   { data: { cargo: 'Pleno', horas: 160 } }
  // // ];

  atualizarMeta(cargo: string) {
  this.cargoSelecionado = cargo; // guarda o card selecionado
}

  
}
