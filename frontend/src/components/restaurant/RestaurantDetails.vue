<template>
  <transition name="fade" mode="out-in">
    <v-row v-if="currentRestaurant !== null" :key="restaurantIndex">
      <v-col cols="2" lg="3">
        <div
          class="dropZone"
          :class="{ 'swiping-left': swipingLeft }"
          @dragover.prevent
          @drop="(event) => onDropLeft(event, handleDislike)"
          @dragenter="onDragEnterLeft"
          @dragleave="onDragLeaveLeft"
        >
          <span><v-icon :icon="mdiThumbDownOutline" size="small" /></span>
        </div>
      </v-col>
      <v-col cols="8" lg="6">
        <div
          class="draggable-item"
          :draggable="true"
          @dragstart="onDragStart(item, $event)"
          @dragend="onDragEnd"
        >
          <restaurant-card :restaurant="currentRestaurant" :show-all-details="false">
            <template #actions>
              <v-btn :icon="mdiThumbDown" @click="handleDislike()" color="red" />
              <v-btn :icon="mdiInformation" @click="showRestaurantDialog = true" color="blue" />
              <v-btn :icon="mdiHeart" @click="handleLike()" color="green" />

            </template>
          </restaurant-card>
        </div>
        <restaurant-dialog v-model="showRestaurantDialog" :restaurant-id="currentRestaurant.id" />
      </v-col>
      <v-col cols="2" lg="3">
        <div
          class="dropZone"
          :class="{ 'swiping-right': swipingRight }"
          @dragover.prevent
          @drop="(event) => onDropRight(event, handleLike)"
          @dragenter="onDragEnterRight"
          @dragleave="onDragLeaveRight"
        >
          <span><v-icon :icon="mdiHeartOutline" size="small" /></span>

        </div>
      </v-col>
    </v-row>
  </transition>
</template>

<script setup>
import { mdiHeart, mdiThumbDown, mdiThumbDownOutline, mdiHeartOutline, mdiInformation } from '@mdi/js';
import { useI18n } from 'vue-i18n';
import { ref, computed } from 'vue';
import { likeRestaurant } from '@/api/restaurant';
import {
  onDragStart,
  onDragEnd,
  onDragEnterLeft,
  onDragEnterRight,
  onDragLeaveLeft,
  onDragLeaveRight,
  onDropLeft,
  onDropRight,
  swipingLeft,
  swipingRight,
} from './draggable';

import { useRestaurantsStore } from '@/stores/restaurants';
import { storeToRefs } from 'pinia';

import RestaurantCard from './RestaurantCard.vue';
import RestaurantDialog from './RestaurantDialog.vue';
import { useErrorHandler } from '../error-handler/ErrorHandler.context';

const props = defineProps(['restaurants']);

const { reveal } = useErrorHandler();
const { t } = useI18n();

const restaurantsStore  = useRestaurantsStore();
const { restaurantIndex } = storeToRefs(restaurantsStore);

const showRestaurantDialog = ref(false);

const currentRestaurant = computed(() => {
  if (restaurantIndex.value >= props.restaurants.length) {
    return null;
  }
  return props.restaurants[restaurantIndex.value];
});

const handleLike = async () => {
    try {
        await likeRestaurant(currentRestaurant.value.id);
        restaurantIndex.value++;
    } catch (error) {
        reveal({ message: t('messages.error') });
    }
};

const handleDislike = async () => {
  restaurantIndex.value++;
};


</script>

<style lang="scss" scoped>

.dropZone {
  display: flex;
  height: 100%;
  width: 100%;
  padding: 1rem;
  border: 1px dashed gray;
  justify-content: center;
  align-items: center;
  transition: background-color 300ms ease-in-out;

  &.swiping-left {
    background-color: red;
  }

  &.swiping-right {
    background-color: green;
  }
}
</style>
