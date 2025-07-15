import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppLoaderComponent } from "./modules/shared/loader/loader.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, 
    AppLoaderComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'siem-portal';
}
