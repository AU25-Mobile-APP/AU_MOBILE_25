import { useGroupIdStore } from '../group';
import { jsonEndpoint, jsonPostEndpoint } from '..';
import { useQuery } from '@tanstack/vue-query';
import { useRestaurantsStore } from '@/stores/restaurants';

const getRestaurants = async (id) => {
  if (!id) {
    return null;
  }
  try {
    return await jsonEndpoint(`groups/${id}/restaurants`);
  } catch (error) {
    throw new Error('Failed to fetch restaurants');
  }
};

const getRestaurant = async (groupId, restaurantId) => {
  if (!groupId || !restaurantId) {
    return null;
  }
  try {
    return await jsonEndpoint(`groups/${groupId}/restaurants/${restaurantId}`);
  } catch (error) {
    throw new Error('Failed to fetch restaurant with id ' + restaurantId);
  }
};

export const useRestaurants = () => {
  const groupId = useGroupIdStore();
  const restaurantsStore  = useRestaurantsStore();

  return useQuery({
    queryKey: ['restaurants', groupId],
    queryFn: async () => {
      const data =  await getRestaurants(groupId.value);
      restaurantsStore.$patch({
        restaurants: data,
        groupId: groupId.value
      });
      return data;
    },
    enabled: false,
  });
};

export const useRestaurant = (restaurantId) => {
  const groupId = useGroupIdStore();
  return useQuery({
    queryKey: ['restaurant', groupId, restaurantId],
    queryFn: async () => await getRestaurant(groupId.value, restaurantId.value),
  });
};

export const likeRestaurant = (restaurantId) => {
  const groupId = useGroupIdStore();
  return jsonPostEndpoint(`groups/${groupId.value}/restaurants/${restaurantId}/like`).then((res) => {
    return res;
  });
};
