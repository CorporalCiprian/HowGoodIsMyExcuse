import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface SubmitExcuseRequest {
  text: string;
  judgePersonality: string;
}

export interface ExcuseResponse {
  id: string;
  text: string;
  score: number;
  verdict: string;
  roast: string;
  judgePersonality: string;
  votes: number;
  createdAt: string;
  username: string;
  hasVoted: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ExcuseService {
  private apiUrl = 'http://localhost:5000/api/excuses';

  constructor(private http: HttpClient) {}

  submitExcuse(text: string, judgePersonality: string): Observable<ExcuseResponse> {
    return this.http.post<ExcuseResponse>(this.apiUrl, {
      text,
      judgePersonality
    } as SubmitExcuseRequest).pipe(
      catchError(error => {
        const errorMessage = error.error?.error || 'Failed to submit excuse. Please try again.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  getLeaderboard(page: number = 1, pageSize: number = 10): Observable<ExcuseResponse[]> {
    return this.http.get<ExcuseResponse[]>(this.apiUrl, {
      params: { page: page.toString(), pageSize: pageSize.toString() }
    }).pipe(
      catchError(error => {
        const errorMessage = error.error?.error || 'Failed to load leaderboard.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  getExcuseById(id: string): Observable<ExcuseResponse> {
    return this.http.get<ExcuseResponse>(`${this.apiUrl}/${id}`).pipe(
      catchError(error => {
        const errorMessage = error.error?.error || 'Failed to load excuse.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  voteOnExcuse(id: string): Observable<ExcuseResponse> {
    return this.http.post<ExcuseResponse>(`${this.apiUrl}/${id}/vote`, {}).pipe(
      catchError(error => {
        const errorMessage = error.error?.error || 'Failed to vote.';
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
