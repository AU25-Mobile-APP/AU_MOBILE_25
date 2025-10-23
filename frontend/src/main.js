import { createApp } from 'vue';
import { createPinia } from 'pinia';
import { vuetify } from './plugins/vuetify';
import { VueQueryPlugin } from '@tanstack/vue-query';
import { setupVeeValidate } from './plugins/vee-validate';
import { googleMapsConfig } from './plugins/google-maps';
import VueGoogleMaps from 'vue-google-maps-community-fork';

import App from './App.vue';
import router from './router';
import { i18n } from './plugins/i18n/i18n';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(vuetify);
app.use(i18n);
app.use(VueQueryPlugin);
app.use(setupVeeValidate);
app.use(VueGoogleMaps, googleMapsConfig);

app.mount('#app');
