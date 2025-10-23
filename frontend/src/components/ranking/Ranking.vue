<template>
  <v-row v-for="(restaurant, index) in restaurants" :key="restaurant.id" class="align-center">
    <v-col cols="auto">
      <v-icon :icon="icons[index]" size="x-large" />
    </v-col>

    <v-col>
      <v-progress-linear
        :model-value="returnProgress(restaurant.likeCount)"
        :color="colors[index]"
        height="40"
        rounded
        buffer-value="100"
      >
        <v-row>
          <v-col offset="1">
            <strong>{{ restaurant.restaurantName }}</strong>
          </v-col>
        </v-row>
      </v-progress-linear>
    </v-col>

    <v-col cols="auto">
      <v-icon :icon="mdiHeart" color="red" />
      {{ restaurant.likeCount }}
    </v-col>

    <v-col cols="auto">
      <v-btn
        color="blue"
        size="small"
        :icon="mdiInformation"
        @click="emits('openDialog', restaurant.restaurantId)"
      />
    </v-col>
  </v-row>
</template>

<script setup>
import {
  mdiHeart,
  mdiNumeric1CircleOutline,
  mdiNumeric2CircleOutline,
  mdiNumeric3CircleOutline,
  mdiNumeric4CircleOutline,
  mdiNumeric5CircleOutline,
  mdiNumeric6CircleOutline,
  mdiNumeric7CircleOutline,
  mdiNumeric8CircleOutline,
  mdiNumeric9CircleOutline,
  mdiNumeric10CircleOutline,
mdiInformation,
} from '@mdi/js';
import { ref, onMounted } from 'vue';

const props = defineProps(['restaurants']);

const emits = defineEmits(['openDialog']);

const colors = ref([]);

const icons = [
  mdiNumeric1CircleOutline,
  mdiNumeric2CircleOutline,
  mdiNumeric3CircleOutline,
  mdiNumeric4CircleOutline,
  mdiNumeric5CircleOutline,
  mdiNumeric6CircleOutline,
  mdiNumeric7CircleOutline,
  mdiNumeric8CircleOutline,
  mdiNumeric9CircleOutline,
  mdiNumeric10CircleOutline,
];

const returnProgress = (likeCount) => {
  const higestLikeCountRatio = props.restaurants[0].likeCount / 100;
  return likeCount / higestLikeCountRatio;
};

const mainColors = [
  'red',
  'pink',
  'purple',
  'deep-purple',
  'indigo',
  'blue',
  'light-blue',
  'cyan',
  'teal',
  'green',
  'light-green',
  'lime',
  'yellow',
  'amber',
  'orange',
  'deep-orange',
];

const modes = [
  'lighten-5',
  'lighten-4',
  'lighten-3',
  'lighten-2',
  'lighten-1',
  'darken-4',
  'darken-3',
  'darken-2',
  'darken-1',
  'accent-4',
  'accent-3',
  'accent-2',
  'accent-1',
];

function createRandomColors() {
  for (let i = 0; i < 10; i++) {
    const randomColorIndex = Math.floor(Math.random() * 16);
    const randomModeIndex = Math.floor(Math.random() * 13);
    colors.value.push(`${mainColors[randomColorIndex]} ${modes[randomModeIndex]}`);
  }
}

onMounted(() => {
  createRandomColors();
});

</script>
