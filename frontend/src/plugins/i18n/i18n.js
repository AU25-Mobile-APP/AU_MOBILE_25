import { createI18n } from 'vue-i18n';
import messagesDe from '@/i18n/messages/de.json';
import rulesDe from '@/i18n/rules/de.json';

export const i18n = createI18n({
  locale: 'de',
  fallbackLocale: 'de',
  messages: {
    de: {
      messages: messagesDe,
      rules: rulesDe,
    },
  },
  legacy: false,
});

export const useGlobalI18n = () => i18n;
