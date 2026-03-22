import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  username = signal('');
  email = signal('');
  password = signal('');
  isLoading = signal(false);
  showPassword = signal(false);
  errorMessage = signal('');

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onUsernameChange(value: string) { this.username.set(value); }
  onEmailChange(value: string) { this.email.set(value); }
  onPasswordChange(value: string) { this.password.set(value); }
  togglePasswordVisibility() { this.showPassword.update((v) => !v); }

  onSubmit() {
    const username = this.username().trim();
    const email = this.email().trim();
    const password = this.password();

    // Validation
    if (!username || !email || !password) {
      this.errorMessage.set('All fields are required.');
      return;
    }

    if (password.length < 8) {
      this.errorMessage.set('Password must be at least 8 characters long.');
      return;
    }

    if (!email.includes('@')) {
      this.errorMessage.set('Please enter a valid email address.');
      return;
    }

    this.errorMessage.set('');
    this.isLoading.set(true);

    this.authService.register(username, email, password).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (error) => {
        this.errorMessage.set(error instanceof Error ? error.message : 'Registration failed. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}
