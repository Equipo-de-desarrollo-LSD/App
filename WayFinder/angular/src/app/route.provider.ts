import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
   // 2. NUEVO ELEMENTO DE MENÚ: CIUDADES
        {
          path: '/ciudades', // <--- La ruta que definiste en app.routes.ts
          name: '::Menu:Ciudades', // El texto que se verá en el menú
          iconClass: 'fas fa-map-marker-alt', // Ícono de un pin de mapa
          order: 2, // Aparecerá después de 'Home'
          layout: eLayoutType.application,
        },
  ]);
}
