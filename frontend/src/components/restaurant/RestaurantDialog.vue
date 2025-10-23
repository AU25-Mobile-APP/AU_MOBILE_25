<template>
  <v-dialog v-model="model">
    <restaurant-card v-if="data" :restaurant="data" :show-all-details="true">
      <template #actions>
        <restaurant-navigate-button :url="data.navigationUrl" />
        <v-btn
          color="primary"
          variant="flat"
          :text="t('messages.ok')"
          @click="model = false"
        />
      </template>
    </restaurant-card>
  </v-dialog>
</template>

<script setup>
import { useI18n } from 'vue-i18n';
import RestaurantCard from './RestaurantCard.vue';
import { useRestaurant } from '@/api/restaurant';
import { watchImmediate } from '@vueuse/core';
import { ref } from 'vue';
import RestaurantNavigateButton from './details/RestaurantNavigateButton.vue';

const props = defineProps(['restaurantId']);

const model = defineModel({ required: true });

const { t } = useI18n();

const restaurantIdRef = ref();

const { data } = useRestaurant(restaurantIdRef);

watchImmediate(() => props.restaurantId, () => {
  restaurantIdRef.value = props.restaurantId;
});

</script>
