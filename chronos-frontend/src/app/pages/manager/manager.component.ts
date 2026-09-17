import { Component, HostListener, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HeaderComponent } from '../../components/header/header.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { CardComponent } from '../../components/card/card.component';
import { ApiService } from '../../services/api.service';
import { EmployeeReturnDTO, ResponseModel } from '../../models/employee.model';
import { calcularDiasUteis } from '../../helpers/CalcDiasUteis';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';


@Component({
  selector: 'app-manager',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent, CardComponent, FormsModule, MatSnackBarModule],
  templateUrl: './manager.component.html',
  styleUrls: ['./manager.component.css'],
})
export class ManagerComponent implements OnInit {
  meta: number | null = null;
  horasAcumuladas: number | null = null;
  diferencaMeta: number | null = null;

  horasAcumuladasFormatadas: string | null = null;
  diferencaMetaFormatada: string | null = null;
  diferencaMetaCor: string = '';
  saldoCor: string = '';
  totalUploadsMes: number = 13;
  horasPrevistasAcumuladas: number | null = null;
  horasPrevistasAcumuladasFormatadas: string | null = null;
  cargoSelecionado: string | null = null;

  isAdmin = false;
  completedRequest = false;

  isCompleteDayPorCargo: { [key: string]: boolean } = {
    Junior: true,
    Pleno: true,
    Sênior: true
  };


  horasPorCargo: { [key: string]: string } = {
    'Junior': '---',
    'Pleno': '---',
    'Sênior': '---'
  };


  dadosAPI: ResponseModel<EmployeeReturnDTO>[] = [];

  metasPorCargo: { [key: string]: number } = {
    'Junior': 880,
    'Pleno': 528,
    'Sênior': 352
  };

  constructor(private http: HttpClient, private api: ApiService, private snackBar: MatSnackBar) { }


  meses: string[] = [
    'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
    'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
  ];

  mesSelecionado: number = new Date().getMonth() + 1 // mes atual

  reprocessando = false;

  ngOnInit(): void {
    // Nenhum carregamento automático — só após upload
    this.carregarHoras();
    this.calcDiasUteis();
    this.verificarRole();
    // this.atualizarDiasTrabalhados()

    window.addEventListener('load', () => {
    // Timeout para garantir que o layout renderizou antes de rolar
    setTimeout(() => {
      window.scrollTo({
        top: 350, 
        behavior: 'smooth'
      });
    }, 100); // 100ms é o tempo suficiente para o navegador se preparar
  });
  }


 reprocessar(): void {
  if (this.reprocessando) return;

  this.reprocessando = true;

  this.api.reprocessar().subscribe({
    next: () => {
      this.reprocessando = false;

      this.snackBar.open(
        'Reprocessamento concluído!',
        'Fechar',
        {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        }
      );

      // Atualiza os cards depois do reprocessamento
      this.carregarHoras();

      // Se já existe um cargo selecionado, recalcula os detalhes
      if (this.cargoSelecionado) {
        this.atualizarMeta(this.cargoSelecionado);
      }
    },

    error: (err) => {
      this.reprocessando = false;

      this.snackBar.open(
        'Erro ao realizar o reprocessamento.',
        'Fechar',
        {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'bottom'
        }
      );

      console.error('Erro no reprocessamento:', err);
    }
  });
}


  // Busca dos dados processados
  // Chama o endpoint GET /GetTotalHours.
  // Armazena os dados na variável dadosAPI.
  carregarHoras(): void {
    this.api.getTotalHours().subscribe({
      next: (res) => {

        this.dadosAPI = res;

        const mapeamentoCargos: { [key: string]: string } = {
          'Junior': 'ANL TI JR',
          'Pleno': 'ANL TI PL',
          'Sênior': 'ANL TI SR'
        };

        for (const cargo in mapeamentoCargos) {

          const registros = res.filter(
            d => d.data?.role === mapeamentoCargos[cargo]
          );

          let totalMinutos = 0;

          // Começa considerando o cargo completo
          this.isCompleteDayPorCargo[cargo] = true;

          for (const registro of registros) {

            const horasStr = registro.data?.totalHours;

            if (horasStr) {
              const [h, m] = horasStr.split(':').map(Number);
              totalMinutos += h * 60 + m;
            }

            // Se qualquer registro estiver incompleto,
            // o card daquele cargo ficará vermelho
            if (registro.data?.isCompleteDay === false) {
              this.isCompleteDayPorCargo[cargo] = false;
            }
          }

          const horas = Math.floor(totalMinutos / 60);
          const minutos = totalMinutos % 60;

          this.horasPorCargo[cargo] =
            `${horas}h${minutos.toString().padStart(2, '0')}`;
        }

        console.log(
          'Status por cargo:',
          this.isCompleteDayPorCargo
        );
      },

      error: (err) => {
        console.error('Erro ao buscar horas:', err);
      }
    });
  }


  verificarRole(): void {
    const token = localStorage.getItem('token');
    if (!token) return;
  
    const decoded: any = jwtDecode(token);
  
    this.isAdmin =
      decoded.role === 'ADM' ||
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'ADM';
    }


  // Atualização dos dados ao clicar em um card
  atualizarMeta(cargo: string): void {
    this.meta = this.metasPorCargo[cargo] || null;
    this.cargoSelecionado = cargo;
  
    const mapeamentoCargos: { [key: string]: string } = {
      'Junior': 'ANL TI JR',
      'Pleno': 'ANL TI PL',
      'Sênior': 'ANL TI SR'
    };
  
    const cargoAPI = mapeamentoCargos[cargo];
  
    // Filtra todos os registros do cargo
    const registrosCargo = this.dadosAPI.filter(d => d.data?.role === cargoAPI);
  
    // Soma total de minutos
    let totalMinutos = 0;
    for (const registro of registrosCargo) {
      const horasStr = registro.data?.totalHours;
      if (horasStr) {
        const [horas, minutos] = horasStr.split(':').map(Number);
        totalMinutos += horas * 60 + minutos;
      }
    }
  
    // Converte minutos totais para HH:MM
    const horas = Math.floor(totalMinutos / 60);
    const minutos = totalMinutos % 60;
    this.horasAcumuladas = parseFloat((horas + minutos / 60).toFixed(2));
    const horasFormatadas = `${horas}h${minutos.toString().padStart(2, '0')}`;
    this.horasAcumuladasFormatadas = horasFormatadas;
  
    // Cálculo da diferença da meta
    if (this.meta !== null) {
      const diferenca = this.horasAcumuladas - this.meta;
      const diferencaHoras = Math.floor(Math.abs(diferenca));
      const diferencaMinutos = Math.round((Math.abs(diferenca) % 1) * 60);
      this.diferencaMetaFormatada = `${diferencaHoras}h${diferencaMinutos.toString().padStart(2, '0')}`;
      this.diferencaMetaCor = diferenca < 0 ? 'negativo' : 'positivo';
    } else {
      this.diferencaMetaFormatada = null;
      this.diferencaMetaCor = '';
    }
  
    // Cálculo das Horas Previstas Acumuladas
    const horasPorDiaCargo: { [key: string]: number } = {
     'Junior': 44,
      'Pleno': 26 + (24 / 60),
      'Sênior': 17 + (36 / 60)
    };
  
    const horasPrevistas = horasPorDiaCargo[cargo] * this.diasTrabalhados;
    this.horasPrevistasAcumuladas = horasPrevistas;
    console.log('HORAS PREVISTAS', this.horasPrevistasAcumuladas)
    console.log('DIAS TRABALHADOS', this.diasTrabalhados)
    // this.horasPrevistasAcumuladasFormatadas = `${(((horas * 60) + minutos) - (horasPrevistas * 60)) < 0 ? '-' : ''}${Math.floor(Math.abs(((horas * 60) + minutos) - (horasPrevistas * 60)) / 60)}:${String(Math.abs(((horas * 60) + minutos) - (horasPrevistas * 60)) % 60).padStart(2, '0')}`;
    const totalMinutosPrevistos = Math.round(((horas * 60) + minutos) - (horasPrevistas * 60));

    this.horasPrevistasAcumuladasFormatadas = `${totalMinutosPrevistos < 0 ? '-' : ''}${Math.floor(Math.abs(totalMinutosPrevistos) / 60)}:${String(Math.abs(totalMinutosPrevistos) % 60).padStart(2, '0')}`;
    console.log('HORAS PREVISTAS ACUMULADAS', this.horasPrevistasAcumuladasFormatadas)


    this.saldoCor = horas - horasPrevistas < 0 ? 'negativo' : 'positivo';

  }
  


  //Reset dos dados ao clicar fora dos cards
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const clickedInsideCard = target.closest('app-card');
  
    // if (!clickedInsideCard) {
    //   this.cargoSelecionado = null;
    //   this.meta = null;
    //   this.horasAcumuladas = null;
    //   this.diferencaMeta = null;
    //   this.horasAcumuladasFormatadas = null;
    //   this.diferencaMetaFormatada = null;
    //   this.horasPrevistasAcumuladas = null;
    //   this.horasPrevistasAcumuladasFormatadas = null;
    // }
  }
  
  status: 'error' | 'done' | null | undefined; // define os status
  fechando = false;

  //Envio do arquivo Excel
  onFileSelected(event: any): void {
  const file: File = event.target.files[0];

  if (file) {
    const formData = new FormData();
    formData.append('file', file);

    this.api.insertHours(formData).subscribe({
      next: () => {
        setTimeout(() => {
          this.status = 'done';
          this.fecharComAnimacao(); 
        }, 800); // tempo de abertura

      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}

fecharComAnimacao(): void { 
  setTimeout(() => {
    this.fechando = true;
    setTimeout(() => {
      this.status = null;
      this.fechando = false;
    }, 300); //tempo da animação

  }, 3500); //tempo para fechamento
}

  mostrarPopup = false;
  totalDiasUteis = 0;
  totalDiasUteisArray: number[] = [];

  diasTrabalhados = 0;

  calcDiasUteis(): void {
    const hoje = new Date();
    const anoAtual = hoje.getFullYear();
    const mesAtual = hoje.getMonth() + 1;
    const diaAtual = hoje.getDate();
  
    const dias = calcularDiasUteis(this.mesSelecionado, anoAtual);
    this.totalDiasUteis = dias.uteis;
    this.totalDiasUteisArray = dias.arrayDiasUteis;
  
    this.diasTrabalhados = 0;
  
    // Se o mês selecionado for anterior ao atual → conta todos os dias úteis
    if (this.mesSelecionado < mesAtual) {
      this.diasTrabalhados = dias.arrayDiasUteis.length;
    }
  
    // Se for o mês atual → conta apenas os dias úteis anteriores ao dia de hoje
    else if (this.mesSelecionado == mesAtual) {
      for (const dia of dias.arrayDiasUteis) {
        if (dia < diaAtual) {
          this.diasTrabalhados++;
        }
      }
    }
  
    // Se for mês futuro → não conta nada (diasTrabalhados já está 0)
    else {
      this.diasTrabalhados = 0;
    }
  
    // Zera uploads ao trocar de mês
    this.totalUploadsMes = 0;
  }
  


  atualizarDetalhes(valores: { total: number, trabalhados: number }): void {
    this.totalDiasUteis = valores.total;
    //this.diasTrabalhados = valores.trabalhados;
  }

}
