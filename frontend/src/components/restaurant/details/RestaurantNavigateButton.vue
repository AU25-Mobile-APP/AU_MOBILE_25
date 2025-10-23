<template>
  <v-row>
    <v-col>
      <v-btn
        :prepend-icon="mdiNavigation"
        color="primary"
        variant="flat"
        :text="t('messages.navigateToRestaurant')"
        :href="url"
        target="_blank"
        rel="noopener nofollow"
      />
    </v-col>
  </v-row>
</template>

<script setup>
import { mdiNavigation } from '@mdi/js';
import { useI18n } from 'vue-i18n';

defineProps(['url']);

const { t } = useI18n();

</script>
