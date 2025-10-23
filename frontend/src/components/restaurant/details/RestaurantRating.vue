<template>
  <v-row class="justify-center">
    <v-col cols="auto">
      <v-rating
        :model-value="rating"
        readonly
        color="primary"
        half-increments
        density="compact"
      />
    </v-col>
  </v-row>
</template>

<script setup>

defineProps(['rating']);

</script>
