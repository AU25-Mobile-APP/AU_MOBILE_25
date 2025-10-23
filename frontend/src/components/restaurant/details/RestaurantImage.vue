<template>
  <v-row>
    <v-col>
      <v-img
        aspect-ratio="16/9"
        cover
        :draggable="false"
        :src="image[0]"
      />
    </v-col>
  </v-row>
</template>

<script setup>

defineProps(['image']);

</script>
