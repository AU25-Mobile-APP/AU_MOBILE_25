import { required, max_value, min_value, alpha_num, digits } from '@vee-validate/rules';
import { defineRule, configure } from 'vee-validate';
import { useGlobalI18n } from '@/plugins/i18n/i18n';
import { interpolate } from './i18n/interpolate';

export function setupVeeValidate() {
  const { global: { t } } = useGlobalI18n();

  defineRule('required', required);
  defineRule('max_value', max_value);
  defineRule('min_value', min_value);
  defineRule('alpha_num', alpha_num);
  defineRule('digits', digits);

  defineRule('coord', (value, _, context) => {
    const regex = /[0-9]*\.[0-9]*/i;
    if (regex.test(value)) {
      return true;
    }
    return t('rules.coord',  { field: context.label || t(`messages.${context.name}`) });
  });

  configure({
    generateMessage: (context) => interpolate(
      t(`rules.${context.rule?.name}`, { field: context.label || t(`messages.${context.name}`) }),
      { params: context.rule?.params || [] },
    ),
  });
}


