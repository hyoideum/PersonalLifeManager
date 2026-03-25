import { Component } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { I18nService } from '../../services/i18n.service';
import { AuthService } from '../../services/auth.service';
import { MatMenuModule } from '@angular/material/menu';

@Component({
    selector: 'app-main-layout',
    standalone: true,
    imports: [
        RouterOutlet,
        MatSidenavModule,
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        RouterModule,
        MatMenuModule
    ],
    templateUrl: './main-layout.html',
    styleUrl: './main-layout.css'
})
export class MainLayout {
    constructor(public i18n: I18nService, public auth: AuthService, private router: Router) { }

    logout() {
        this.auth.logout();
        this.router.navigate(['auth/login']);
    }

    changeLanguage(lang: 'en' | 'hr') {
        this.i18n.setLanguage(lang);
    }
}