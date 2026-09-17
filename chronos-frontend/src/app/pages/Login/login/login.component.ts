import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { jwtDecode } from 'jwt-decode';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  loginForm: FormGroup;
  loginError = '';
  loading = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      registration: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  onSubmit() {
    this.loginError = '';

    if (this.loginForm.invalid) {
      this.loginError = 'Preencha todos os campos.';
      return;
    }

    const body = {
      registration: this.loginForm.value.registration,
      password: this.loginForm.value.password
    };

      var registration = parseInt(body.registration);

    this.loading = true;

    this.http.post('http://localhost:5271/api/User/login', body)
      .subscribe({

        next: (res: any) => {

          
          if (!res.success) {

            switch (res.message) {
              case 'Usuário não existe':
                this.loginError = 'Este usuário não existe.';
                break;

              case 'Senha incorreta':
                this.loginError = 'Senha incorreta.';
                break;

              default:
                if (Number.isNaN(registration)) this.loginError = "A matrícula deve ser numérica";
                else this.loginError = res.message || 'Erro ao tentar fazer login.';
            }

            this.loading = false;
            return;
          }
          
          const decoded: any = jwtDecode(res.data.Token);
          localStorage.setItem('token', res.data.Token);
          localStorage.setItem('userId', res.data.Id);


          this.router.navigate(['/home']);
        },

        error: () => {
          this.loginError = 'Erro ao conectar ao servidor.';
          this.loading = false;
        }
      });
  }
}
