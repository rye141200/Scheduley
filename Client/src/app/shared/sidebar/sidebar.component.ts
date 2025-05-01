import { HttpContext } from '@angular/common/http';
import {
  Component,
  computed,
  EventEmitter,
  inject,
  Input,
  Output,
  signal,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-sidebar',
  imports: [RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
  isOpenSignal = signal(false);
  private readonly authService = inject(AuthService);
  @Input() username!: string | null;

  toggle() {
    this.isOpenSignal.set(!this.isOpenSignal());
    // console.log(this.username);
  }
  onLogout() {
    this.authService.logout();
  }
  onLogin() {
    this.authService.login();
  }
}
