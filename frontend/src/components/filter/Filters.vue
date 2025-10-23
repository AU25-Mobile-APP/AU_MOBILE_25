<template>
  <v-row>
    <v-col class="d-flex ga-3 flex-wrap justify-center">
      <v-chip v-for="filter in displayedFilters" :key="filter.id">
        {{ t(`messages.filterLabels.${filter.id}`) }}
      </v-chip>
    </v-col>
  </v-row>
</template>

<script setup>
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';

const props = defineProps(['filters']);

const { t, te } = useI18n();
const displayedFilters = computed(() => {
    return props.filters.filter(filter => te(`messages.filterLabels.${filter.id}`));
});

</script>
