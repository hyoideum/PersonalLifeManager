import { Injectable } from '@angular/core';
import { EN_STRINGS } from '../i18n/en';
import { HR_STRINGS } from '../i18n/hr';

@Injectable({
  providedIn: 'root',
})
export class I18nService {
  private currentLang: 'en' | 'hr' = 'en';
    private strings = EN_STRINGS;

    setLanguage(lang: 'en' | 'hr') {
        this.currentLang = lang;
        this.strings = lang === 'en' ? EN_STRINGS : HR_STRINGS;
    }

  get(key: string): string {
    return key.split('.').reduce((acc: any, k: string) => acc[k], this.strings) || '';
  }
}
