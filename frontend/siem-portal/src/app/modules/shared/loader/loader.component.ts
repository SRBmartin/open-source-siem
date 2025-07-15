// app/shared/ui/app-loader/app-loader.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoaderService } from '../../../shared/services/loader.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-loader',
  standalone: true,
  imports: [CommonModule, NgIf],
  template: `
    <div class="loader-backdrop" *ngIf="loaderService.isLoading | async">
      <div class="spinner"></div>
    </div>
  `,
  styleUrls: ['./loader.component.scss']
})
export class AppLoaderComponent {
  constructor(public loaderService: LoaderService) {}
}
