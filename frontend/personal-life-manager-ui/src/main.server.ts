import { BootstrapContext, bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app/app.routes';
import { authInterceptor } from './app/core/interceptors/auth.interceptor';

const bootstrap = (context: BootstrapContext) =>
    bootstrapApplication(App, {
    providers: [
      provideRouter(routes),
      provideHttpClient(withInterceptors([authInterceptor])),
    ],
  }, context);

export default bootstrap;