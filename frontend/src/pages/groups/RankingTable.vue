<template>
  <v-row v-if="data">
    <v-col>
      <ranking-header />
      <ranking
        :restaurants="data"
        @open-dialog="(restaurantId) => {
          dialogRestaurantId = restaurantId;
          openRestaurantDialog = true;
        }" />
      <restaurant-dialog v-model="openRestaurantDialog" :restaurant-id="dialogRestaurantId" />
    </v-col>
  </v-row>
</template>

<script setup>
import { useRanking } from '@/api/ranking';
import RankingHeader from '@/components/ranking/RankingHeader.vue';
import Ranking from '@/components/ranking/Ranking.vue';
import { ref } from 'vue';
import RestaurantDialog from '@/components/restaurant/RestaurantDialog.vue';

defineProps({
  id: { type: String, required: true }
});

const { data } = useRanking();

const dialogRestaurantId = ref(null);
const openRestaurantDialog = ref(false);

</script>
