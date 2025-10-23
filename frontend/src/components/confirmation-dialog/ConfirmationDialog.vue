<template>
  <v-dialog :model-value="isRevealed" persistent>
    <v-card :text="revealData?.message" :title="revealData?.title">
      <template #actions>
        <v-btn color="secondary" variant="flat" @click="cancel">
          {{ t('messages.no') }}
        </v-btn>
        <v-btn color="primary" variant="flat" @click="confirm">
          {{ t('messages.yes') }}
        </v-btn>
      </template>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { VBtn, VCard, VDialog } from 'vuetify/lib/components/index.mjs';

import { useConfirmationDialog } from './ConfirmationDialog.context';

const { t } = useI18n();

const { isRevealed, confirm, onReveal, cancel } = useConfirmationDialog();

const revealData = ref();

onReveal((data) => {
  revealData.value = data;
});
</script>
