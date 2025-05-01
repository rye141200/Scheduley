import { HttpClient } from '@angular/common/http';
import { Component, computed, inject, Input, signal } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  private authService = inject(AuthService);
  private http = inject(HttpClient);
  userName: string | null = null;
  ngOnInit() {
    this.userName = this.authService.getUsername();
  }
  onLogin() {
    this.authService.login();
  }
  isLoggedIn() {
    return this.userName != null;
  }
  onProtected() {
    this.authService.getUserData();
  }
  onRefreshMe() {
    this.http.get(environment.apiURL + 'api/account/refresh-token').subscribe({
      next: (value) => console.log(value),
      error: (value) => console.log(value),
    });
  }
}
