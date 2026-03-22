import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './signin.component.html'
})
export class SigninComponent {
  email = signal('');
  password = signal('');
  isLoading = signal(false);
  showPassword = signal(false);
  errorMessage = signal('');

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onEmailChange(value: string) { this.email.set(value); }
  onPasswordChange(value: string) { this.password.set(value); }
  togglePasswordVisibility() { this.showPassword.update((v) => !v); }

  onSubmit() {
    if (!this.email() || !this.password()) {
      this.errorMessage.set('All parameters are required to proceed.');
      return;
    }

    this.errorMessage.set('');
    this.isLoading.set(true);

    this.authService.login(this.email(), this.password()).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.errorMessage.set(error instanceof Error ? error.message : 'Login failed. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}