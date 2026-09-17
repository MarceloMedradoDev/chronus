import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { ManagerComponent } from './pages/manager/manager.component';
import { LoginComponent } from './pages/Login/login/login.component';
import { AuthGuard } from './core/guards/auth.guard';
import { SignUpComponent } from './pages/sign-up/sign-up.component';
import { ReportsComponent } from './pages/reports/reports.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signUp', component: SignUpComponent },
  { path: 'home', canActivate: [AuthGuard], component: HomeComponent },
  { path: 'manager', canActivate: [AuthGuard], component: ManagerComponent },
  { path: 'reports', canActivate: [AuthGuard], component: ReportsComponent },
  { path: '**', redirectTo: '/login' },
];
