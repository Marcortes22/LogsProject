import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ErrorLogsComponent } from './components/error-logs/error-logs.component';
import { LoginComponent } from "./components/login/login.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ErrorLogsComponent, LoginComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'LogsFrontEnd';
}
