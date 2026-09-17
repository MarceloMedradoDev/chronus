import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {

  signUpForm: FormGroup;
  errorMessage = '';
  loading = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.signUpForm = this.fb.group({
      name: ['', Validators.required],
      registration: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    });
  }

  onSubmit() {
    this.errorMessage = '';

    if (this.signUpForm.invalid) {
      this.errorMessage = 'Preencha todos os campos corretamente.';
      return;
    }

    const { name, registration, password, confirmPassword } = this.signUpForm.value;

    if (password !== confirmPassword) {
      this.errorMessage = 'As senhas não conferem.';
      return;
    }

    const body = {
      name,
      registration,
      password,
      type: 0
    };

    this.loading = true;

    this.http.post('http://localhost:5271/api/User/create', body)
      .subscribe({
        next: (res: any) => {

          if (!res.success) {

            if (res.message === 'Usuário já cadastrado') {
              this.errorMessage = res.message;
            } else {
              this.errorMessage = res.message || 'Erro ao criar usuário.';
            }

            this.loading = false;
            return;
          }

          // Sucesso → salvar token retornado
          localStorage.setItem('token', res.data.token);

          this.router.navigate(['/home']);
        },

        error: () => {
          this.errorMessage = 'Erro ao conectar ao servidor.';
          this.loading = false;
        }
      });
  }
}
