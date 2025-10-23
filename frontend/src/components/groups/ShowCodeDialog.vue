<template>
  <v-dialog v-model="model">
    <v-card :title="t('messages.joinCode')">
      <template #text>
        <v-row>
          <v-col class="d-flex justify-center align-center ga-3">
            <h4>{{ data?.joinCode }}</h4>
            <v-tooltip
              v-model="copied"
              location="bottom"
              :open-on-hover="false"
            >
              <template #activator="{ props }">
                <v-btn
                  v-bind="props"
                  :icon="mdiContentCopy"
                  color="primary"
                  size="small"
                  @click="copy(data?.joinCode)"
                />
              </template>
              {{ t('messages.joinCodeCopied') }}
            </v-tooltip>
          </v-col>
        </v-row>
      </template>
      <template #actions>
        <v-btn
          color="primary"
          variant="flat"
          :text="t('messages.ok')"
          @click="model = false"
        />
      </template>
    </v-card>
  </v-dialog>
</template>

<script setup>
import { useGroup } from '@/api/group';
import { useI18n } from 'vue-i18n';
import { useClipboard } from '@vueuse/core';
import { mdiContentCopy } from '@mdi/js';

const model = defineModel({ required: true });

const { data } = useGroup();

const { t } = useI18n();

const { copy, copied } = useClipboard();

</script>
