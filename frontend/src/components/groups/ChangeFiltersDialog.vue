<template>
  <v-dialog v-model="model">
    <v-form>
      <v-card :title="t('messages.changeFilter')">
        <template #text>
          <v-row>
            <v-col>
              <span v-if="isPending">{{ t('messages.query.loading') }}</span>
              <span v-else-if="isError">{{ t('messages.query.error') }}</span>
              <div v-else>
                <v-checkbox
                  v-for="filter in defaultFilters"
                  :key="filter.id"
                  :label="t('messages.filterLabels.' + filter.id)"
                  hide-details
                  v-model="selectedFilters"
                  :value="filter.id"
                />
              </div>
            </v-col>
          </v-row>
        </template>
        <template #actions>
          <v-btn
            color="secondary"
            variant="flat"
            :text="t('messages.cancel')"
            @click="model = false"
          />
          <v-btn
            :loading="loading"
            color="primary"
            variant="flat"
            :text="t('messages.save')"
            @click="saveFilter()"
          />
        </template>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script setup>
import { useI18n } from 'vue-i18n';
import { useDefaultFilters, updateFilters } from '@/api/filters';
import { ref } from 'vue';
import { useGroup } from '@/api/group';
import { watchImmediate } from '@vueuse/core';
import { useErrorHandler } from '../error-handler/ErrorHandler.context';
const { reveal } = useErrorHandler();

const model = defineModel({ required: true });

const emits = defineEmits(['filtersChanged']);

const { t } = useI18n();

const loading = ref(false);
const selectedFilters = ref([]);

const { isPending, isError, data: defaultFilters } = useDefaultFilters();

const { data: group } = useGroup();

watchImmediate(group, () => {
  if (!group.value) {
    return;
  }
  selectedFilters.value = group.value.filters.map((filter) => filter.id);
});

const saveFilter = async () => {
  loading.value = true;
  const mappedFilters = selectedFilters.value.map((id) => {
    return {
      id,
      active: true
    };
  });

  try {
      await updateFilters(mappedFilters);
      emits('filtersChanged');
      model.value = false;
      loading.value = false;
  } catch (error) {
    reveal({ message: t('messages.error') });
  }
};

</script>
