import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useRestaurantsStore = defineStore('restaurants', () => {
  const restaurants = ref([]);
  const groupId = ref('');
  const restaurantIndex = ref(0);

  return { restaurants, groupId, restaurantIndex };
});
