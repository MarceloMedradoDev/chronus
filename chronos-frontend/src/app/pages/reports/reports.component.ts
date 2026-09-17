import { Component, Input } from '@angular/core';
import { FooterComponent } from '../../components/footer/footer.component';
import { HeaderComponent } from '../../components/header/header.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { HttpClient } from '@angular/common/http';
import { EmployeeModel, ResponseModel } from '../../models/employee.model';
import { Card2Component } from '../../components/card2/card2.component';
import { DashboardItemComponent } from '../../components/dashboard-item/dashboard-item.component';
import {
  Chart,
  DoughnutController,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js';
import { jwtDecode } from 'jwt-decode';

Chart.register(
  DoughnutController,
  ArcElement,
  Tooltip,
  Legend
);



@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FormsModule, DashboardItemComponent],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent implements OnInit{
    
  dadosAPI: ResponseModel<EmployeeModel>[] = [];
  cargoSelecionado = '';
  isAdmin = false;

  constructor(private api: ApiService) {}

  //inicia as funções ao iniciar
  ngOnInit(): void {
    this.carregarReports();
    this.verificarRole();
  }

   carregarReports(): void {
  this.api.getReports().subscribe({
    next: (res) => {

      const cargosPermitidos = [
        'ANL TI JR',
        'ANL TI SR',
        'ANL TI PL'
      ];

      this.dadosAPI = res
        .filter(item =>
          cargosPermitidos.includes(item.data?.role?.trim() || '')
        )
        .sort((a, b) => {
          const roleA = (a.data?.role || '').toLowerCase();
          const roleB = (b.data?.role || '').toLowerCase();

          return roleA.localeCompare(roleB);
        });

      console.log(this.dadosAPI);
    },
    error: (err) => console.error(err)
  });
}


  //verifica a role e permite o acesso
  verificarRole(): void {
    const token = localStorage.getItem('token');

  if (!token) return;

  const decoded: any = jwtDecode(token);

  this.isAdmin =
    decoded.role === 'ADM' ||
    decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'ADM';
  }

  atualizarMeta(cargo: string) {
    this.cargoSelecionado = cargo;
  }

}