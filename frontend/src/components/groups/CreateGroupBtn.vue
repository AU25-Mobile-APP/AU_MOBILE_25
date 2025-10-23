<template>
  <v-card>
    <template #text>
      <v-btn
        color="primary"
        :text="t('messages.createGroup')"
        variant="flat"
        class="w-100"
        :prepend-icon="mdiPlus"
        @click="router.push({ name:  'CreateGroup' })"
      />
    </template>
  </v-card>
</template>

<script setup>
import { mdiPlus } from '@mdi/js';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();

const router = useRouter();

</script>
