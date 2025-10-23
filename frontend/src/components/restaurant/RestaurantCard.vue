<template>
  <v-card :title="restaurant.name">
    <template #text>
      <restaurant-image :image="restaurant.photoUrls" />
      <restaurant-rating :rating="restaurant.rating" />
      <filters :filters="restaurant.suitable" />
      <restaurant-description v-if="showAllDetails && restaurant.description" :description="restaurant.description" />
    </template>
    <template #actions>
      <v-card-actions class="justify-space-around w-100">
        <slot name="actions"></slot>
      </v-card-actions>
    </template>
  </v-card>
</template>

<script setup>
import RestaurantImage from './details/RestaurantImage.vue';
import RestaurantRating from './details/RestaurantRating.vue';
import Filters from '@/components/filter/Filters.vue';
import RestaurantDescription from './details/RestaurantDescription.vue';

defineProps(['restaurant', 'showAllDetails']);

</script>
