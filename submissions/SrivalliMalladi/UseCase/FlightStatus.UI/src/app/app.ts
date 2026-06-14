import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';

import { SearchFormComponent } from './components/search-form/search-form.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule, SearchFormComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {}
