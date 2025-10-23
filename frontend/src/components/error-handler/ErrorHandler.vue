<template>
  <v-snackbar
    v-model="isRevealed"
    color="red"
  >
    {{ revealData?.message }}

    <template #actions>
      <v-btn
        color="white"
        variant="text"
        @click="confirm"
      >
        {{ t('messages.close') }}
      </v-btn>
    </template>
  </v-snackbar>
</template>
  
  <script lang="ts" setup>
  import { ref } from 'vue';
  import { useI18n } from 'vue-i18n';
  import { VBtn } from 'vuetify/lib/components/index.mjs';
  
  import { useErrorHandler } from './ErrorHandler.context';
  
  const { t } = useI18n();
  
  const { isRevealed, confirm, onReveal } = useErrorHandler();
  
  const revealData = ref();
  
  onReveal((data) => {
    revealData.value = data;
  });
  </script>
  