<template>
  <v-row>
    <v-col offset-lg="3" sm="12" lg="6">
      <v-row>
        <v-col>
          <create-group-btn />
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <join-group-btn />
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup>
import CreateGroupBtn from '@/components/groups/CreateGroupBtn.vue';
import JoinGroupBtn from '@/components/groups/JoinGroupBtn.vue';
</script>
