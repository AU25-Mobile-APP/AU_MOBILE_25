import 'vuetify/styles';
import { createVuetify } from 'vuetify';
import { aliases, mdi } from 'vuetify/iconsets/mdi-svg';
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';

export const vuetify = createVuetify({
  components,
  directives,
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: {
      mdi,
    },
  },
  theme: {
    defaultTheme: 'dark',
    themes: {
      light: {
        dark: false,
        colors: {
          background: '#f8f9fa',

          // Primary Blue Color
          primary: '#128ef2',
          'on-primary': '#FFFFFF',

          // secondary Color
          secondary: '#dedede',
          'on-secondary': '#000000',
        },
      },
      dark: {
        dark: true,
        colors: {
          // Primary Blue Color
          primary: '#128ef2',
          'on-primary': '#FFFFFF',

          // secondary Color
          secondary: '#424242',
          'on-secondary': '#FFFFFF',
        },
      },
    },
  },
  defaults: {
    VDialog: {
      maxWidth: 600,
      VCard: {
        VCardActions: {
          class: 'justify-end',
        },
      },
    },
  }

});
