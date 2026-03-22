import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ExcuseService, ExcuseResponse } from '../services/excuse.service';

@Component({
  selector: 'app-submit-excuse',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './submit-excuse.component.html'
})
export class SubmitExcuseComponent {
  excuseText = signal('');
  judgePersonality = signal('');
  customPersonality = signal('');
  isLoading = signal(false);
  errorMessage = signal('');
  result = signal<ExcuseResponse | null>(null);

  presetPersonalities = [
    'sarcastic lawyer',
    'disappointed parent',
    'strict teacher',
    'cynical boss',
    'tough coach',
    'supportive therapist'
  ];

  constructor(
    private excuseService: ExcuseService,
    private router: Router
  ) {}

  setPersonality(personality: string) {
    this.judgePersonality.set(personality);
    this.customPersonality.set('');
  }

  onSubmit() {
    if (this.excuseText().length < 10) {
      this.errorMessage.set('Excuse must be at least 10 characters.');
      return;
    }

    const personality = this.customPersonality() || this.judgePersonality();
    if (!personality) {
      this.errorMessage.set('Please select or enter a judge personality.');
      return;
    }

    this.errorMessage.set('');
    this.isLoading.set(true);

    this.excuseService.submitExcuse(this.excuseText(), personality).subscribe({
      next: (response) => {
        this.result.set(response);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(error instanceof Error ? error.message : 'Failed to submit excuse.');
        this.isLoading.set(false);
      }
    });
  }

  resetForm() {
    this.excuseText.set('');
    this.judgePersonality.set('');
    this.customPersonality.set('');
    this.result.set(null);
    this.errorMessage.set('');
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  goToLeaderboard() {
    this.router.navigate(['/wall-of-fame']);
  }
}
