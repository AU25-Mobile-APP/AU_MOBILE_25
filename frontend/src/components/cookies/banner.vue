<template>
  <v-banner
    class="my-4"
    color="warning"
    icon="$warning"
    lines="one"
    v-if="!store"
  >
    <v-banner-text>{{ t('messages.cookieMessage') }}</v-banner-text>

    <template #actions>
      <v-btn @click="accept">{{ t('messages.close') }}</v-btn>
    </template>
  </v-banner>
</template>
<script setup>
import { useI18n } from 'vue-i18n';
import { useLocalStorage } from '@vueuse/core';
const { t } = useI18n();
const store = useLocalStorage('cookiesAccepted');

function accept() {
    store.value = true;
}
</script>
