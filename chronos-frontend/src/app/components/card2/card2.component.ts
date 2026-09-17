import { CommonModule } from '@angular/common';
import {
  Component,
  Input,
  Output,
  EventEmitter,
  AfterViewInit,
  ViewChild,
  ElementRef,
  input
} from '@angular/core';
import Chart from 'chart.js/auto';
import { EmployeeModel, ResponseModel } from '../../models/employee.model';

@Component({
  selector: 'app-card2',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card2.component.html',
  styleUrl: './card2.component.css'
})
export class Card2Component implements AfterViewInit {

  
  @Input() icone = '';
  @Input() titulo = '';
  @Input() descricao!: string; // ex: "90:25"
  @Input() isSelected = false;
  @Input() nome = '';
  @Input() isAdmin = false;
  @Input() isCompleteDay = true;

  @Output() cardClick = new EventEmitter<string>();

  @ViewChild('chartCanvas') canvas!: ElementRef<HTMLCanvasElement>;

  chart: any; //criando a variavel que pode receber qualquer dado

  readonly TOTAL_MINUTOS = 176 * 60; // 176 horas. Transformando horas para minutos.

  ngAfterViewInit() {

    const realizados = this.converterParaMinutos(this.descricao); //definindo os valores de realizadas e convertendo para minutos.
    const faltantes = Math.max(this.TOTAL_MINUTOS - realizados, 0); //definindo as horas faltantes, e calculando as horas faltantes.

    this.chart = new Chart(this.canvas.nativeElement, { //criando um elemento para o chart para passar todas as informações.
      type: 'doughnut',
      data: {
        labels: [`Horas faltantes: ${this.formatarMinutos(faltantes)}`, `Horas realizadas: ${this.formatarMinutos(realizados)}`],
        datasets: [{
          data: [faltantes, realizados],
          backgroundColor: ['#c74359', '#00bfb8']
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            labels: {
              font: {
                size: 14,
                family: 'Arial',
              }
            }
          },
          tooltip: {
            callbacks: {
              label: (ctx) => this.formatarMinutos(ctx.raw as number) 
            }
          }
        }
      },
    });
  }

  converterParaMinutos(valor: string): number { //calculo para converter para minutos
    if (!valor) return 0;
    const [h, m] = valor.split(':').map(Number);
    return h * 60 + m;
  }

  formatarMinutos(min: number): string { //função para transformar mminutos em formato de horas:minutos
    const h = Math.floor(min / 60);
    const m = min % 60;
    return `${h}:${m.toString().padStart(2, '0')}`;
  }

  textoCentralPlugin(texto: string) { // para transformar os dados do chart 
    return {
      id: 'textoCentral',
      beforeDraw(chart: any) {
        const { ctx } = chart;
        const centerX = chart.getDatasetMeta(0).data[0].x;
        const centerY = chart.getDatasetMeta(0).data[0].y;
      }
    };
  }

  onClick() {
    this.cardClick.emit(this.titulo);
  }
}