<template>
  <v-row>
    <v-col cols="auto">
      <v-btn :icon="mdiArrowLeft" color="black" @click="router.back" />
    </v-col>
    <v-col>
      <h1>{{ t('messages.ranking') }}</h1>
    </v-col>
  </v-row>
</template>

<script setup>
import { mdiArrowLeft } from '@mdi/js';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();

const router = useRouter();

</script>
