import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ExcuseService, ExcuseResponse } from '../services/excuse.service';

@Component({
  selector: 'app-my-excuses',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-excuses.component.html'
})
export class MyExcusesComponent implements OnInit {
  excuses = signal<ExcuseResponse[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  currentPage = signal(1);
  pageSize = 10;

  constructor(
    private excuseService: ExcuseService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadExcuses();
  }

  loadExcuses() {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.excuseService.getMyExcuses(this.currentPage(), this.pageSize).subscribe({
      next: (response) => {
        this.excuses.set(response);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(error instanceof Error ? error.message : 'Failed to load excuses.');
        this.isLoading.set(false);
      }
    });
  }

  loadMore() {
    this.currentPage.update(p => p + 1);
    this.isLoading.set(true);

    this.excuseService.getMyExcuses(this.currentPage(), this.pageSize).subscribe({
      next: (response) => {
        this.excuses.update(existing => [...existing, ...response]);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(error instanceof Error ? error.message : 'Failed to load more.');
        this.isLoading.set(false);
      }
    });
  }

  canLoadMore(): boolean {
    return this.excuses().length % this.pageSize === 0 && this.excuses().length > 0;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }

  viewDetails(excuseId: string) {
    this.router.navigate(['/excuse', excuseId]);
  }

  goToSubmit() {
    this.router.navigate(['/submit-excuse']);
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }
}
