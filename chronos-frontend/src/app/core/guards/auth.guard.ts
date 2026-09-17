import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  exp: number; // Tempo de expiração em timestamp Unix (segundos)
}

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    // 1. Usa o próprio método isTokenExpired() para validar o token
    if (this.isTokenExpired()) {
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      if (!decoded.exp) return false;

      // Date.now() retorna milissegundos, decoded.exp está em segundos
      const dateExp = decoded.exp * 1000;
      return Date.now() >= dateExp;
    } catch (e) {
      // Se o token for inválido ou malformatado, considera expirado
      return true;
    }
  }
}