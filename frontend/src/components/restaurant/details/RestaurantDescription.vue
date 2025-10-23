<template>
  <v-row>
    <v-col>
      <v-row>
        <v-col cols="12" class="text-overline">
          {{ t('messages.description') }}
        </v-col>
        <v-divider />
        <v-col cols="12">
          {{ description }}
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup>
import { useI18n } from 'vue-i18n';

defineProps(['description']);

const { t } = useI18n();

</script>

