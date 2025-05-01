import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';
import { ScheduleyJWTPayload } from '../../dto/ScheduleyJWTPayload';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private cookieService = inject(CookieService);
  private cookieName = 'ScheduleyAccessToken';
  private token!: string;

  public login() {
    window.location.href = encodeURI(environment.googleLoginUrlString);
  }

  public logout() {
    this.http.get(`${environment.apiURL}api/account/logout`).subscribe({
      next: (value) => console.log(value),
      error: (err) => console.log(err),
    });
  }
  public getUsername() {
    if (!this.isLoggedIn()) return null;
    const jwtPayload = jwtDecode<ScheduleyJWTPayload>(
      this.cookieService.get(this.cookieName)
    );
    return jwtPayload.unique_name;
  }
  public isLoggedIn() {
    //TODO replace this shit
    return this.cookieService.get(this.cookieName) ?? false;
  }

  public getUserData() {
    this.http.get('https://localhost:7116/api/account/').subscribe({
      next: (value) => console.log(value),
      error: (err) => console.log(err),
    });
  }
}
